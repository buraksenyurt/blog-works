---
title: "WCF Serileştirme(Serialization)"
pubDate: 2008-06-16 12:00:00
categories:
  - WCF
tags:
  - WCF
  - C#
  - .NET
  - ASP.NET
  - Silverlight
  - XML
  - JSON
  - Web Service
  - HTTP
  - Java
  - JavaScript
  - Performance
  - Serialization
---

# WCF Serileştirme(Serialization)
Değerli Okurlarım Merhabalar,

Serileştirme (Serialization) ve çözümleme (Encoding) çoğu zaman bir birlerine karıştırılan kavramlar olabilmektedir. Oysaki aralarında çok ince ama bir o kadarda önemli farklılıklar vardır. Serileştirme ve çözümleme, SOA (Service Oritented Architecture) tarzındaki uygulama çözümlerinde sıklıkla kullanılmaktadır. Nitekim bu tip mimarilerde servis ve istemci arasında yapılan veri transferlerinde bilginin serileştirilmesi ve mesajların çözümlenmesi gerekmektedir.

Serileştirme özet olarak nesne grafiğinin (Object Graph) byte dizisine dönüştürülmesi olarak düşünülebilir. Bu sayede çalışma zamanı (Run-Time) nesne örneklerinin herhangibir kaynakta sürekli olarak saklanması mümkün olabilmektedir. Bir başka deyişle nesneye ait veri içeriğinin, bir dosyada, veritabanında, bellekte tutulması mümkündür. Ancak en önemli olan kısmı söz konusu nesne içeriğinin başka bir ortama herhangibir protokol üzerinden taşınabiliyor olmasıdır ki bu dağıtık uygulama (Distributed Applications) çözümlerinde kilit rollerden birisidir.

Peki duruma WCF (Windows Communication Foundation) açısından bakıldığında göze çarpan noktalar nelerdir? WCF tarafında serileştirme sadece nesne grafiğinin bir byte dizisine dönüştürülmesi olarak yorumlanmaz. Bunun yerine nesne grafiğinin bir XML InfoSet (XML Information Set) içeriğine dönüştürülmesi olarak yorumlanır. XML InfoSet içeriği WCF mesajlarının oluşturulmasında kullanılmaktadır. XML InfoSet, XML'in bir üst kümesi olarak düşünülebilir. XML InfoSet sayesinde, XML içeriğinin sadece Text formatında olma zorunluluğu ortadan kalkmaktadır. Bir başka deyişle nesne içeriğinin, örneğin binary XML formatında üretilmesi mümkün olabilmektedir. Bu WCF açısından önemlidir çünkü interoperability, performans gibi konularda XML formatının Text bazlı olmayan versiyonlarının kullanılmasının avantajlarından yararlanılabilinir.

> XML InfoSet, WCF mimarisine özgü bir kavram değildir. Nitekim Asp.Net Web Service modelindeki uygulamalarda XML InfoSet yaklaşımını kullanmaktadır.

WCF mimarisinde kullanılmakta olan serileştirici tipler aşağıdaki gibidir.

![mk254_1.gif](images/mk254_1.gif)

Çözümleme (Encoding) kısaca, WCF mesajlarının byte dizisi haline dönüştürülmesi olarak ele alınabilir. Bu sayede mesajın içeriğinin iletişim kanalları (Transport Channels) üzerinden aktarılabilmesi mümkün olmaktadır. WCF mimarisi temel olarak beş farklı çözümleme formatını (Encoding Formats) desteklemektedir.

![mk254_2.gif](images/mk254_2.gif)

WCF tarafında duruma göre yukarıda bahsedilen formatları ele alan hazır encoder tipleri bulunmaktadır. Özellikle.Net uygulamaları arasında bir mesajlaşma söz konusu ise performans adına BinaryMessageEncoder, platformlar arası uyumluluk (Interoperability) gerekiyorsa TextMessageEncoder veya MtomMessageEncoder, Ajax tabanlı web istemcilerinin yer aldığı senaryolarda ise, JsonMessageEncoder tipleri devreye girmektedir. Önemli olan noktalardan biriside WCF tarafında encoding sisteminin genişletilebilmesidir. Yani yeni çıkan çözümleme formatlarına uygun eklemeler ve ilaveler yapılabilir.

Serileştirme ve çözümleme WCF tarafında bir arada düşünülmesi gereken konulardır. Nitekim, nesne içeriğinin serileştirilerek XML InfoSet haline getirilmesi sadece sürecin ilk adımıdır. Sonrasında bu InfoSet bilgisinden yararlanılarak karşı tarafa gönderilecek olan mesajın üretilmesi için bir çözümleme (Encoding) işlemi yapılır. Bu sürecin sonunda oluşturulan mesaj, iletişim kanalları üzerinden karşı tarafa gönderilir. Burada hatırlanması gereken noktalardan biriside, encoding işlemlerinde çalışma zamanında oluşan kanal yığının (Channel Stack) içeriğidir. Bilindiği gibi bu yığınında mutlaka Encoding Channel ve Transport Channel kanallarının olması gerekir. İşte Encoding Channel içerisinde, serileştirilerek InfoSet haline gelen bilginin ilgili Encoder tipine göre çözümlenerek mesaj içeriği haline getirilmesi sağlanmaktadır.

İlk olarak WCF serileştirme opsiyonlarına bakmakta yarar bulunmaktadır. WCF, varsayılan olarak DataContractSerializer tipini baz alarak serileştirme işlemlerini gerçekleştirmektedir. DataContractSerializer, nesneyi serileştirirken XSD şemalarını kullanır. Bir başka deyişle söz konusu CLR (Common Language Runtime) tipini, karşılığı olan XSD tipi ile ifade eder. Bu sayede farklı platformların serileştirilen nesneyi kullanabilme imkanı doğar. Örneğin.Net tarafındaki System.String tipi XSD tarafında xs:String olarak ele alınır.

Bu tipin kullanıldığı veri sözleşmesini (Data Contract) kullanacak olan bir java uygulamasıda java.lang.String tipini karşılık olarak ele alır. Bir başka deyişle XSD formatı ilkel veri tiplerini baz alaraktan bir köprü vazifesini görür. Elbette karmaşık tiplerde (Complex Type) ilkel tip seviyelerine indirilerekten serileştirme işlemine tabi tutulurlar. Tabiki burada göz ardı edilmemesi gereken önemli bir nokta vardır. Özellikle karmaşık tiplerde DataContract ve DataMember nitelikleri (attributes) ile serileştirme işlemi sağlanmaktadır. (İlerleyen örneklerde aşağıdaki yer alan Urun isimli tip kullanılmaktadır.)

![mk254_3.gif](images/mk254_3.gif)

```csharp
using System;
using System.Runtime.Serialization;

namespace Formatters
{ 
    [DataContract(Namespace="http://www.bsenyurt.com/Urun")]
    class Urun
    {
        private int _id;
        private string _ad;
        private double _listeFiyati;
        private DateTime _stokTarihi;

        public Urun(int id, string ad, double listeFiyati, DateTime stokTarihi)
        {
            Id = id;
            Ad = ad;
            ListeFiyati = listeFiyati;
            StokTarihi = stokTarihi;
        }

        [DataMember]
        public DateTime StokTarihi
        {
            get { return _stokTarihi; }
            set { _stokTarihi = value; }
        }

        [DataMember]
        public double ListeFiyati
        {
            get { return _listeFiyati; }
            set { _listeFiyati = value; }
        }

        [DataMember]
        public string Ad
        {
            get { return _ad; }
            set { _ad = value; }
        }

        [DataMember]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }
}
```

DataContractSerializer ile Serileştirme

DataContractSerializer tipi.Net Framework 3.0 ile birlikte gelen ve WCF tarafında varsayılan olarak kullanılan bir serileştiricidir. Yukarıda yer alan Urun sınıfına ait nesne örneğini serileştirmek amacıyla DataContractSerializer tipi, örnek bir Console uygulamasına ait aşağıdaki kod parçası ile ele alınabilir.(Söz konusu Console uygulaması.Net Framework 3.5 şablonunda geliştirilmiştir.)

```csharp
using System;
using System.IO;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace Formatters
{
    class Program
    {
        static void Main(string[] args)
        {
            #region DataContractSerilazier ile Type Serileştirme

            // Serileştirilecek nesne örneği.
            Urun mouse=new Urun(1,"Mx Mouse Optic",10,new DateTime(2004,1,1));

            // Serileştirici sınıf örneklenir.
            // Parametre olarak serileştirilecek veri sözleşmesi tipi verilir.
            DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Urun));

            // Serileştirme sonuçlarının yazılacağı örnek bir Stream oluşturulur.
            FileStream stream2 = new FileStream("Urun.xml", FileMode.Create, FileAccess.Write);
            // WriteObject metodu ile stream2 nesnesi ile tanımlanan Stream üzerine mouse isimli Urun nesne örneği verisi aktarılır
            dcSerializer.WriteObject(stream2, mouse);
            stream2.Close(); // Stream kapatılır.

            #endregion

            #region DataContractSerializer ile Type Ters-Serileştirme 

            // Urun.xml dosyası var ise ters serileştirme işlemleri yapılır.
            if (File.Exists("Urun.xml"))
            {
                // DataContractSerializer nesnesi örneklenir.
                DataContractSerializer dcDeSerializer = new DataContractSerializer(typeof(Urun));
                // Serileştirilmiş veri içeriğinin bulunduğu dosyayı okumak için bir Stream örneklenir.
                FileStream stream5 = new FileStream("Urun.xml", FileMode.Open, FileAccess.Read);
                // ReadObject metodu ile parametre olarak verilen Stream içerisindeki bilgi ters serileştirme işlemine tabi tutulur.
                // Dönüş türü object olduğu için sonucun uygun türe cast edilmesi gereklidir.
                Urun urn=(Urun)dcDeSerializer.ReadObject(stream5);
                // Elde edilen nesne örneği bilgisi ekrana yazdırılır.
                Console.WriteLine("Id :{0} Ad:{1} Fiyat:{2} Stok Tarihi:{3}", urn.Id.ToString(), urn.Ad, urn.ListeFiyati.ToString("C2"), urn.StokTarihi.ToString());
                // Stream kapatılır.
                stream5.Close();
            }

            #endregion

            #region DataContractSerializer ile Array Serileştirme
        
            Urun[] urunler ={
                                        new Urun(2,"A Mouse Optic",11.2,DateTime.Now),
                                        new Urun(3,"Y Mouse Optic, Kablolu",9.49,DateTime.Now),
                                        new Urun(5,"Z Mouse",3.45,new DateTime(2008,2,3)),
                                    };    

            // DataContractSerializer nesne örneğini oluşturulurken parametre olarak Urun tipinden dizi verilmiştir
            DataContractSerializer dcArraySerializer = new DataContractSerializer(typeof(Urun[]));
            FileStream stream3 = new FileStream("Urunler.xml", FileMode.Create, FileAccess.Write);
            dcArraySerializer.WriteObject(stream3, urunler);
            stream3.Close();
        
            #endregion

            #region DataContractSerializer ile Array Ters-Serileştirme
        
            if (File.Exists("Urunler.xml"))
            {
                DataContractSerializer dcDeSerializer = new DataContractSerializer(typeof(Urun[]));
                FileStream stream6 = new FileStream("Urunler.xml", FileMode.Open, FileAccess.Read);
                Urun[] gelenUrunler = (Urun[])dcDeSerializer.ReadObject(stream6);
                Console.WriteLine("Ürünler");
                foreach (Urun urun in gelenUrunler)
                {
                    Console.WriteLine("Id :{0} Ad:{1} Fiyat:{2} Stok Tarihi:{3}", urun.Id.ToString(), urun.Ad, urun.ListeFiyati.ToString("C2"), urun.StokTarihi.ToString());
                } 
                stream6.Close();
            }

            #endregion
        }
    }
}
```

Bu kod parçasında Urun tipine ait bir nesne örneği ve bir dizinin serileştirilmesi ve serileşen dosya içeriğinden tekrardan elde edilmesi (DeSerialization) işlenmektedir. Serileştirme işleminde WriteObject metodu, ters serileştirme işleminde ise ReadObject metodları kullanılmaktadır. Serileştirme ve TersSerileştirme işlemlerinde tipe ait bilgilerin alınması için DataContractSerializer nesne örneğinin üretimi sırasında devreye giren yapıcı (Constructor) metoddan yararlanılır. Dikkat edileceği üzere, Urun ve Urun[] tip bilgileri yapıcı metoda parametre olarak verilmektedir. ReadObject metodu, ilgili Stream üzerinden gelen veriyi okumakta ve ters serileştirme işlemine tabi tutmaktadır. Elbette dönen veri türü object olduğundan uygun tipe dönüştürülmesi gerekir. Örnekte dosya sistemi Stream olarak kullanılmaktadır. Uygulamanın çalışması sonrasında serileştirme işlemi sonucu üretilen Urun.xml ve Urunler.xml dosyalarının içerikleri aşağıdaki gibi olacaktır.

Urun.xml içeriği;

![mk254_5.gif](images/mk254_5.gif)

Urunler.xml içeriği;

![mk254_6.gif](images/mk254_6.gif)

Çalışma zamanında oluşan ekran çıktısı ise aşağıdaki gibidir. Dikkat edileceği üzere Stream içerisindeki bilgilerden Urun ve Urun[] tipleri elde edilerek sonuçlar ekrana yazdırılmaktadır.

![mk254_4.gif](images/mk254_4.gif)

Burada merak edilen noktalardan biriside mesajlaşma sırasında kullanılan XSD şemasının içeriğinin ne olduğudur..Net Framework 3.0 ile birlikte gelen XsdDataContractExporter tipinden yararlanılaraktan, söz konusu XSD şemasının içeriği manuel olaraktan üretilebilir ve incelenebilir. Aşağıdaki kod parçasında bu durum ele alınmaktadır.

```csharp
// XSD formatına dönüştürme yapacak tip tanımlanır
XsdDataContractExporter exporter = new XsdDataContractExporter();
exporter.Options = new ExportOptions();
exporter.Export(typeof(Urun)); // Export işlemi gerçekleştirilir

// Export işlemi sonucu oluşan XSD bilgisi herhangibir stream üzerine aktarılabilir
FileStream stream = new FileStream("UrunSchema.xsd", FileMode.Create, FileAccess.Write);

// Export edilen XmlSchemaSet içerisindeki tüm XmlSchema tipleri dolaşılır
foreach (XmlSchema set in exporter.Schemas.Schemas())
{
    // Çıktının kolay anlaşılır olması açısından o anki XmlSchema örneğinin TargetNamespace özelliğinin değerinin DataContract niteliğinde belirtilen Namespace bilgisine eşit olup olmadığına bakılır
    if (set.TargetNamespace == "http://www.bsenyurt.com/Urun")
        set.Write(stream); // Eşit ise XmlSchema içeriği dosyaya yazılır
}
stream.Close();
```

Bu kodun çalışması sonucu aşağıdaki ekran görüntüsünde yer alan XSD şemasının üretildiği görülür. (Burada if ifadesi kaldırıldığı takdirde içerikte oluşacak farklılığın izlenmesi ve analiz edilmesi önerilir.)

![mk254_7.gif](images/mk254_7.gif)

Görüldüğü gibi Urun isimli karmaşık tip içerisindeki ilkel tiplerin (Primitive Type) tamamı, XSD karşılıklarına çevrilerek ele alınmaktadır. İşte DataContractFormatter ile serileştirilen bir Urun nesne örneği iki uygulama arasında hareket ederken bu şema bilgisinden yararlanılmaktadır.

NetDataContractSerializer ile Serileştirme

Bazı durumlarda hem istemci hemde sunucu tarafında veri sözleşmelerinin aslına uygun şekilde kullanıldığı durumlar söz konusu olabilir. Bu özellikle aynı tip.Net uygulamaların olduğu senaryolarda söz konusudur. Bu sebepten WCF içerisinde buna destek olacak şekilde NetDataContractSerializer tipi kullanılmaktadır. Ne yazıkki WCF'in bu tipe doğrudan desteği yoktur. Bu sebepten ekstra kod yazılması ve WCF tarafında serileştirilecek olan tipin çalışma zamanı için özel bir nitelik (attribute) tasarlanması gerekmektedir. Yukarıda tasarlanan örnek için NetDataContractSerializer aşağıdaki kod parçasında olduğu gibi kullanılabilir.

```csharp
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Formatters
{
    class Program
    {
        static void Main(string[] args)
        {
            #region NetDataContractSerializer ile Type Serileştirme

            // Urun tipine ait nesne örneği oluşturulur.
            Urun lcd = new Urun(9, "LCD 17inch", 125.45, DateTime.Now);
            // NetDataContractSerializer nesnesi örneklenir.
            NetDataContractSerializer netSerializer = new NetDataContractSerializer();
            // Serileştirmenin yapılacağı fiziki dosyayı işaret eden Stream açılır.
            FileStream stream4 = new FileStream("NetUrun.xml", FileMode.Create, FileAccess.Write);
            // WriteObject metodu ile lcd isimli Urun nesne örneği, stream4 ile belirtilen Stream üzerine aktarılır.
            netSerializer.WriteObject(stream4, lcd);
            // Stream kapatılır
            stream4.Close();

            #endregion

            #region NetDataContractSerializer ile Type Ters Serileştirme
    
            // NerUrun.xml dosyası var ise işlemleri yap.
            if (File.Exists("NetUrun.xml"))
            {
                // NetDataContractSerializer nesnesi örneklenir
                NetDataContractSerializer netDeSerializer = new NetDataContractSerializer();
                // FileStream nesnesi örneklenir
                FileStream stream7 = new FileStream("NetUrun.xml", FileMode.Open, FileAccess.Read);
                // ReadObject metodu parametre olarak ters serileştirilecek tipe ait verileri taşıyan Stream örneğini alır.
                // Metod geriye object tipini döndürdüğü için cast işlemi yapılır.
                Urun gelenLcd = (Urun)netDeSerializer.ReadObject(stream7);
                // Elde edilen nesneye ait bilgiler ekrana yazdırılır
                Console.WriteLine("Id :{0} Ad:{1} Fiyat:{2} Stok Tarihi:{3}", gelenLcd.Id.ToString(), gelenLcd.Ad, gelenLcd.ListeFiyati.ToString("C2"), gelenLcd.StokTarihi.ToString());
                // Stream kapatılır
                stream7.Close();
            }

            #endregion

            #region NetDataContractSerializer ile Array Serileştirme
        
            Urun[] urunler ={
                                    new Urun(2,"A Mouse Optic",11.2,DateTime.Now),
                                    new Urun(3,"Y Mouse Optic, Kablolu",9.49,DateTime.Now),
                                    new Urun(5,"Z Mouse",3.45,new DateTime(2008,2,3)),
                                    };

            NetDataContractSerializer netdcArraySerializer = new NetDataContractSerializer();
            FileStream stream8 = new FileStream("NetUrunler.xml", FileMode.Create, FileAccess.Write);            
            netdcArraySerializer.WriteObject(stream8, urunler);
            stream8.Close();
        
            #endregion
    
            #region NetDataContractSerializer ile Array Ters Serileştirme
    
            if (File.Exists("NetUrunler.xml"))
            {
                NetDataContractSerializer netdcDeSerializer = new NetDataContractSerializer();
                FileStream stream9 = new FileStream("NetUrunler.xml", FileMode.Open, FileAccess.Read);
                Urun[] gelenUrunler = (Urun[])netdcDeSerializer.ReadObject(stream9);
                Console.WriteLine("Net Ürünler");
                foreach (Urun urun in gelenUrunler)
                {
                    Console.WriteLine("Id :{0} Ad:{1} Fiyat:{2} Stok Tarihi:{3}", urun.Id.ToString(), urun.Ad, urun.ListeFiyati.ToString("C2"), urun.StokTarihi.ToString());
                }
                stream9.Close();
            }

            #endregion
        }
    }
}
```

Örnekte Urun ve Urun[] dizilerinin NetDataContractSerializer kullanılarak serileştirme işlemlerine tabi tutulması ele alınmaktadır. Dikkat edileceği üzere NetDataContractSerializer tipine ait nesneler örneklenirken DataContractSerializer'da olduğu gibi tip bildirimi yapılmamaktadır. Bunun sebebi, serileştirilen tipin zaten uygulama tarafındaki ilgili assembly içerisinde var olmasıdır. Diğer taraftan yine WriteObject metodu ile serileştirme, ReadObject metodu ilede ters serileştirme işlemleri gerçekleştirilmektedir. Üretilen NetUrun.xml ve NetUrumler.xml dosyalarının içerikleri ise aşağıda görüldüğü gibidir.

NetUrun.xml içeriği;

![mk254_8.gif](images/mk254_8.gif)

NetUrunler.xml içeriği;

![mk254_9.gif](images/mk254_9.gif)

Dikkat edileceği üzere DataContractSerializer ile üretilen XML çıktılarında farklı bir sonuç oluşmaktadır. Herşeyden önce z:Assembly niteliği içerisinde Urun tipinin yer aldığı Assembly bilgisi bulunmaktadır. Bunun önemli bir sonucu vardır. Serileşen içeriğin ele alındığı taraflarda Urun tipinin yer aldığı Assembly'ın var olması şarttır ki aslında bu durum SOA mimarisinin uygulanış biçimlerinde tercih edilen yollardan birisi değildir. İçerikte göze çarptan noktalardan biriside z:Id isimli niteliklerdir (Attributes). Bu niteliklerin sadece referans tiplerine uygulandığına dikkat edilmelidir (String,Array gibi). z:Id temel olarak referans tiplerinin korunmasına yönelik olarak kullanılmaktadır. Bir diğer dikkat çekici nitelik ise Urun[] dizisinin serileştirilmesi sonrası ortaya çıkan z:Size bilgisidir. Buradada serileştirilen dizi içerisindeki eleman sayısı yer almaktadır. Özet olarak NetDataContractSerializer, WCF alt yapısında doğrudan desteklenmemektedir. Her iki taraftada tipe ait Assembly bilgisinin olmasını gerektirmektedir. Üstüne üstelik nesne örneğinin bilinen.Net CLR tiplerine eşleştirilerek XML içeriğine alınmasını sağlamaktadır. Bu sebeplerden dolayı ilgili serileştiricinin kullanılması pek yaygın değildir.

DataContractJsonSerializer ile Serileştirme

Bilindiği üzere.Net Framework 3.5 ile birlikte WCF mimarisinde Ajax tabanlı istemciler için destek gelmektedir. Bu desteğin temelinde ise JSON (JavaScript Object Notation) formatlı veriler söz konusudur. Bu noktada WCF alt yapısı DataContractJsonSerializer tipini ele almaktadır. Bu tipte serileştirme desteği için WebScriptEnablingBehavior niteliğinin veya WebHttpBehavior niteliğinde çözümle için JSON tipinin seçilmesi yeterlidir. Söz konusu serileştirme tipi sayesinde Javascript, Asp.Net Ajax ve Silverlight tabanlı web uygulmalarına destek verilebilmektedir. Aşağıdaki kod parçasında yukarıdaki örneklerde kullanılan Urun ve Urun[] dizi örneklerinin JSON serileştirilmesi ele alınmaktadır. (DataContractJsonSerializer tipi System.Runtime.Serialization.Json isim alanı (Namespace) altında yer almaktadır. Ancak bu isim alanına erişebilmek için projeye System.ServiceModel.Web.dll assembly'ının referans edilmesi gerekmektedir.)

```csharp
using System;
using System.IO;
using System.Xml.Schema;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Formatters
{
    class Program
    {
        static void Main(string[] args)
        {
            #region DataContractJsonSerilazier ile Type Serileştirme

            // Serileştirilecek nesne örneği.
            Urun mouse = new Urun(1, "Mx Mouse Optic", 10, new DateTime(2004, 1, 1));

            // Serileştirici sınıf örneklenir.
            // Parametre olarak serileştirilecek veri sözleşmesi tipi verilir.
            DataContractJsonSerializer dcJsonSerializer = new DataContractJsonSerializer(typeof(Urun));

            // Serileştirme sonuçlarının yazılacağı örnek bir Stream oluşturulur.
            FileStream stream10 = new FileStream("UrunJson.xml", FileMode.Create, FileAccess.Write);
            // WriteObject metodu ile stream10 nesnesi ile tanımlanan Stream üzerine mouse isimli Urun nesne örneği verisi aktarılır
            dcJsonSerializer.WriteObject(stream10, mouse);
            stream10.Close(); // Stream kapatılır.

            #endregion

            #region DataContractJsonSerializer ile Type Ters-Serileştirme
        
            // UrunJson.xml dosyası var ise ters serileştirme işlemleri yapılır.
            if (File.Exists("UrunJson.xml"))
            {
                // DataContractJsonSerializer nesnesi örneklenir.
                DataContractJsonSerializer dcJsonDeSerializer = new DataContractJsonSerializer(typeof(Urun));
                // Serileştirilmiş veri içeriğinin bulunduğu dosyayı okumak için bir Stream örneklenir.
                FileStream stream11 = new FileStream("UrunJson.xml", FileMode.Open, FileAccess.Read);
                // ReadObject metodu ile parametre olarak verilen Stream içerisindeki bilgi ters serileştirme işlemine tabi tutulur.
                // Dönüş türü object olduğu için sonucun uygun türe cast edilmesi gereklidir.
                Urun urn = (Urun)dcJsonDeSerializer.ReadObject(stream11);
                // Elde edilen nesne örneği bilgisi ekrana yazdırılır.
                Console.WriteLine("Id :{0} Ad:{1} Fiyat:{2} Stok Tarihi:{3}", urn.Id.ToString(), urn.Ad, urn.ListeFiyati.ToString("C2"), urn.StokTarihi.ToString());
                // Stream kapatılır.
                stream11.Close();
            }

            #endregion
    
            #region DataContractJsonSerializer ile Array Serileştirme
        
            Urun[] urunler ={
                                    new Urun(2,"A Mouse Optic",11.2,DateTime.Now),
                                    new Urun(3,"Y Mouse Optic, Kablolu",9.49,DateTime.Now),
                                    new Urun(5,"Z Mouse",3.45,new DateTime(2008,2,3)),
                                    };

            DataContractJsonSerializer dcJsonArraySerializer = new DataContractJsonSerializer(typeof(Urun[]));
            FileStream stream12 = new FileStream("UrunlerJson.xml", FileMode.Create, FileAccess.Write);
            dcJsonArraySerializer.WriteObject(stream12, urunler);
            stream12.Close();
        
            #endregion

            #region DataContractJsonSerializer ile Array Ters-Serileştirme
        
            if (File.Exists("UrunlerJson.xml"))
            {
                DataContractJsonSerializer dcJsonDeSerializer = new DataContractJsonSerializer(typeof(Urun[]));
                FileStream stream13 = new FileStream("UrunlerJson.xml", FileMode.Open, FileAccess.Read);
                Urun[] gelenUrunler = (Urun[])dcJsonDeSerializer.ReadObject(stream13);
                Console.WriteLine("JSON Ürünler");
                foreach (Urun urun in gelenUrunler)
                {
                    Console.WriteLine("Id :{0} Ad:{1} Fiyat:{2} Stok Tarihi:{3}", urun.Id.ToString(), urun.Ad, urun.ListeFiyati.ToString("C2"), urun.StokTarihi.ToString());
                }
                stream13.Close();
            }

            #endregion
        }
    }
}
```

DataContractJsonSerializer sınıfına ait nesne örnekleri kullanılılırken yapıcı metoda, type bilgisi verilmektedir. DataContractSerializer tipine benzer olaraktan WriteObject ve ReadObject metodları ile stream üzerine serileştirme yapmak ve stream üzerinden serileştirilen bilgileri okumak mümkündür. Kodun çalışması sonrası üretilen çıktılar ise aşağıdaki gibidir.

UrunJson.xml içeriği;

![mk254_10.gif](images/mk254_10.gif)

UrunlerJson.xml içeriği;

![mk254_11.gif](images/mk254_11.gif)

JSON çıktısı, XML çıktısına göre daha az yer tutmaktadır. Bununla birlikte çıktının okunurluğu çok daha kolaydır. Dikkat edileceği üzere bilgiler key:value çiftleri şeklinde yazılmıştır. Buda özellikle javascript tarafında veriye erişimde büyük kolaylık sağlamaktadır.

XmlSerializer ile Serileştirme

.Net Framework 2.0 ile birlikte gelen tiplerden birisi olan XmlSerializer ile, serileştirme adımlarını özelleştirmek mümkündür. Diğer taraftan tüm Asp.Net Web Service modeli, XmlSerializer üzerine kurulmuştur. Bu nedenle Asp.Net Web Service uygulamalarının WCF tarafına aktarılmasında kolaylık sağlamaktadır. Diğer taraftan bazı vakalarda tiplerin kaynak kodlarına erişilemediği veya yeniden derleme işlemlerinin yapılamadığı durumlar bulunmaktadır. Bu durumlarda XmlSerializer tipinden faydalanılabilir. Nitekim bu vakalarda tiplere DataContract yada DataMember niteliklerinin uygulanması söz konusu değildir.

XmlSerializer ile temel olarak üç farklı modelde serileştirme işlemi gerçekleştirilebilir. İlk modele göre varsayılan yapıcı (Default Constructor) kullanılır ve tipin public olan özellik (Property) veya alanları (Field) serileşir. Diğer modelde serileşen üyelerin çıktılarının özelleştirilmesi amacıyla XmlElement, XmlAttribute gibi nitekiklerden yararlanılır. Son modelde ise IXmlSerializable arayüzü (Interface) implemantasyonu gerçekleştirilerek serileştirme işleminin özelleştirilmesi sağlanır.

> WCF tarafında tanımlı bir servis sözleşmesinin (Service Contract) çıktılarda XmlSerializer modelini kullanması için XmlSerializerFormat niteliğinden yararlanılması gerekmektedir.

Karar Vermek

Eğer serileştirme işlemi sırasında serileşen tipe ait özel işlemler yapılması isteniyorsa XmlSerializer tipinden yararlanılabilir. Bununla birlikte istemcilerin AJAX (AsynchronousJavascriptAndXml) veya Silverlight tabanlı olmaları halinde JSON formatında serileştirmeyi tercih etmekte yarar bulunmaktadır. NetDataContractFormatter tipi ne yazıkki her iki taraftada serileşecek tipe ait Assembly'ın olmasını ve özel kodlamalar yapılmasını gerektirdiğinden (Nitekim çalışma zamanına bu serileştiricinin kullanılacağının söylenmesi için özel attribute yazılması gerekmektedir) çok fazla tercih edilmemektedir. Bunların dışında kalan varsayılan durumlarda ise WCF zaten otomatik olarak DataContractFormatter tipinden yararlanmaktadır. Bu sebepten geliştiricinin sadece DataContract ve DataMember niteliklerini kullanması yeterlidir.

Versiyonlama (Versioning)

Buraya kadarki kısımda WCF tarafında ele alınabilecek olan serileştirici tiplerden kısaca bahsedilmiştir. Serileştirme sürecinde önem arz eden konulardan biriside versiyonlamadır. Nitekim eski istemcilerin yeni servis ile yada tam tersine, eski servislerin yeni istemciler ile çalışması gerektiği durumlar söz konusu olabilir. Servis ve istemci tarafının aynı veri sözleşmesine (Data Contract) sahip oldukları durumlarda versiyon farklılıklarına karşı hazırlıklı olmaları gerekebilir. WCF tarafında versiyonlama problemi aslında sessiz bir şekilde görmezden gelinmektedir. Ancak yinede veri sözleşmelerine ait versiyon farklılıkları olduğunda, vakanın nasıl ele alınması gerektiğinin bilinmesinde yarar vardır. Temel olarak üç farklı versiyonlama senaryosu bulunmaktadır.

Yeni Üyeler (New Members)

![mk254_12.gif](images/mk254_12.gif)

Bu senaryoda taraflardan birisi veri sözleşmesinin yeni versiyonunu karşı tarafa göndermektedir. Yeni üyelere sahip sözleşemeyi gönderen tarafın istemci veya servis olması farketmemektedir. Bu vakada alıcı taraf, gelen yeni üyeleri görmezden gelmektedir. Örneğin yukarıdaki şekilde taraflardan birisi Urun tipinin yeni bir versiyonunu karşı tarafa göndermektedir. Yeni versiyonda Id,Ad ve Fiyat alanlarına ek olarak Stok isimli yeni bir alan daha bulunmaktadır. Karşı tarafta ise bu alan görmezden gelinir. Yinede yeni versiyona ait veri içeriğinin tamamı karşı tarafa gönderilmekte ve ters serileştirme (Deserializing) işlemi sırasında gelen yeni üye içerikleri atlanılmaktadır (Ignore).

Kayıp Üyeler (Missing Members)

![mk254_13.gif](images/mk254_13.gif)

Özellikle eski veri sözleşmelerini kullanan istemciler ile servis tarafında yeni veri sözleşmesinin yer aldığı durumlarda söz konusudur. Burada servis tarafı istemciden gelen eksik üyelerin yer aldığı veri sözleşmesini ters serileştirme işlemine tabi tutarken varsayılan değerlerin atanmasını sağlamaktadır. Bir başka deyişle yine sessiz bir şekilde versiyon farkı görmezden gelinir ve eksik üyeler için ilk değerler atanır. Elbette OnDeserializing niteliği ile imzalanmış bir metoddan yararlanılarak eksik üyeler için farklı değerlerin verilmesi sağlanabilir. Tabi bu senaryoda ekstra bir durumda vardır. Bilindiği üzere DataMember niteliğinin IsRequired özelliği bulunmaktadır. Bu özelliğe göre ilgili üyeye ait değerin istemci tarafından gelmesi şarttır. Bu durum ilerleyen kısımlardaki örneklerde irdelenmektedir.

Round-Trip

![mk254_14.gif](images/mk254_14.gif)

Bu durumda istemci yeni üyelere sahip veri sözleşmesi içeriğini servis tarafına göndermektedir. Ancak servis tarafıda operasyon çağrısı sonucunda geriye aynı veri tipini döndürmektedir. Servis tarafı, ters serileştirme işlemi sırasında gelen fazla üyeyi kesip dışarıda bıraktığı için, operasyon sonucunda istemci tarafına eksik veri içeriği gönderilecektir. Diğer tarafan senaryo aşağıdaki şekildeki gibi olabilir.

![mk254_15.gif](images/mk254_15.gif)

Burada istemci tarafında veri sözleşmesinin yeni üyeler içeren bir versiyonu bulunmaktadır. Urun sınıfında yer alan Stok isimli özellik yeni üyedir. Ancak Service 1 tarafında Stok özelliğine sahip olmayan eski veri sözleşmesi kullanılmaktadır. Bu durumda Service 1, doğal olaraktan Stok özelliğini ve değerini görmezden gelecektir. Ne varki Service 1 kendisine gelen Urun nesne verisini kırptıktan sonra, son haliyle Service 2 tarafına göndermektedir. Oysaki Service 2 tarafında Urun tipine ait yeni veri sözleşmesi yer almaktadır. Bu durumdada Stok özelliğine varsayılan ilk değer atanır. Dolayısıyla istemcinin ilk etapta Stok değeri tamamen ortadan kaybolmaktadır. Burada istemci ve Service 1 arasında New Members versiyonlama koşulları; Service 1 ile Service 2 arasında ise Missing Members versiyonlama koşulları oluşmaktadır. Round-Trip senaryosunda, IExtensibleDataObject arayüzünden yararlanılarak durumun belirli ölçülerde kontrol altına alınmasıda sağlanabili ki ilerleyen örneklerde bu durumda incelenmektedir.

Böylece geldik WCF mimarisinde serileştirme, versiyonlama ve çözümleme ile ilgili ilk makalemizin sonuna. Devam eden makalemizde versiyonlama koşullarını örnekler üzerinden incelemeye çalışacağız. Özellikle Round-Trip hallerinde IExtensibleDataObject arayüzü kullanımını, IsRequired özelliğinin değerinin Missing Members vakasına olan etkisini, serileştirilen tipler için vekil (Surrogate) tip kullanımını ve çözümleyiciler (Encoders) arasında karar vermede dikkat edilmesi gereken hususları irdeliyor olacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/WCFSerializationAndEncoding.rar)
