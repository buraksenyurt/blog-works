---
layout: post
title: "Kodla Saçmalamaca"
date: 2012-07-18 05:05:00 +0300
categories:
  - csharp
tags:
  - csharp
  - xml
  - programlama
---
Programlamaya ister yeni başlamış olun ister yıllardır bu işin içerisinde bulunun, hızlı çözüm üretmek, analitik düşünmek ve olabildiğince işe yarar parçalar çıkartmak en büyük hedeflerimizden birisi olmalıdır. Elbette yıllar içerisinde elde edinilen, kazanılan tecrübe ve bilgi birikimine bağlı olarak kendinize ait bir geliştirme (Development) tarzı da ister istemez oluşacak ve hatta sonrasında değiştirilemez/değiştirilmesi zor bir alışkanlık haline gelecektir.

[İzleyen yazı Level 100 altı bir deneyimi içermekte olup üstünde kalan geliştiricileri pekala sıkabilir][![Genius-Training](/assets/images/2012/Genius-Training_thumb.jpg)](/assets/images/2012/Genius-Training.jpg)

Makbul olan pek çok geliştirici gibi ortak bazı kurallar veya standartlar üzerinde buluşabiliyor olmaktır tabiki. Şimdi diyeceksiniz ki “yazının başlığı ve içeriği arasında nasıl bir bağ kurdun be adam?”. Aslında ispatlamak istediğim basit bir teori var.

> Her ne kadar saçma görünen bir fikrin icrası da söz konusu olsa, developer işini kafasında veya kağıt üstünde titizce planlar, araştırır, sırayla adımlar ve hatta satranç oynarmışçasına bir kaç hamle ilerisini düşünerek kodlama yapar. Sonrasında ise…Okuyalım ve görelim
>
> ![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_105.png)

Doğruyu söylemek gerekirse bu tip felsefik söylemleri veya yaklaşımları kanıtlamak veya kabul ettirmek zordur. Hatta tepki almak çok ama çok daha kolaydır. Felsefeyi anlatabilmenin belki de en kolay yolu kendi beyninizi açıp bir işi yaparken canlı canlı kağıda dökmekten geçmektedir.

Lafı fazla uzatmadan felsefemizi örnek bir fikir ile ilişkilendirip ilerlemeye çalışalım. Örneğin geliştireceğiniz Freelance uygulamalarınızda sıklıkla kullandığınız ama aslında dünya bakış açısına göre çok uzun bir zaman boyunca sabit kalan belirli veri içeriklerine ihtiyacınız oldu. Ülke adları, kodları, telefon alan kodları vb…Karar verdiniz ve dediniz ki,

> Ülkelerin isim, kod, ISO, telefon alan kodu bilgilerini tek bir XML kaynağında ele almak ve hatta gerektiğinde POCO olarak da dış ortama sunmak istiyorum. Ve…

İşte gerisini beynim nasıl yorumlamış görelim

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_105.png)

1- Ülkeler için öncelikli olarak XML veri kaynağı araştırılır ancak kafamızada tasarladığımız gibi tüm alanları içermediği görülebilir. Bulunan içerik her ihtimale karşı kayıt altına alınır. (5 dakika)

> İşin en zevkli kısımlarından birisi de ön araştırmalardır. Ancak geliştirici kendisini araştırmaya çok fazla kaptırıp, bir anda hedeflediği üretime ulaşma noktasında süre bazında sapabilir. Bu yüzden dikkatli olunmalıdır.

```xml
<?xml version='1.0' encoding='UTF-8'?> 
<Countries> 
  <Country Code="AF" ISOCode="4">Afghanistan</Country> 
  <Country Code="AL" ISOCode="8">Albania</Country> 
  <Country Code="DZ" ISOCode="12">Algeria</Country> 
  <Country Code="AS" ISOCode="16">American Samoa</Country> 
  <Country Code="AD" ISOCode="20">Andorra</Country> 
  <Country Code="AO" ISOCode="24">Angola</Country> 
… 
</Countries>
```

2 – Telefon Alan kodlarının olmadığı görülünce, bu kez bunlara ilişkin bir XML veri kaynağı araştırılır ve aşağıdakine benzer bir içerik bulunur. (5 Dakika)

```xml
<?xml version='1.0' encoding='UTF-8'?> 
<TelephoneCode> 
    <AF>93</AF> 
    <AL>355</AL> 
    <DZ>213</DZ> 
    <AD>376</AD> 
    <AO>244</AO> 
… 
</TelephoneCode>
```

3 – Elde edilen XML içeriklerine kısaca bir göz atılır. Alanlar analiz edilir. Her iki içeriğin bir arada ele alınarak tek bir çıktı üretilebileceği gözlemlenir ki bu oldukça iyidir

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_105.png)

Hedef XML içeriğinin nasıl olacağı planlanır. Bunun için kağıt kalem bile kullanılabilir. (2 Dakika)

```xml
<?xml version="1.0" encoding="utf-8" standalone="yes"?> 
<Countries> 
  <Country Name="" PhoneCode="" Code="" ISO="" /> 
  <Country Name="" PhoneCode="" Code="" ISO="" /> 
… 
</Countries>
```

4 – Bir sınıf kütüphanesi üretilip içerisine Country bilgisini taşıyacak bir POCO (Plain Old CLR Objects) tipi ile XML kaynaklarını birleştirip, tekil halde çıktı olarak verecek fonksiyonları içeren yardımcı bir sınıf yazılır. Bunu doğrudan Client uygulama içerisinde de gerçekleştirebiliriz. Ama farklı istemcilerin ihtiyacı olabileceğini düşünerekten (ve hatta belki servis olarak açarız dışarıya) atomik olarak fonksiyonlaştırmak yerinde bir hareket olacaktır. (20 Dakika)

Country POCO tipi;

```csharp
using System;

namespace Common 
{ 
    /// <summary> 
    /// Ülke 
    /// </summary> 
    public class Country 
    { 
        /// <summary> 
        /// İki karakterden oluşan ülke kodu 
        /// </summary> 
        public string CountryCode { get; set; } 
        /// <summary> 
        /// ISO kodu 
        /// </summary> 
        public short ISO { get; set; } 
        /// <summary> 
        /// Ülke adı 
        /// </summary> 
        public string Name { get; set; } 
        /// <summary> 
        /// Uluslararası telefon kodu 
        /// </summary> 
        public short PhoneCode { get; set; }

        /// <summary> 
        /// Bir ülkenin özelliklerini string formatta geri döndürür 
        /// </summary> 
        /// <returns>Ülke bilgileri</returns> 
        public override string ToString() 
        { 
            return String.Format("{0},{1},({2}),({3})", Name, CountryCode, ISO.ToString(),PhoneCode.ToString()); 
        } 
    } 
}
```

> XML Comment’ lerin eklenmesi zaman alan bir iştir. Bu istenirse en son adımlarda icra edilebilir.

CountryUtility yardımcı tipi;

```csharp
using System; 
using System.Collections.Generic; 
using System.IO; 
using System.Linq; 
using System.Xml.Linq;

namespace Common 
{ 
    /// <summary> 
    /// Country listesinin çekilmesi için gerekli yardımcı sınıftır 
    /// </summary> 
    public static class CountryUtility 
    { 
        /// <summary> 
        /// Ülke listesini geriye döndürür 
        /// </summary> 
        /// <param name="countryFile">Ülke ad,kod ve iso bilgilerinin tutulduğu XML dosya adresidir</param> 
        /// <param name="phoneCodeFile">Kod ve telefon kodu bilgilerinin tutulduğu XML dosya adresidir</param> 
        /// <returns>Ülke adı, kodu, telefon alan kodu ve ISO kod bilgilerini tutan Country tipine ait bir listedir</returns> 
        public static List<Country> GetCountryListFromXmlFile(string countryFile,string phoneCodeFile) 
        { 
            List<Country> countryList = null;

            try 
            { 
                XDocument countryDocument = XDocument.Load(countryFile); 
                XDocument telephoneDocument = XDocument.Load(phoneCodeFile);

                countryList = (from countryNode in countryDocument.Document.Root.Elements("Country") 
                               let code=countryNode.Attribute("Code").Value 
                               select new Country 
                               { 
                                   Name = countryNode.Value, 
                                   ISO = Convert.ToInt16(countryNode.Attribute("ISOCode").Value), 
                                   CountryCode = code, 
                                   PhoneCode = telephoneDocument.Root.Element(code)!=null?Convert.ToInt16(telephoneDocument.Root.Element(code).Value):(short)-1 
                               }) 
                              .OrderBy(c => c.Name) 
                              .ToList(); 
            } 
            catch (Exception excp) 
            { 
                throw excp; 
            }

            return countryList; 
        }

        /// <summary> 
        /// Toparlanan ülke listesini XML dosyasına kaydetmek üzere kullanılır 
        /// </summary> 
        /// <param name="countries">Ülkerin listesi</param> 
        /// <returns>Kayıt başarılı bir şekilde yapılmış ise fiziki dosya adres bilgisini döndürür</returns> 
        public static string WriteToXml(List<Country> countries) 
        { 
            string fileOutputPath = String.Empty; 
            if (countries.Count != 0) 
            { 
                fileOutputPath = Path.Combine(Environment.CurrentDirectory, "Country.xml"); 
                XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes")); 
                XElement root = new XElement("Countries");

                foreach (var country in countries) 
                { 
                    XElement element = new XElement( 
                               "Country" 
                               , new XAttribute("Name", country.Name) 
                               , new XAttribute("PhoneCode", country.PhoneCode) 
                               , new XAttribute("Code", country.CountryCode) 
                               , new XAttribute("ISO", country.ISO)); 
                    root.Add(element); 
                } 
                xDoc.Add(root); 
                xDoc.Save(fileOutputPath);                
            } 
            return fileOutputPath; 
        } 
    } 
}
```

> Sınıf kütüphanesinin tip modelinin aşağıdaki şekilde görüldüğü gibi tesis edildiği doğrulanır.[![UtilityAppResult_2](/assets/images/2012/UtilityAppResult_2_thumb.png)](/assets/images/2012/UtilityAppResult_2.png)

5 – Araçları kullanacak olan basit bir Console uygulaması açılır, sınıf kütüphanesi referans edilir ve içeriği aşağıdaki gibi geliştirilir. (3 Dakika)

```csharp
using System; 
using System.Linq; 
using System.IO; 
using Common;

namespace ConsoleApplication3 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            // Country listesini tutan fiziki dosya adresini ve telefon kod bilgilerini tutan fiziki dosya adreslerini alalım 
            string countryXmlFilePath = Path.Combine(Environment.CurrentDirectory, "Countries.xml"); 
            string telephoneCodeXmlFilePath = Path.Combine(Environment.CurrentDirectory, "TelephoneCodes.xml");

            try 
            { 
                // Country tipinden generic List koleksiyonunu yardımcı kütüphanedeki static fonksiyondan üretelim. 
                var countries = CountryUtility.GetCountryListFromXmlFile(countryXmlFilePath, telephoneCodeXmlFilePath); 
                // Test sonuçlarını görmek amacıyla ülke listesinin bir kısmını yazdıralım 
                foreach (var country in countries.Take(20)) 
                    Console.WriteLine(country.ToString()); 

                CountryUtility.WriteToXml(countries); 
            } 
            catch (Exception excp) 
            { 
                Console.WriteLine(excp.Message); 
            } 
        } 
    } 
}
```

6 – Program çalıştırılır ve aşağıdaki çıktıların üretilip üretilmediği gözlemlenir. (1 Dakika)

[![UtilityAppResult_1](/assets/images/2012/UtilityAppResult_1_thumb.png)](/assets/images/2012/UtilityAppResult_1.png)

ve elbetteki XML dosya içeriği kontrol edilip istenilen şekilde olup olmadığına bakılır.

[![UtilityAppResult_3](/assets/images/2012/UtilityAppResult_3_thumb.png)](/assets/images/2012/UtilityAppResult_3.png)

[![bugfrapanyone](/assets/images/2012/bugfrapanyone_thumb.jpg)](/assets/images/2012/bugfrapanyone.jpg) 7 – İlk 6 adım başarılı bir şekilde aşıldıktan sonra ise mutfağa gidilir, havanın durumuna göre soğuk veya sıcak bir içecek alınır

![Open-mouthed smile](/assets/images/2012/wlEmoticon-openmouthedsmile_29.png)

(15 Dakika)

Görüldüğü üzere toplamda 36 dakikalık sürede (kahve süresi hariç) istediğimiz üretimi gerçekleştirdik. Buraya kadarki adımları siz de sabırla uyguladıysanız eğer basit bir kod antrenmanı da yapmış olmuşsunuz demektir. Özetle neler yapmışız gelin bir bakalım.

- İki farklı XML içeriğini tek bir LINQ sorgusunda harmanladık ve POCO tabanlı generic bir List koleksiyonu ürettik (LINQ sorgusunda Let keyword’ ünü kullandık)
- Çıktı olarak üretilecek XML şemasını düşündük ve tasarladık.
- İlerki kullanımlar için bir POCO tipi tasarladık.
- Generic List koleksiyonundan yararlanıp bir XML dökümanı ve içeriğini oluşturduk.
- XML işlemleri için XDocument, XElement, XAttribute gibi tiplerden yararlandık.
- XML dökümanındaki Country elementlerini üretirken, XElement tipinin Constructor metodu içerisinde birden fazla XAttribute örneğini oluşturarak ekleme yolunu tercih ettik.

Peki her şey istenildiği gibi mi acaba? Mükemmel mi? Eksik bir şeyler hiç mi yok? Yoksa gözden kaçırdığımız bir şeyler var mı?

Doğruyu söylemek gerekirse sonuçları irdelemeden projeyi kapatırsak pek de iyi bir iş yapmamış oluruz. Söz gelimi elimizde istediğimiz ülke bilgilerini içeren bir XML dökümanımız ve bunu karşılayan POCO türevli bir nesne koleksiyonumz artık var. O halde Utility içerisindeki farklı XML kaynaklarını birleştiren metodumuz atıl olmuş durumda. Ya yeni XML kaynağından veri yükleme işini üstlenmeliyiz ya da içeriye daha akıllı bir parça koyup sonuç XML dökümanı yoksa veya içeriği boş ise tekrardan orjinal kaynaklardan çekip üretme işini ele alacak bir kod parçası eklemeliyiz. İşte bu da bizim için 8nci maddemiz oluyor.

8 – Kodun işleyişinin gözden geçirip düzeltilmesi veya eklenmesi gereken yerler var ise bunların tespit edilerek kodlanması. (??? Dakika)

???

9 – Çalışmanın tekrardan test edilmesi ve sonuçların yeniden irdelenerek istenen üretimin gerçekleştirilip gerçekleştirilmediğine bakılması ki bu 8nci madde içerisinde ela alınabilir (??? Dakika)

???

Soru işaretlerinden de anlaşılacağı üzere bu kısımlar tamamen size ait değerli okurlarım

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_105.png)

Ama düşünce yapısını az da olsa ifade edebildiğimi düşünüyorum. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ConsoleApplication3.zip (76,18 kb)](/assets/files/2012/ConsoleApplication3.zip) [Örnek bilinmez ama öyle denk geldiği için Visual Studio 2012 RC sürümünde geliştirilmiştir]