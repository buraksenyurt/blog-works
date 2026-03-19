---
title: "WCF WebHttp Services - JSON Formatlı Response Üretmek"
pubDate: 2010-02-18 22:50:00
categories:
  - WCF Eco System
  - WCF WebHttp Services
tags:
  - WCF Eco System
  - WCF WebHttp Services
  - C#
  - WCF
  - XML
  - REST
  - JSON
  - HTTP
  - JavaScript
  - Serialization
  - Generics
  - Visual Studio
  - Rc
---

# WCF WebHttp Services - JSON Formatlı Response Üretmek
![blg133_Giris.gif](images/blg133_Giris.gif)

Merhaba Arkadaşlar,

Yandaki Logo size neyi çağırıştırıyor? Aslında bakarsanız çok meşhur olan hafif siklette bir veri değiş tokuş formatının logosunu ifade etmekte. [JSON (JavaScript Object Notation).](http://www.json.org/)Hatırlayacağınız üzere bir süredir WCF Eco System içerisinde yer alan WCF WebHttp Service alt yapısını incelemeye çalışıyoruz. WCF WebHttp Service'leri eğer istemci tarafından aksi belirtilmezse varsayılan olarak XML formatında çıktı üretmektedir. Ancak istenirse JSON (JavaScript Object Notation) formatında çıktı üretmeside sağlanabilir.

Söz konusu çıktı üretim işlemi iki yolla gerçekleştirilebilir. Bilinçli olarak (Excplicitly) veya otomatik olarak. Bu yazımızda söz konusu yolları inceleyerek JSON formatında çıktıları nasıl verebileceğimizi basit bir örnek üzerinden görmeye çalışıyor olacağız. Örnek uygulamamızı bu kez Visual Studio 2010 Ultimate RC ortamı üzerinde geliştirdiğimizi belirtelim. Dolayısıyla ilerleyen sürümde bazı farklılıklar olabilir. Lesson3 isimli WCF REST Service Application uygulamamız içerisinde yer alan servis sınıfı içeriğimiz çok basit olarak aşağıdaki kod parçasından oluşmaktadır.

```csharp
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Lesson3
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class PersonalityService
    {
        [WebGet(UriTemplate = "AllPersons")]
        public List<Person> GetAllPersons()
        {
            return new List<Person>() 
            { 
                new Person() { Id = 1, Name = "Burak Selim Şenyurt",Birth=new DateTime(1976,12,1) } ,
                new Person() { Id = 2, Name = "Bill Amca",Birth=new DateTime(1975,4,5) } ,
                new Person() { Id = 3, Name = "Luka Ton-i",Birth=new DateTime(1980,3,4) } 
            };
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
    }
}
```

Servisimizde yer alan GetAllPersons isimli operasyon istemci tarafına Person tipinden bir liste içeriği döndürmektedir. Söz konusu operasyon HTTP protokolünün GET metoduna ait talepleri kabul etmektedir. Varsayılan olarak URL üzerinden yapacağımız AllPersons çağrısının sonucu aşağıdaki gibi olacaktır.

![blg133_XmlResponse.png](images/blg133_XmlResponse.png)

Ancak şimdiki hedefimiz bu XML çıktısı yerine JSON çıktısını vermektir. Bunu iki yol ile gerçekleştirebileceğimizden bahsetmiştik. Öncelikle otomatik JSON çıkıtısı üretiminin nasıl gerçekleştirilebileceğine bakalım. Bu amaçla sunucu tarafındaki web.config dosyası içerisinde yer alan webHttpEndpoint içerisindeki standardEndpoint elementinin automaticFormatSelectionEnabled niteliğinin true değere sahip olması yeterlidir. Aynen aşağıda görüldüğü gibi.

```csharp
<system.serviceModel>
    <serviceHostingEnvironment aspNetCompatibilityEnabled="true"/>
    <standardEndpoints>
      <webHttpEndpoint>
        <standardEndpoint name="" helpEnabled="true" automaticFormatSelectionEnabled="true"/>
      </webHttpEndpoint>
    </standardEndpoints>
  </system.serviceModel>
```

Peki bu otomatikliğin anlamı nedir?

![Undecided](images/smiley-undecided.gif)

Nitekim herhangibir yerde JSON çıktısı vereceğimizi belirtmedik. Dolayısıyla birisinin bunu talep ediyor olması gerekmekte. Tahmin edeceğiniz üzere burada sorumluluk istemci tarafına ait. Bir başka deyişle istemci uygulama talebini gönderirken JSON formatında bir içerik istediğini servis tarafına bildirmelidir. Dolayısıyla örneğimize aşağıdaki kodları içeren istemci uygulamayı yazarak devam etmeliyiz.

> İstemci uygulama açısından önem arz eden konulardan biriside, HttpClient tipinin kullanımı için gerekli olan WCF REST Starter Kit Preview 2 assmebly'ları ile ReadAsJsonDataContract genişletme metodunun (Extension Methods) kullanımı için gerekli olan System.ServiceModel.Web ve System.Runtime.Serialization assembly'larını referans etmesidir.

![blg133_References.gif](images/blg133_References.gif)

Buna göre istemci tarafının kodlarını aşağıdaki şekilde geliştirebiliriz.

```csharp
using System;
using Microsoft.Http;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient("http://localhost:2360/"))
            {
                HttpRequestMessage request = new HttpRequestMessage("GET", "AllPersons");                
                request.Headers.Accept.AddString("application/json");
                HttpResponseMessage response = client.Send(request);
                response.EnsureStatusIsSuccessful();
                HttpContent content=response.Content;
                Console.WriteLine(content.ReadAsString());
            }
        }
    }
}
```

Bu kod parçasında en çok dikkat edilmesi gereken nokta talep ile ilişkili Header kısmına eklenen application/json bilgisidir. Bu durumda HTTP Get metoduna göre yapılan servis çağrısı çıktısının JSON formatında olması istenmektedir. Servis tarafında da gelen isteğe göre bir çıktı üretildiğinden, WCF çalışma zamanı operasyon çıktısını JSON formatına dönüştürecektir. Kodun çalışması sonrasında aşağıdaki ekran çıktısı ile karşılaştığımızı görürüz.

![blg133_RuntimeJson.gif](images/blg133_RuntimeJson.gif)

Dikkat edileceği üzere JSON formatında bir çıktı elde edilmiştir.

İstemci tarafına gelen bu çıktının Person tipini içeren bir koleksiyon şeklinde ele alınması istediğimizdeyse HttpContent tipi üzerinden System.Runtime.Serialization.Json isim alanında yer alan ReadAsJsonDataContract genişletme metodunu çağırabiliriz. Tabi burada istemci tarafında Person tipininde bir örneğinin yer aldığını varsayıyoruz ki bunu bildiğiniz üzere WCF REST Starter Kit Preview 2 ile gelen Paste XML As Types seçeneği ile oluşturabiliriz. Eğer hatırlamıyorsanız biraz araştırmaya ne dersiniz?

![Wink](images/smiley-wink.gif)

İşte istemci tarafındaki yeni kod içeriğimiz.

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Microsoft.Http;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient("http://localhost:2360/"))
            {
                HttpRequestMessage request = new HttpRequestMessage("GET", "AllPersons");                
                request.Headers.Accept.AddString("application/json");
                HttpResponseMessage response = client.Send(request);
                response.EnsureStatusIsSuccessful();
                HttpContent content=response.Content;
                //Console.WriteLine(content.ReadAsString());
             
                List<Person> personList=response.Content.ReadAsJsonDataContract<List<Person>>();

                foreach (Person person in personList)
                {
                    Console.WriteLine("{0} {1} {2}",person.Id,person.Name,person.Birth.ToString());
                }
            }
        }
    }
}
```

Bu durumda çalışma zamanında aşağıdaki sonucu elde ederiz.

![blg133_Runtime2.gif](images/blg133_Runtime2.gif)

Gelelim bilinçli olarak çıktı formatının nasıl belirleneceğine. Öncelikli olarak neden bilinçli bir şekilde format çıktısını söylememiz gerektiğini kavramamızda yarar olduğu kanısındayım. İstemci tarafının her zaman HTTP talebinin Header kısmına müdahale etmesi söz konusu olamayabilir. Böyle bir durumda istemcinin JSON formatında talepte bulunabilmesi de mümkün değildir. Dolayısıyla bu tip bir vakada JSON formatında çıktı verileceğinin bilinçli olarak bildirilmesi gerekmektedir. Peki ya nerede ve nasıl? Cevap: Servis tarafındaki ilgili operasyon içerisinde ve bir parça kod yardımıyla

![Wink](images/smiley-wink.gif)

İşte GetAllPersons isimli servis operasyonumuzun bilinçli olarak JSON formatında çıktı veren yeni versiyonu.

```csharp
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Lesson3
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class PersonalityService
    {
        [WebGet(UriTemplate = "AllPersons?whichFormat={format}")]
        public List<Person> GetAllPersons(string format)
        {
            if (format.ToLower().Equals("json"))
            {
                WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
            }
            return new List<Person>() 
            { 
                new Person() { Id = 1, Name = "Burak Selim Şenyurt",Birth=new DateTime(1976,12,1) } ,
                new Person() { Id = 2, Name = "Bill Amca",Birth=new DateTime(1975,4,5) } ,
                new Person() { Id = 3, Name = "Luka Ton-i",Birth=new DateTime(1980,3,4) } 
            };
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
    }
}
```

İlk dikkat edilmesi gereken nokta, WebGet niteliğinde belirtilen format isimli parametre ile istemciden hangi formatta çıktı almak istendiğinin sorulmasıdır. Eğer json kelimesi yazılmışsa WebOperationContext üzerinden güncel çalışma zamanı içeriğine geçilerek cevap formatının JSON olacağı belirtilir ki buda dikkat edilmesi gereken ikinci noktadır. Tahmin edileceği üzere json dışında bir kelime girildiği takdirde varsayılan XML çıktısının üretilmesi söz konusu olacaktır. Servis operasyonumuzun bu son haline göre Internet Explorer üzerinden http://localhost:2360/AllPersons?whichFormat=json şeklinde bir talepte bulunursak, içeriği kaydetmemiz için bir iletişim penceresi ile karşılaşırız. İçeriği kaydettikten sonra Notepad programı ile açacak olursa aşağıdaki içeriğin üretildiğini görebiliriz.

![blg133_RuntimeJsonText.gif](images/blg133_RuntimeJsonText.gif)

ki buda tam anlamıyla JSON çıktısıdır.

![Smile](images/smiley-smile.gif)

Çıktının JSON veya XML harici formatlarda olması da söz konusudur aslında. Bu formatların nasıl ele alınacağını ise ilerleyen yazılarımızda değerlendirmeye çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Lesson3_RC.rar (173,29 kb)](assets/Lesson3_RC.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 Sürümünde geliştirilmiş ancak RC sürümü üzerinde de test edilmiştir]
