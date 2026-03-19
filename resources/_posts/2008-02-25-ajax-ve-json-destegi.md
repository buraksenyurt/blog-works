---
layout: post
title: "AJAX ve JSON Desteği"
date: 2008-02-25 08:00:00 +0300
description: "Son yıllarda özellikle Web uygulamalarında AJAX(Asynchronous Javascript And XML) mimarisi oldukça yaygın bir şekilde kullanılmaktadır. Özellikle sunucu taraflı(Server-Side) çalışan Asp.Net gibi web uygulaması geliştirme modellerinde istemciler(Clients) tarafından sunucuya(Server) doğru gerçekleştirilen POST işlemleri sırasında, sayfanın tamamının gönderilmesi söz konusudur..."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - json
  - xml
  - dotnet
  - aspnet
  - soap
  - http
  - iis
  - authentication
  - javascript
  - serialization
  - visual-studio
  - dataset
---
# AJAX ve JSON Desteği
Değerli Okurlarım Merhabalar,

Son yıllarda özellikle Web uygulamalarında AJAX (Asynchronous Javascript And XML) mimarisi oldukça yaygın bir şekilde kullanılmaktadır. Özellikle sunucu taraflı (Server-Side) çalışan Asp.Net gibi web uygulaması geliştirme modellerinde istemciler (Clients) tarafından sunucuya (Server) doğru gerçekleştirilen POST işlemleri sırasında, sayfanın tamamının gönderilmesi söz konusudur. AJAX modeli sayesinde istemci tarafında yer alan sayfanın tüm içeriği yerine sadece değiştirilmesi istenen parçaların sunucuya gönderilmesi, işlenmesi ve cevapların alınarak tarayıcı uygulama (Browser Application) üzerinde gösterilmesi mümkün olmaktadır. Böylece sayfanın gerçektende değişmesi gereken içeriğinin istemci ve sunucu arasındaki hareketi söz konusudur. Bir başka deyişle gereksiz olan içeriğin sunucuya tekrar gönderilmesinin, işlenmesinin önüne geçilmesi sağlanmaktadır. Bu bir anlamda son kullanıcı (End User) için daha zengin etkileşime sahip ve performanslı bir web ortamı oluşturulması anlamına da gelir.

AJAX mimarisi, Asp.Net AJAX ile.NET platformu üzerinde çok daha kolay bir şekilde uygulanabilir hale getirilmiştir. Microsoft'un.Net Framework mimarisine getirdiği bu ilave yenilik hakkında söylenecek ve yazılacak çok şey vardır. Bu yazıdaki hedef ise Windows Communication Foundation servislerinin (WCF Services) AJAX bazlı istemcilere hizmete verebilecek şekilde nasıl geliştirileceklerinin öğrenilmesidir. Nitekim Web uygulamalarıda kendi içlerinde WCF servislerine erişebilir ve kullanabilirler. Bununla birlikte WCF mimarisi AJAX tipindeki istemcilere JSON (JavaScript and Object Notation) formatında veri içeriği sunabilme kapasitesine de sahip hale gelmiştir. İşe ilk olarak Asp.Net AJAX tipindeki istemcileri ele alarak başlamakta yarar vardır.

Asp.Net AJAX modeli aslında iki önemli parçadan oluşur. Bu parçalardan birisi istemci betik kütüphanelerdir (Client Script Libraries). Diğer parça ise sunucu taraflı betik kontrollerdir (Server Side Script Controls). Asp.Net AJAX sayfalarından bir WCF servisine ulaşmak son derece kolaydır. Bunun için öncelikli olarak servisin adres bilgisinin ScriptManager bloğu içerisinde belirtilmesi gerekir. Bu işlemden sonra istemci tarafından sanki bir JavaScript fonksiyonu çağırılıyormuş gibi WCF servisine ait operasyonlar (Service Operations) kullanılabilir. Elbette servis tarafından sunulan EndPoint noktasının AJAX tipinden istemcilere hizmet verecek şekilde ayarlanmış olması gerekir.

> AJAX istemcileri için WCF servisleri geliştirmek eğer Visual Studio 2008 gibi bir geliştirme ortamı kullanılıyorsa çok kolaydır. Nitekim bir Web uygulamasına Add New Item ile Ajax Enabled WCF Service şablonun eklenmesi yeterlidir.
>
> ![mk243_2.gif](/assets/images/2008/mk243_2.gif)

Ajax-enabled WCF Service seçeneğini kullanmadanda AJAX uyumlu WCF servisleri geliştirilebilir. Bunu yapmanın temel olarak iki farklı yolu vardır. Buna göre servisin kod tarafında yada konfigurasyon bazlı olacak şekilde geliştirilmesi mümkündür. Kod tarafında yapılan geliştirme çoğunlukla Dinamik Host Aktivasyon (Dynamic Host Activation) olarak bilinmektedir. Hangi model seçilirse seçilsin, AJAX istemcilere destek verecek EndPoint noktasına sahip olan WCF servisinin, IIS (Internet Information Services) üzerinde Host ediliyor olması şarttır.

Dynamic Host Activation modeline göre gelen talepleri değerlendirmek üzere WebServiceScriptHostFactory isimli bir CLR tipi (Common Language Runtime Type) devreye girmektedir. Bilindiği üzere IIS üzerinden yayınlanan WCF servislerinde svc uzantılı bir dosya bulunmaktadır. Söz konusu dosyada yer alan Service direktifi (Directive) ne ait Factory niteliğinde (Attribute) WebScriptServiceHostFactory ataması yapılarak, çalışma zamanında gelen taleplerin (Requests) dinamik olarak üretilen bir EndPoint tarafından karşılanması sağlanmaktadır. Bir başka deyişle AJAX istemcilerin gönderecekleri talepere göre EndPoint dinamik olarak uygun bir şekilde WebScriptServiceHostFactory tipi tarafından üretilir.

> WebScriptServiceFactory sınıfı, çalışma zamanında herhangibir konfigurasyon bilgisine ihtiyaç duymadan servise, Asp.Net Ajax EndPoint noktası eklenmesini sağlar. Bu sınıfın üretimi hem IIS (Internet Information Services) hemde (WAS) Windows Process Activation Services ortamları tarafından desteklenmektedir. WebScriptServiceFactory tipi, ServiceHostFactoryBase abstract sınıfından türeyen ServiceHostFactory sınıfından kalıtılmıştır.

Konuyu daha net kavrayabilmek adına basit bir örnek ile devam edilmesinde yarar vardır. Örnekte, IIS üzerinden Ajax istemcileri için Dynamic Host Activation mantığına uygun olacak şekilde bir servis geliştirilmektedir. Her zamanki gibi işe servis kütüphanesinin (WCF Service Library) tasarlanmasıyla başlanılmasında yarar vardır. Servis kütüphanesi içerisinde yer alacak olan tipler dışında Web üzerinde AJAX istemcilere hizmet verileceği için ilgili operasyonların WebGet yada WebInvoke nitelikleri (attribute) ile imzalanması gerekmektedir. Bu nedenle WCF servis kütüphanesinin System.ServiceModel.Web.dll isimli assembly referansına sahip olması gerekmektedir.

![mk243_3.gif](/assets/images/2008/mk243_3.gif)

Servis tarafı şimdilik geriye ilkel tip döndüren tek bir metoda sahiptir.

![mk243_1.gif](/assets/images/2008/mk243_1.gif)

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace OrtakIslemler
{
    [ServiceContract(Namespace="OrtakServis")]
    public interface IMatematik
    {
        [OperationContract]
        [WebGet]
        double DaireAlan(double r);
    }

    public class Matematik : IMatematik
    {
        #region IMatematik Members

        public double DaireAlan(double r)
        {
            return Math.PI * r * r;
        }

        #endregion
    }
}
```

IMatematik isimli servis sözleşme arayüzü (Interface) geriye double değer döndüren ve double tipinden parametre alan DaireAlan isimli bir operasyon tanımlamaktadır. İlgili operasyonun WebGet niteliği ile imzalandığına dikkat edilmelidir. Servis sözleşmesini uygulayan Matematik isimli sınıf ise DaireAlan metodunu uygulamaktadır. Servis uygulaması IIS üzerinden yayınlama yapması gerektiğinden Visual Studio 2008 ortamında Add New Web Site ile WCF Service şablonundan bir proje oluşturulabilir. Söz konusu uygulamada şu an için web.config dosyasının olmasına gerek yoktur. Nitekim ilk amaç Dynamic Host Activation modelini uygulamaktır. Diğer taraftan servis kütüphanesinin ilgili web uygulamasına referans edilmesi gerekmektedir. Sonuç itibariyle AjaxServiceDemo isimli WCF Service uygulamasının Solution Explorer üzerinden görülen ilk hali aşağıdaki gibi olmalıdır.

![mk243_4.gif](/assets/images/2008/mk243_4.gif)

Artık Service.svc dosyasının içeriği aşağıdaki gibi geliştirilebilir.

```text
<%@ ServiceHost Language="C#" Debug="true" Service="OrtakIslemler.Matematik" Factory=System.ServiceModel.Activation.WebScriptServiceHostFactory %>
```

Service niteliğinde standart olarak servis nesnesine ait tipin bildirimi yapılmaktadır. Bununla birlikte Ajax istemcilerine hizmet verilmesini sağlayabilmek içinde Factory niteliğine System.ServiceModel.Activation.WebScriptServiceHostFactory bilgisi atanmıştır. Bu noktadan sonra svc dosyası herhangibir tarayıcı penceresinden talep edilebilir. Yanlız dikkat edilemesi gereken bir nokta vardır. Yazının hazırlandığın tarih itibariyle Vista işletim sistemi üzerinde yer alan IIS 7.0 sürümünde aşağıdaki ekran görüntüsünde yer alan hata mesajı ile karşılaşılmaktadır.

![mk243_5.gif](/assets/images/2008/mk243_5.gif)

Bu sorunun şimdilik çözümü için IIS 7.0 üzerinde ilgili WCF servisine ait Authentication bilgisinden sadece tek bir modelin seçili olmasını sağlamaktır. Nitekim ilgili WCF servisi için hem Anonymous hemde Windows Authentication modları aktiftir. Örnekte sadece Anonymous modun seçil halde bırakılması sağlanılmalıdır.

![mk243_6.gif](/assets/images/2008/mk243_6.gif)

Bu düzeltmeden sonra service.svc yeniden talep edilirse aşağıdaki ekran görüntüsü elde edilir.

![mk243_7.gif](/assets/images/2008/mk243_7.gif)

Görüldüğü gibi standart olarak bir WCF servisine HTTP üzerinden yapılan talep sonrası karşılaşılan ekran üretilmiştir. Elbette burada herhangibir şekilde konfigurasyon ayarı yapılmadığından yada ilgili nitelikler ile servise veya EndPoint noktasına bir davranış (Behavior) belirtilmediğinden HTTP üzerinden metadata bilgisi çekilmesine izin verilmemektedir.

> İster Dynamic Host Activation modeli ister konfigurasyon bazlı modele göre AJAX istemcilere hizmet verecek şekilde tasarlanmış olsun, bir WCF servisi ek EndPoint noktaları içerebilir. Söz gelimi hem AJAX istemcilere hizmet veren hemde SOAP protokolü üzerinden hizmet veren EndPoint noktalarına sahip bir WCF servisinin tasarlanması mümkündür.

Artık AJAX uyumlu istemcinin yazılmasına başlanabilir. Söz konusu istemci Visual Studio 2008 ortamında geliştirilen bir Asp.Net uygulamasıdır. Diğer taraftan.Net Framework 3.5 şablonu seçileceği için doğrudan AJAX desteğide otomatik olarak gelmektedir. Sayfanın en önemli olan noktalarından birisi ScriptManager içeriğidir. Nitekim ScriptManager elementi içerisinde kullanılmak istenen servisin adresi mutlaka belirtilmelidir. Örnek aspx sayfasının içeriği aşağıdaki gibidir.

![mk243_8.gif](/assets/images/2008/mk243_8.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script type="text/javascript">

    function ServisCagir()
    {
        var proxy = new OrtakServis.IMatematik(); 
        var r=parseFloat(document.getElementById("txtYaricap").value); 
        proxy.DaireAlan(r,Basarili,Basarisiz,null);
    }

    function Basarili(sonuc)
    { 
        document.getElementById("lblSonuc").value=sonuc;
    }
    function Basarisiz()
    {
        document.getElementById("lblSonuc").value="Bir sorun var.";
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        
                <asp:ScriptManager ID="ScriptManager1" runat="server">
                    <Services>
                        <asp:ServiceReference Path="http://localhost/AjaxServiceDemo/Service.svc" />
                    </Services>
                </asp:ScriptManager>
    
                <br />Daire Yarıçapı :<asp:TextBox ID="txtYaricap" runat="server"></asp:TextBox><br />
                <input type="button" id="btnHesapla" value="Daire Alanı Bul" onclick="ServisCagir()" /><br />
                <br />
                <input type="text" id="lblSonuc" />
        </div>
    </form>
</body>
</html>
```

ScriptManager kontrolü kendi içerisinde birden fazla servis noktası tanımlanabilmesini sağlayacak şekilde Services isimli bir alt elemente sahiptir. Bu element içerisinde ServiceReference isimli bileşenler kullanılır. ServiceReference bileşeninin Path özelliğine verilen değer kullanılmak istenen WCF servisine ait URL bilgisini taşımaktadır. Diğer taraftan javascript kodlarında dikkat edileceği üzere btnHesapla isimli input kontrolüne basıldığında çalışan ServisCagir isimli bir metod yer almaktadır. Bu metod içerisinde önce bir proxy nesnesi oluşturulmaktadır. Bu nesne oluşturulurken OrtakServis.IMatematik isimli tipten yararlanılmaktadır. OrtakServis adı, ServiceContract niteliğinde belirtilen Namespace değerinden gelmektedir. Diğer taraftan IMatematik ismi ise, servis sözleşmesinin (Service Contract) adıdır. Buna göre üretilen proxy nesnesi üzerinden DaireAlan metodu çağırılarbilir.

Metod çağrısı gerçekleştirilirken ilk parametre olarak, DaireAlan metodunun beklediği yarıçap değeri verilmektedir. İkinci parametre söz konusu operasyon başarılı bir şekilde tamamlanırsa devreye girecek olan ve sonuçların alınabileceği geri bildirim fonksiyonunun adıdır. Bu metod içerisinde çağırılan servis operasyonunun sonucu alınması gerektiğinden ilgili metod, DaireAlan operasyonunun dönüş değerini parametre olarak almaktadır. Üçüncü parametre ile belirtilen metod ise, servis üzerindeki operasyonun çağırılması esnasında bir hata oluşması halinde devreye girecek olan metoddur. Son parametre null olarak geçilmekle birlikte çoğunlukla HttpContext tipinin kullanılması gerektiği durumlarda ele alınmaktadır. Artık istemci uygulama çalıştırılıp test edilebilir. Eğer düğme tıklanırsa sayfanın tamamının Post edilmeden, sadece servis operasyonun çağırıldığı ve sonucun ekrandaki ilgili TextBox kontrolüne alındığı görülebilir.

![mk243_9.gif](/assets/images/2008/mk243_9.gif)

Şimdi WCF servisinin Dynamic Host Activation yerine konfigurasyon bazlı olarak nasıl inşa edileceğine bakılabilir. Konfigurasyon dosyası kullanıldığında WebHttpBinding bağlayıcı tipinin (Binding Type) ve EnableWebScript davranışının (Behavior) kullanılması gerekir. Söz gelimi az önce geliştirilen WCF servis uygulamasının konfigurasyon tabanlı olacak şekilde çalıştırılması için web.config dosyasında yer alan system.serviceModel elementinin içeriğinin aşağıdaki gibi tasarlanması yeterlidir.

```text
<system.serviceModel>
    <behaviors>
        <endpointBehaviors>
            <behavior name="AjaxEndPointBehavior">
                <enableWebScript />
            </behavior>
        </endpointBehaviors>
    </behaviors>
    <services>
        <service name="OrtakIslemler.Matematik">
            <endpoint address="http://localhost/AjaxServiceDemo/Service.svc" behaviorConfiguration="AjaxEndPointBehavior" binding="webHttpBinding" name="AjaxEndPoint" contract="OrtakIslemler.IMatematik" />
        </service>
    </services>
</system.serviceModel>
```

Dikkat edileceği üzere AjaxEndPoint isimli EndPoint tanımlanırken webHttpBinding bağlayıcı tipinin (Binding Type) kullanılacağı belirtilmiştir. Ayrıca EndPoint davranışı içerisinde enableWebScript elementi kullanılmıştır. Bu durumda istemci uygulama test edildiğinde yine WCF servisinin başarılı bir şekilde çalıştığı görülebilir.

AJAX uyumlu WCF servislerinde önem arz eden konulardan biriside, istemcilerin servis operasyonlarına yaptıkları talep sonrası dönecek olan verinin formatıdır. AJAX destekli WCF servisleri XML (eXtensible Markup Language) tipinde veri formatını kullanmakla birlikte.Net Framework 3.5 ile JSON (JavaScript Object Notation) desteğinede sahip olmuşlardır ki çoğu AJAX servisi varsayılan olarak JSON bazlı yayınlama yapmaktadır. Daha önceki bölümlerden de hatırlanacağı gibi, bir servis operasyonuna uygulanan WebGet ve WebInvoke niteliklerine (attribute) ait ResponseFormat özelliklerinin değerleri kullanılarak JSON formatında veri dönüştürüleceği belirtilebilmektedir. Diğer taraftan WCF AJAX EndPoint noktaları hem JSON hemde XML formatındaki taleplere cevap verebilmektedir. application/json tipindeki talepler (requests) tahmin edileceği üzere JSON formatı ile alakalıdır. Bununla birlikte text/xml formatındaki taleplerde XML formatı ile alakalıdır. Tabi Ajax destekli servislerde sorun oluşturan önemli noktalardan biriside,.NET tiplerinin JSON formatına serileştirme (Serialization) işlemleridir. Bu nedenle ilk olarak JSON serileştirmesine bir göz atmakta yarar vardır.

> JSON (JavaScript Object Notation) özellikle AJAX uyumlu servisler (Ajax Enabled WCF Services/Web Services) ile istemciler arasında hızlı veri değiş tokuşu yapılmasına olanak tanıyan önemli veri formatı (Data Format) standartlarındadır.

JSON serileştirmede de servis tarafından istemciye yayınlanan veri tiplerinin önemi büyüktür. Nitekim WCF gibi.Net tabanlı bir ortamda, CLR tiplerinin JSON karşılıklarının bilinmesinde yarar vardır. Bu amaçla öncelikli olarak aşağıdaki tablonun göz önüne alınması yararlı olabilir. Hemen hemen tüm CLR tipleri (Common Language Runtime Types) uygun JSON tiplerine dönüştürülmektedir.

.Net Tipi
JSON Karşılığı

Int16, Int32, Double, Decimal gibi sayısal tiplerin tamamı.
Number

Boolean
Boolean

String, Char
String

Timespan, Guid, Uri
String

XmlElement, XmlNode gibi Xml tipleri
String

ISerializable, DataSet gibi tipler
String

Enum
Number

Byte Dizisi
Number Dizisi

DateTime
DateTime veya String

Collections (Koleksiyonlar), Dictionary Tipleri ve Arrays (Diziler)
Array

Herhangibir tipin Null değeri
Null

DataContract niteliğini (Attribute) uygulamış tipler
Complex Type

ISerializable arayüzünü (Interface) uygulamış tipler
Complex Type

DBNull
Empty Complex Type

XmlQualifiedName
String

Buradaki tablo.Net ve JSON tipleri arasındaki eşleştirmelerin basit bir özetidir. Tip dönüşümleri sırasında göz önüne alınması gereken oldukça fazla kural ve vaka bulunmaktadır. (Söz konusu durumlar makalemizin konusunu aşmaktadır. Ancak detaylı bilgi için [MSDN](http://msdn2.microsoft.com/en-us/library/bb412170.aspx) kaynaklarındaki ilgili bağlantıya bakılabilir.)

Normal şartlarda WCF servisleri JSON serileştirmesini otomatik olarak gerçekleştirmektedir. Özellikle kullanıcı tanımlı tiplerde (User Defined Types) veri sözleşmesi tanımlanarak (DataContract ve DataMember nitelikleri-attribute yardımıyla) serileştirme işlemi otomatik hale getirilmektedir. Yinede bazı durumlarda JSON serileştirilme (Serialization) ve ters-Serileştirme (DeSerialization) işlemlerinin elle yapılması gerekebilir. Bu amaçla.Net Framework 3.5 ile birlikte gelen DataContractJsonSerializer tipinden yararlanılabilir.

![mk243_10.gif](/assets/images/2008/mk243_10.gif)

System.ServiceModel.Web.dll assembly'ındaki System.Runtime.Serializetion.Json isim alanında (Namespace) bulunan DataContractJsonSerializer, XmlObjectSerializer abstract sınıfından türemiş selaed olarak imzalanmış bir CLR tipidir. Sealed olarak işaretlenmiş olması nedeniyle kendisinden türetme (Inherit) yapılamamaktadır. Temel görevi tipleri (Types) JSON formatında veri olarak serileştirmek veya tam tersini yapmaktır.

JSON serileştirmeyi daha iyi kavramak için bir örnek ile devam edilmesinde yarar vardır. Bu amaçla Visual Studio 2008 ortamında.Net Framework 3.5 tabanlı bir Console uygulaması geliştirildiği göz önüne alınsın. Öncelikli olarak System.ServiceModel.Web.dll ' inin projeye refarans edilmiş olması gerekmektedir. Ayrıca DataContract ve DataMember niteliklerini kullanabilmek içinde System.Runtime.Serialization.dll assembly'ının referans edilmesi şarttır.

![mk243_12.gif](/assets/images/2008/mk243_12.gif)

Örnek olarak class diagram görüntüsü aşağıdaki gibi olan Urun isimli bir sınıf (Class) tasarlanabilir.

![mk243_11.gif](/assets/images/2008/mk243_11.gif)

```csharp
using System;
using System.Runtime.Serialization;

namespace JSONSerilestirme
{
    [DataContract]
    class Urun
    {
        private int _id;
        private string _ad;
        private double _fiyat;
        private DateTime _stokGirisTarihi;

        [DataMember]
        public DateTime StokGirisTarihi
        {
            get { return _stokGirisTarihi; }
            set { _stokGirisTarihi = value; }
        }

        [DataMember]
        public double Fiyat
        {
            get { return _fiyat; }
            set { _fiyat = value; }
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

        public Urun(int id, string ad, double fiyat, DateTime stokGirisTarihi)
        {
            Id = id;
            Ad = ad;
            Fiyat = fiyat;
            StokGirisTarihi = stokGirisTarihi;
        }
    }
}
```

Urun sınıfı DataContract niteliği (Attribute) ile imzalanmıştır. Bununla birlikte serileştirmeye tabi tutulacak olan Id,Ad,Fiyat,StokGirisTarihi özellikleride (Properties) DataMember nitelikleri ile işaretlenmişlerdir. Serileştirme ve ters-Serileştirme için örnek olarak aşağıdaki gibi bir kod parçası göz önüne alınabilir.

```csharp
using System;
using System.Runtime.Serialization.Json;
using System.IO;

namespace JSONSerilestirme
{
    class Program
    {
        static void Main(string[] args)
        {
            // Urun sınıfına ait bir nesne örneklenir.
            Urun dvd = new Urun(1, "Double Layer DVD Box 150", 30, DateTime.Now);

            #region Kullanıcı tanımlı bir tipi JSON Serileştirme
    
            // Json serileştirme işlemleri için DataContractSerializer tipi örneklenir. Örnekleme işlemi sırasında parametre olarak serileştirilecek olan tip belirtilir.
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Urun));
            // Serileştirme örnek olarak bir dosyaya doğru yapılacaktır.
            using (FileStream stream = new FileStream("UrunJason.xml", FileMode.Create, FileAccess.Write))
            {
                // WriteObject metodu ile ilk parametrede belirtilen stream üzerine, ikinci parametrede belirtilen nesne örneğinin verisi serileştirilir
                serializer.WriteObject(stream, dvd);
            }

            #endregion
    
            #region JSON datasını DeSerialize edip Object haline getirme
    
            // Dosyadaki veriden nesne elde edileceği için FileStream oluşturulur.
            using (FileStream stream = new FileStream("UrunJason.xml", FileMode.Open, FileAccess.Read))
            {
                // ReadObject metodu ile stream ile belirtilen dosya içerisindeki JSON formatlı veri okunur Object olarak elde edilir. Sonrasında ise kullanılabilmesi için Urun tipine cast edilir.
                Urun okunanDvd = (Urun)serializer.ReadObject(stream);
                Console.WriteLine(okunanDvd.Id+" "+okunanDvd.Ad+" "+okunanDvd.Fiyat.ToString("C2")+" "+okunanDvd.StokGirisTarihi.ToString());
            }

            #endregion
        }
    }
}
```

Serileştirme işlemi sonrasında oluşan UrunJason.xml dosyasının içeriği aşağıdaki gibi olmaktadır.

```json
{"Ad":"Double Layer DVD Box 150","Fiyat":30,"Id":1,"StokGirisTarihi":"\/Date(1203765080248+0200)\/"}
```

Ayrıca ters serileştirme (DeSerializetion) işlemi sonrasında ise programın ekran çıktısı aşağıdaki gibidir. Görüldüğü gibi dosyada duran JSON formatlı veri içeriğinden Urun nesne örneği elde edilmiştir.

![mk243_13.gif](/assets/images/2008/mk243_13.gif)

Elbette Urun tipinden nesne örneklerini bünyesinde barındıran bir dizide JSON formatında serileştirilebilir ve hatta okunabilir. Aşağıdaki kod parçasında bu durum örneklenmeye çalışılmaktadır.

```csharp
Urun[] urunler ={
                        new Urun(2,"Urun X",1.45,new DateTime(2007,12,2))
                        ,new Urun(3,"Urun Y",2.34,new DateTime(2008,2,3))
                        ,new Urun(4,"Z Urun",34.56,new DateTime(2006,6,7))
                        };

DataContractJsonSerializer arraySerializer = new DataContractJsonSerializer(typeof(Urun[]));
using (FileStream stream = new FileStream("Urunler.json", FileMode.Create, FileAccess.Write))
{
    arraySerializer.WriteObject(stream, urunler);
}

using (FileStream stream = new FileStream("Urunler.json", FileMode.Open, FileAccess.Read))
{
    Urun[] gelenUrunler=(Urun[])arraySerializer.ReadObject(stream);
    foreach(Urun urn in gelenUrunler)
        Console.WriteLine(urn.Ad);
}
```

Bu kez serileştirme işleminde Urun tipinden bir dizi (Array) söz konusudur. Bu sebepten DataContractJsonSerializer sınıfına ait nesne örneklenirken parametre olarak typeof (Urun[]) bilgisi verilmektedir. Sonuç olarak üretilen JSON formatlı veri içeriği aşağıdaki gibi olacaktır.

```text
[{"Ad":"Urun X","Fiyat":1.45,"Id":2,"StokGirisTarihi":"\/Date(1196546400000+0200)\/"},{"Ad":"Urun Y","Fiyat":2.34,"Id":3,"StokGirisTarihi":"\/Date(1201989600000+0200)\/"},{"Ad":"Z Urun","Fiyat":34.56,"Id":4,"StokGirisTarihi":"\/Date(1149627600000+0300)\/"}]
```

Hemen yeni bir kod parçası ile bir DataSet nesne örneğinin JSON formatında nasıl serileşeceğine bakmakta yarar vardır. Nitekim veri tabanı uygulamalarının çok sık kullanılması nedeni ile WCF servislerinden özellikle bağlantısız katmana (Disconnected Layer) ait tiplerin döndürülmesi sık rastlanan bir durumdur. Bu amaçla Console uygulamasına aşağıdaki kod parçasının eklenmesi yeterlidir.

```csharp
using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
{
    SqlDataAdapter adapter = new SqlDataAdapter("Select Top 5 ProductId,Name,ListPrice From Production.Product", conn);
    DataSet set = new DataSet();
    adapter.Fill(set);

    DataContractJsonSerializer dataSetSerializer = new DataContractJsonSerializer(typeof(DataSet));
    using (FileStream stream = new FileStream("Products.json", FileMode.Create, FileAccess.Write))
    {
        dataSetSerializer.WriteObject(stream, set);
    }
}
```

Bu kodun çalışma sonrasın oluşan Products.json dosyasının içeriği aşağıdaki gibi olur.

```text
"<DataSet><xs:schema id=\"NewDataSet\" xmlns:xs=\"http:\/\/www.w3.org\/2001\/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\"> <xs:element name=\"NewDataSet\" msdata:IsDataSet=\"true\" msdata:UseCurrentLocale=\"true\"> <xs:complexType> <xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:element name=\"Table\"> <xs:complexType><xs:sequence><xs:element name=\"ProductId\" type=\"xs:int\" minOccurs=\"0\"\/> <xs:element name=\"Name\" type=\"xs:string\" minOccurs=\"0\"\/><xs:element name=\"ListPrice\" type=\"xs:decimal\" minOccurs=\"0\"\/><\/xs:sequence> <\/xs:complexType><\/xs:element><\/xs:choice> <\/xs:complexType> <\/xs:element><\/xs:schema><diffgr:diffgram xmlns:diffgr=\"urn:schemas-microsoft-com:xml-diffgram-v1\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\"><NewDataSet> <Table diffgr:id=\"Table1\" msdata:rowOrder=\"0\"><ProductId>1<\/ProductId> <Name>Adjustable Race<\/Name><ListPrice>0.0000<\/ListPrice><\/Table><Table diffgr:id=\"Table2\" msdata:rowOrder=\"1\"><ProductId>2<\/ProductId> <Name>Bearing Ball<\/Name><ListPrice>0.0000<\/ListPrice><\/Table><Table diffgr:id=\"Table3\" msdata:rowOrder=\"2\"><ProductId>3<\/ProductId><Name>BB Ball Bearing<\/Name><ListPrice>0.0000<\/ListPrice><\/Table><Table diffgr:id=\"Table4\" msdata:rowOrder=\"3\"><ProductId>4<\/ProductId><Name>Headset Ball Bearings<\/Name><ListPrice>0.0000<\/ListPrice><\/Table><Table diffgr:id=\"Table5\" msdata:rowOrder=\"4\"><ProductId>316<\/ProductId><Name>Blade<\/Name> <ListPrice>0.0000<\/ListPrice><\/Table><\/NewDataSet><\/diffgr:diffgram><\/DataSet>"
```

Daha öncedende belirtildiği gibi DataSet tipinin JSON karşılığı String olarak ifade edilmektedir. Bu nedenle üretilen çıktı çift tırnaklar içerisinde yer almaktadır.

Bu bölümde son olarak kullanıcı tanımlı bir tipin (User Defined Type) WCF servisinden JSON formatında döndürüldüğü ve AJAX uyumlu bir istemci tarafından ele alındığı örnek geliştirilmeye çalışılmaktadır. Burada servis operasyonunun JSON formatında cevap (Response) vermesi ve POST metoduna göre çalışması için WebInvoke niteliği (attribute) ile imzalanmış olması gerekmektedir. İstemci tarafında ise gelen cevabın ayrıştırılması ve kullanılması ele alınmaktadır. İlk olarak servis tarafında kullanılacak olan standart WCF servis kütüphanesi geliştirilerek işe başlanabilir. Örnek kütüphane içerisinde yer alacak tipler aşağıdaki gibidir.

![mk243_14.gif](/assets/images/2008/mk243_14.gif)

Servis sözleşmesi (Service Contract), uygulayıcı sınıf ve veri sözleşmesi (Data Contract) tiplerin içeriği ise aşağıdaki gibidir.

Urun Sınıf;

```csharp
using System;
using System.Runtime.Serialization;

namespace VeriServisKutuphanesi
{
    [DataContract]
    public class Urun
    {
        [DataMember]
        public int Id;
        [DataMember]
        public string Ad;
        [DataMember]
        public double Fiyat;
    }
}
```

Servis Sözleşmesi ve Uygulayıcı Tip;

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Data.SqlClient;

namespace VeriServisKutuphanesi
{
    [ServiceContract(Namespace="AdventureVeriHizmeti")]
    public interface IVeriIslemleri
    {
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        Urun UrunBul(int urunId); 
    }

    public class VeriIslemleri
        : IVeriIslemleri
    {
        #region IVeriIslemleri Members

        public Urun UrunBul(int urunId)
        {
            Urun u = null;
            using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
            {
                SqlCommand cmd = new SqlCommand("Select ProductID,Name,ListPrice From Production.Product Where ProductID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", urunId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    u = new Urun()
                                        {
                                            Id=Convert.ToInt32(reader["ProductID"]),
                                            Ad=reader["Name"].ToString(),
                                            Fiyat=Convert.ToDouble(reader["ListPrice"])
                                        };
                }
            }
            return u;
        }
        #endregion
    }
}
```

UrunBul isimli metoda WebInvoke niteliği uygulanmıştır. Dikkat edileceği üzere ResponseFormat özelliğine WebMessageFormat.Json değeri atanmıştır. Bu atama, metodun çıktısının istemcilere JSON formatında gönderileceğini belirtmektedir. UrunBul isimli metod parametre olarak aldığı urunId değerine göre Product tablosundan çektiği satırı baz alarak Urun tipinde bir nesne örneği oluşturup döndürmektedir. Servis tarafı yine IIS (Internet Information Services) üzerinde konuşlandırılmış olarak tasarlanmalıdır. Bu amaçla WCF Service şablonunda bir uygulama açılarak devam edilebilir. Söz konusu uygulama çok doğal olarak VeriServisKutuphanesi.dll'ini referans etmelidir. Bununla birlikte Service.svc dosyasının içeriği aşağıdaki gibi tasarlanabilir.

```text
<%@ ServiceHost Language="C#" Debug="true" Service="VeriServisKutuphanesi.VeriIslemleri" %>
```

Örnekte AJAX uyumlu EndPoint noktası Web.config dosyası içerisinde tanımlanmaktadır. Web.config dosyasında yer alan serviceModel elementinin içeriği aşağıdaki gibidir.

```xml
<system.serviceModel>
    <behaviors>
        <endpointBehaviors>
            <behavior name="AjaxEndPointBehavior">
                <enableWebScript />
            </behavior>
        </endpointBehaviors>
    </behaviors>
    <services>
        <service name="VeriServisKutuphanesi.VeriIslemleri">
            <endpoint address="http://localhost/AjaxServiceDemo2/Service.svc" behaviorConfiguration="AjaxEndPointBehavior" binding="webHttpBinding" bindingConfiguration="" name="AjaxEndPoint" contract="VeriServisKutuphanesi.IVeriIslemleri" />
        </service>
    </services>
</system.serviceModel>
```

Gelelim istemci tarafındaki uygulamamıza. Bu sefer ilk örnekten farklı olarak ScriptManager kullanmadan WCF Servis operasyonunu çağırılmaktadır. Bu amaçla geliştirien Asp.Net Web uygulamasında yer alacak olan aspx sayfasının içeriği aşağıdaki gibi geliştirilebilir.

![mk243_15.gif](/assets/images/2008/mk243_15.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script type="text/javascript">

    function Getir()
    {
        var productId = document.getElementById("txtProductId").value;

        if(productId)
        {
            var xmlHttp;
            try 
            {
                xmlHttp = new XMLHttpRequest();
            } 
            catch (e) 
            {
                try 
                {
                    xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
                } 
                catch (e) 
                {
                    try 
                    {
                        xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                    } 
                    catch (e) 
                    {
                        alert("Tarayıcıda AJAX desteği yok."); 
                        return false;
                    }
                }
            }

            xmlHttp.onreadystatechange=function()
                {
                    if(xmlHttp.readyState == 4)
                    {
                        var sonuc = eval("(" + xmlHttp.responseText + " )").d;
                        document.getElementById("txtId").value = sonuc.Id;
                        document.getElementById("txtName").value = sonuc.Ad;
                        document.getElementById("txtListPrice").value = sonuc.Fiyat;
                    }
                }

            var url = "http://localhost/AjaxServiceDemo2/service.svc/UrunBul";

            var mesajGovde = '{"urunId":'+ document.getElementById("txtProductId").value + '}';

            xmlHttp.open("POST", url, true);
            xmlHttp.setRequestHeader("Content-type", "application/json");
            xmlHttp.send(mesajGovde);
        }
    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Ürün Numarası : <asp:TextBox ID="txtProductId" runat="server" />
        <input type="button" onclick="return Getir();" value="Getir" />
        <br />
        <br />
        <table>
            <tr>
                <td>Urun Id</td>
                <td><input id="txtId" type="text" /></td>
            </tr>
            <tr>
                <td>Ad</td>
                <td><input id="txtName" type="text" /></td>
            </tr>
            <tr>
                <td>Fiyat</td>
                <td><input id="txtListPrice" type="text" /></td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
```

İstemci tarafında klasik olarak AJAX uyumlu Javascript kodları yer almaktadır. Önemli olan noktalardan bir tanesi, xmlHttp isimli nesnesinin örneklenmesinden sonra open metodu ile WCF servisine doğru yapılan çağrıdır. Dikkat edilecek olursa WebInvoke niteliği nedeni ile http://localhost/AjaxServiceDemo2/Service.svc/UrunBul isimli bir URL bilgisi kullanılmaktadır.

Diğer taraftan istemciden servis operasyonuna doğru gönderilecek olan talebin (Request) başlık kısmının içeriğinin JSON olacağı setRequestHeader metodu ile belirlenmektedir. Sonrasında send metodu ile ilgili paket WCF servisine gönderilmektedir. İşlem tamamlandığında devreye giren fonksiyon içerisinde responseText özelliğinden de yararlanılarak dönen cevap alınmakta ve sayfa üzerindeki ilgili bileşenlere gelen değerler aktarılmaktadır. Burada d isimli özellik yardımıyla aslında Urun tipinin verisine ulaşılabilmektedir. Bunu daha iyi görebilmek için ilgili kodlar debug edilerek QuickWatch çıktısına bakılabilir.

![mk243_16.gif](/assets/images/2008/mk243_16.gif)

Dikkat edilecek olursa dönen cevap içerisinde Urun nesne örneği d isimli bir değişken olarak gelmektedir. Bu değişken üzerinden Ad, Fiyat ve Id isimli alanlarada erişilebilmektedir. Çalışma zamanında (Runtime) F11 ile step into modunda hareket edildiğinde istemci tarafına aşağıdaki ekran görüntüsünde yer alan bir içeriğin geldiği görülür.

![mk243_17.gif](/assets/images/2008/mk243_17.gif)

İşte bu bilgiden yararlanılarak geliştirilen istemcide Getir başlıklı düğmeye basıldığında aşağıdaki ekran görüntüsü elde edilecektir.

![mk243_18.gif](/assets/images/2008/mk243_18.gif)

Elbette burada gözden kaçırılmaması gereken bazı noktalar vardır. Öncelikli olarak veri içeriğin istemciye Null gelme olasılığı vardır. Bu da çok doğal olarak çalışma zamanı hatalarının oluşması anlamına gelmektedir. Bu gibi noktalar elbetteki gerçek bir uygulamada mutlaka ele alınmalıdır.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kısaca WCF servis uygulamalarının AJAX destekli olacak şekilde nasıl geliştirilebildiklerini incelemeye çalıştık. Burada önemli olan JSON veri formatı ile ilişkin olaraktanda.Net Framework 3.5 ile gelen DataContractJsonSerializer tipini inceleme fırsatı bulduk. Son olarakta JSON formatında içerik sunan bir WCF servisinin AJAX tabanlı bir istemci ile nasıl çağırılabileceğini inceledik. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/AjaxDestegi.rar)