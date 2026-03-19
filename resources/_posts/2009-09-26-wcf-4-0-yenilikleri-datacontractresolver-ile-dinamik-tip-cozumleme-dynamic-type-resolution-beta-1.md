---
layout: post
title: "WCF 4.0 Yenilikleri - DataContractResolver ile Dinamik Tip Çözümleme(Dynamic Type Resolution) [Beta 1]"
date: 2009-09-26 16:30:00 +0300
description: "Hatırlayacağınız üzere bir önceki yazımızda, WCF serileştirme işlemlerinde Known Types sorunsalını değerlendirmeye çalışmıştık. Bu sorunsalın giderilmesinde ele alınan tekniklerden biriside KnownType niteliğinin(Attribute) kullanılmasıyıdı. Ama istersek servise uygulanacak ServiceKnownType niteliği ve başka diğer teknikleri de değerlendirebileceğimizden bahsetmiştik. Ne varki tüm bu teknikler static bir model sunmaktadır. WCF 4.0 ile birlikte, tip çözümlemelerinin(Type Resolution) dinamik olarak ele alınmasını sağlayan DataContractResovler isimli abstract bir sınıfın geldiği görülmektedir. Bu sınıf System.Runtime.Serialization.dll assembly' ının .Net Framework 4.0 versiyonunda yer almaktadır. Abstract bir sınıf olması, türetmede(Inheritance) kullanıldığı takdirde anlam kazanacak bir tip olduğunu ifade etmektedir."
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - csharp
  - dotnet
  - wcf
  - xml
  - http
  - serialization
  - visual-studio
---
# WCF 4.0 Yenilikleri - DataContractResolver ile Dinamik Tip Çözümleme(Dynamic Type Resolution) [Beta 1]
Merhaba Arkadaşlar,

Hatırlayacağınız üzere bir önceki yazımızda, WCF serileştirme işlemlerinde Known Types sorunsalını değerlendirmeye çalışmıştık. Bu sorunsalın giderilmesinde ele alınan tekniklerden biriside KnownType niteliğinin (Attribute) kullanılmasıyıdı. Ama istersek servise uygulanacak ServiceKnownType niteliği ve başka diğer teknikleri de değerlendirebileceğimizden bahsetmiştik. Ne varki tüm bu teknikler static bir model sunmaktadır. WCF 4.0 ile birlikte, tip çözümlemelerinin (Type Resolution) dinamik olarak ele alınmasını sağlayan DataContractResovler isimli abstract bir sınıfın geldiği görülmektedir. Bu sınıf System.Runtime.Serialization.dll assembly'ının.Net Framework 4.0 versiyonunda yer almaktadır. Abstract bir sınıf olması, türetmede (Inheritance) kullanıldığı takdirde anlam kazanacak bir tip olduğunu ifade etmektedir.

Aslında teori basittir. DataContractResolver sınıfı iki abstract metod tanımlaması içerir. ResolveType ve ResolveName. Bu metodlar tahmin edileceği üzere, tip çözümlemesinde serileştirme (Serialization) ve ters-serileştirme (DeSerialization) işlemlerinde bir veya daha fazla Known Type'ın ele alınması gerektiği durumlarda devreye girmektedir. Çok doğal olarak metodların uygulanması için bir sınıfın DataContractResolver tipinden türetilmesi gerekir. O halde "türetilen ve tip çözümlemesi işlerini üstlenen sınıf nerede kullanılır?" sorusu da ortaya çıkmaktadır

![Wink](/assets/images/2009/smiley-wink.gif)

Bunun için farklı teknikler olmasına rağmen belkide en basiti, DataContractSerializer nesne örneği oluşturulurken yapılan bildirimdir.

Böylece, DataContractSerializer nesne örneğinin uygulayacağı serileştirme ve ters-serileştirme işlemleri sırasında karşılaşılabilecek olası Known Type sorunlarında başvurulabilecek bir yardımcı belirlenmiş olmaktadır ki bu yardımcı, DataContractResolver türevi olan bir sınıftır. İşe bu açılardan bakıldığında, DataCotractResolver sayesinde dinamik tip çözümleme yeteneğine (Dynamic Type Resolution) sahip olduğumuzu görebiliriz. Aslında kafalarımızı dahada karıştırmadan önce dilerseniz bir önceki yazımızda ele aldığımız ve Known Type sendromuna neden olan örneğimizi ele alıp ilerlemeye çalışalım. (Örneklerimizi Visual Studio 2010 Beta 1 ve.Net Framework Beta 1 üzerinde geliştirdiğimizi hatırlatmak isterim. Yani bir sonraki sürümde Beta 1 pek çok farklılık olabilir ![Undecided](/assets/images/2009/smiley-undecided.gif))

```csharp
using System;
using System.Runtime.Serialization;
using System.Xml;

namespace UsingDataContractResolver
{
    [DataContract]
    class Product
    {
        [DataMember]
        public object Information { get; set; }
    }

    [DataContract]
    class ProductInformation
    {
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public int Id { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            XmlObjectSerializer serializer = new DataContractSerializer(typeof(Product));

            serializer.WriteObject(new XmlTextWriter(Console.Out) { Formatting = Formatting.Indented }
                , new Product { Information = new ProductInformation { Id = 1000, Summary = "Özet bilgi" } });
       }
    }
}
```

Hatırlayacağınız üzere object tipinden tanımlanmış olan Information özelliğine, ProductInformation tipinden bir nesne örneği atandığında ve Known Type ile ilişkili bir bildirimde bulunmadığımızda, çalışma zamanı hatası almaktaydık. Bu sefer örneğimizi.Net Framework 4.0 tabanlı olarak derleyip çalıştıracağız. Çok doğal alarak SerializationException tipinden bir istisna mesajı almayı bekliyoruz. Ancak bu sefer, SerializationException mesajı içerisinde DataContractResolver kullanılmasının da önerildiği görülmektedir.

![blg85_Exception.gif](/assets/images/2009/blg85_Exception.gif)

Peki ya çözüm?

Zaten.Net 3.5 sürümünde KnownType niteliği gibi materyalleri kullanarak bu sorunu aşabilmekteyiz. Ancak WCF 4.0 ile birlikte sunulan DataContractResolver abstract sınıfı sayesinde, söz konusu sorunu çalışma zamanında dinamik olarak değerlendirme şansına sahibiz. Şimdi örneğimizi buna göre revize edeceğiz. İlk yapmamız gereken DataContractResolver türevli bir sınıfın tasarlanması olacaktır. Aynen aşağıda görüldüğü gibi.

![blg85_ClassDiagram.gif](/assets/images/2009/blg85_ClassDiagram.gif)

```csharp
class ProductInformationResolver
        :DataContractResolver
    {
        public override Type ResolveName(string typeName, string typeNamespace, DataContractResolver knownTypeResolver)
        {
            if (typeName == "ProductInfo" 
                && typeNamespace == "http://www.adventure.com/resolver/productInformationType")
                return typeof(ProductInformation);
            else
                return knownTypeResolver.ResolveName(typeName, typeNamespace, null);
        }

        public override void ResolveType(Type dataContractType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (dataContractType == typeof(ProductInformation))
            {
                XmlDictionary dictionary = new XmlDictionary();
                typeName = dictionary.Add("ProductInfo");
                typeNamespace = dictionary.Add("http://www.adventure.com/resolver/productInformationType");
            }
            else
            {
                knownTypeResolver.ResolveType(dataContractType, null, out typeName, out typeNamespace);
            }
        }
    }
```

Güzel...

![Laughing](/assets/images/2009/smiley-laughing.gif)

Şimdi bir kaç noktayı açıklığa kavuşturmaya çalışalım. Öncelikli olarak DataContractResolver tipine ait iki metodun ezildiğini (override) görmekteyiz. ResolveType metodu serileştirme işlemi sırasında devreye girmektedir ve içeride kontrol edilen tipin XML'de nasıl ifade edileceğini belirtmektedir (xsi:type tanımlaması). Metodda ilk olarak dataContractType parametresinin çalışma zamanında ProductInformation olup olmadığı kontrol edilir. Eğer ProductInformaion tipindense yeni bir XmlDictionary nesnesi örneklenir ve typeName ile typeNamespace değerleri bu nesne üzerine Add metodu ile set edilir.

Zaten typeName ve typeNamespace parametrelerinin out tipinden oldukları gözden kaçmamalıdır. Bir başka deyişle bu parametre değerleri, ResolveType metodunun çağırıldığı ortama aktarılmaktadır.(Out ve Ref parametrelerini hatırlıyorsunuz değil mi? ![Wink](/assets/images/2009/smiley-wink.gif)) Kısacası, serileştirme işlemi sırasında eğer ProductInformation tipi ile karşılaşılırsa, tip çözümlemesinin nasıl yapılacağı geliştiricinin isteği doğrultusunda tanımlanabilmektedir. Burada yer alan typeName veya typeNamespace değerlerinin herhangibir dış ortamdan alınabileceğini (örneğin parametrik bir XML tablosu birden fazla tipin çözümlenmesi sırasında değerlendirilebilir) belirtmekte yarar olduğu kanısındayım.

ResolveName metodu ise tahmin edileceği üzere ters serileştirme (DeSerialization) işlemi sırasında devreye girmektedir. Metod içerisinde ilk olarak typeName ve typeNamespace değişkenleri kontrol edilir ve buna göre geriye döndürülecek tip (Type) belirlenir. Bu metod içerisinde de nesne tipi (Object Type) ve xsi:type eşleştirmeleri için bir referans veri kaynağı (örneğin bir XML içeriği) kullanılabilir.

Peki ya bundan sonrası? Çalışma zamanı ProductInformationResolver tipini kullanacağını nereden bilecek? İşte cevap...

```csharp
XmlObjectSerializer serializer = new DataContractSerializer(typeof(Product),null,Int32.MaxValue,false,false,null,new ProductInformationResolver());
            serializer.WriteObject(new XmlTextWriter(Console.Out) { Formatting = Formatting.Indented }
                , new Product { Information = new ProductInformation { Id = 1000, Summary = "Özet bilgi" } });
```

Dikkat edileceği üzere DataContractSerialiazer nesne örneği oluşturulurken son parametre olarak ProductInformationResolver örneği verilmektedir. Buna göre serializer nesnesinin yapacağı serileştirme ve ters-serileştirme işlemleri sırasında, ProductInformationResolver nesne örneği devreye girecektir. Örneğimizi bu haliyle deneyecek olursak, çalışma zamanında aşağıdaki sonuçların üretildiğini görebiliriz.

![blg85_Runtime.gif](/assets/images/2009/blg85_Runtime.gif)

Görüldüğü üzere Information elementi içerisinde, DataContractResolver türevli olan ProductInformationResolver nesne örneğine ait ResolveType metodu içerisinde belirlenen, typeName ve typeNamespace değerleri yer almaktadır. Elbetteki serileştirilen nesnenin ters-Serileştirme işlemi sırasında da işlemlerin başarılı bir şekilde yürütüldüğü gözlemlenebilir. Ama yinede kodumuzu aşağıdaki gibi revize edip ters serileştirme işleminin çalıştığından emin olmalıyız.

```csharp
static void Main(string[] args)
{
	FileStream fs=new FileStream("Product.xml",FileMode.Create,FileAccess.Write);
	serializer.WriteObject(fs
		, new Product { Information = new ProductInformation { Id = 1000, Summary = "Özet bilgi" } });
	fs.Close();

	Product product=(Product)serializer.ReadObject(new FileStream("Product.xml",FileMode.Open,FileAccess.Read));
	ProductInformation information=(ProductInformation)product.Information;
	Console.WriteLine("{0} {1}",information.Id,information.Summary);
}
```

Bu kez Product.xml dosyası içerisinde serileştirme işlemini yaptıktan sonra ReadObject metodu yardımıyla XML kaynağından okuma işlemini gerçekleştirmekteyiz. ReadObject metodu geriye object türünden bir referans döndürdüğü için, bilinçli bir tür dönüşümü (Explicitly Type Cast) yapılmaktadır. Sonrasında ise product nesne örneği üzerinden Information özelliğine gidilmekte ve ProductInformation tipine dönüştürülen referansın, Id ve Summary değerlerine bakılmaktadır. İşte çalışma zamanı sonucu.

![blg85_SecondRun.gif](/assets/images/2009/blg85_SecondRun.gif)

WCF 4.0 son sürümü ile gelmesi muhtemel olan bu yenilik sayesinde, Known Type durumlarının çalışma zamanında dinamik olarak değerlendirilmesi sağlanmaktadır. Bu özelliğin geliştiricilere daha büyük bir esneklik sunduğuda ortadadır. Böylece geldik WCF 4.0 ile ilgili bir yeniliğin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.