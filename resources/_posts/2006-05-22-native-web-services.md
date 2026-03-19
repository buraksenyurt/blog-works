---
layout: post
title: "Native Web Services"
date: 2006-05-22 12:00:00 +0300
description: "Sql Server 2005 ile gelen etkili özelliklerden biriside, doğal xml web servisi (native xml web services) desteğinin bulunmasıdır. Sql Server 2000 sürümünde, web servisi desteğini sunabilmek için SqlXml 3.0 ve IIS' in sunucu sistemde mutlaka yüklü olması gerekmektedir. Bununla birlikte istemciler MDAC desteğine sahip olmalıdır..."
categories:
  - xml-web-services
tags:
  - xml-web-services
  - csharp
  - dotnet
  - sql-server
  - xml
  - soap
  - web-service
  - http
  - iis
  - authentication
  - dataset
  - datatable
---
# Native Web Services
Değerli Okurlarım Merhabalar,

Sql Server 2005 ile gelen etkili özelliklerden biriside, doğal xml web servisi (native xml web services) desteğinin bulunmasıdır. Sql Server 2000 sürümünde, web servisi desteğini sunabilmek için SqlXml 3.0 ve IIS'in sunucu sistemde mutlaka yüklü olması gerekmektedir. Bununla birlikte istemciler MDAC desteğine sahip olmalıdır. Oysaki Sql Server 2005 istemcilerden Http protokolüne göre gelecek SOAP uyumlu talepleri doğrudan işletebilecek şekilde tasarlanmıştır. Sistemde yer alan Http dinleyici (Http Listener), istemcilerden gelecek olan talepleri doğrudan Sql Server 2005 üzerindeki EndPoint'lere iletmektedir. Dolayısıyla sunucu sistem üzerine IIS bulunma zorunluluğu ortadan kaldırılmıştır.

Sql Server 2005, saklı yordamların (stored procedures) ve kullanıcı tanımlı fonksiyonların (User-defined Functions) http üzerinden SOAP uyumlu taleplere cevap verebilecek halde sunulmasını sağlar. Bunun içinde Http veya Tcp protokolüne göre çalışan EndPoint nesnelerinden yararlanır. Sql Server 2005, http üzerinden gelecek olan talepleri yine SOAP mesajları şeklinde cevaplandırdığından, istemci her hangibir platform veya sistem olabilir. Örneğin bir Unix sistemi yada Linux sistemi de Sql Server 2005 tarafından sununlan bu servisleri kullanabilir. Biz bu makalemizde adım adım Sql Server 2005 üzerinden web servisi hizmetini nasıl verebileceğimizi incelemeye çalışacağız. Sql Server 2005 üzerinden herhangibir saklı yordamı yada kullanıcı tanımlı fonksiyonu web servisi kuralları içerisinde sunmanın ve kullanmanın yolu aşağıdaki şekilde kısaca tasvir edilmeye çalışılmaktadır.

![mk162_4.gif](/assets/images/2006/mk162_4.gif)

Buna göre ilk olarak web servisi üzerinden hizmet verecek saklı yordam yada fonksiyon hazırlanır. Hazırlanan bu fonksiyonun istemcilere cevap verebilmesi ve SOAP taleplerini uygun bir biçimde ele alabilmesi için bir EndPoint nesnesi hazırlanır. Daha sonra istemci uygulama ilgili EndPoint için proxy sınıfını oluşturur. Bildiğiniz gibi.Net ile geliştirilen web servislerini, isteci tarafındaki uygulamalarda kullanmanın yollarından birisi proxy nesnelerinden faydalanmaktır. Buna göre, tüketilmek (Consume) istenen web servisinin wsdl dökümanına göre istemci tarafında bir proxy sınıfı oluşturulur. Aynı durum, Sql Server 2005 üzerinden sunulan EndPoint nesneleri içinde geçerlidir. Tabi istenirse, proxy nesnesi yardımıyla değil SOAP taleplerini doğrudan oluşturacak ve cevapları değerlendirecek şekilde kodlama teknikleride kullanılabilir. Web servisinin kullanılabilmesi için gerekli adımların tamamlanmasının ardından son olarak Sql Server 2005 tarafındaki EndPoint tüketilerek kullanılır.

Bu kadar teorik bilgiden sonra dilerseniz örnek bir senaryo üzerinen hareket edelim. İlk olarak işe yarar bir saklı yordam (Stored Procedure) geliştireceğiz. Örnek olarak Sql Server 2005 ile standart olarak yüklenen AdventureWorks veritabanını kullanacağız. Aşağıdaki sql script ile Production isim alanındaki Product tablosundan liste fiyatı belirli bir değerin üstünde olan ürünlerin elde edilebildiği bir saklı yordam (stored procedure) oluşturulmaktadır.

```text
USE AdventureWorks
GO

CREATE PROCEDURE ListeFiyatinaGoreUrunler
    @ListPrice float
AS
BEGIN
    SELECT
          ProductID
        , Name
        , ProductNumber
        , MakeFlag
        , StandardCost
        , ListPrice
        , SafetyStockLevel
    FROM Production.Product
    WHERE (ListPrice > @ListPrice)
END
GO
```

İstemcilerin bu saklı yordama http üzerinden talepte bulunup sonuçlarını alabilmelerini sağlamak için ise Sql Server 2005 üzerinde bir EndPoint oluşturmamız gerektiğinden daha öncesinde bahsetmiştik. Bu EndPoint nesnesini aşağıdaki scriptte görüldüğü gibi oluşturabiliriz. (EndPoint nesneleride diğer çoğu veritabanı nesnesi gibi Drop ve Alter gibi komutlar ile birlikte kullanılabilmektedir.)

```text
USE AdventureWorks
GO

CREATE ENDPOINT ListeFiyatinaGoreUrunlerEndPoint
    STATE = STARTED
    AS HTTP ( path = '/sql/ListeFiyatinaGoreUrunler', AUTHENTICATION = (INTEGRATED), PORTS = (CLEAR) )
    FOR SOAP( 
                        WEBMETHOD 'ListeFiyatinaGoreUrunler' (NAME = 'AdventureWorks.dbo.ListeFiyatinaGoreUrunler',SCHEMA = STANDARD),
                        BATCHES = ENABLED,
                        WSDL = DEFAULT,
                        SCHEMA = STANDARD,
                        DATABASE = 'AdventureWorks',
                        NAMESPACE = 'http://tempUri.org/'
                 )
GO
```

Böylece sistemde http üzerinden ListeFiyatinaGoreUrunler saklı yordamına (stored procedure) gelecek talepleri karşılayacak bir EndPoint nesnesi oluşturmuş oluyoruz. Artık istemciler bu EndPoint'i http veya tcp üzerinden çağırıp sonuçlarını alabilirler.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Windows XP Service Pack 2 yüklü bir sistemde çalışıyorsanız eğer, EndPoint nesnesini oluşturbilmek için World Wide Web Publishing servisini bu işlem sırasında durdurmamız gerekebilir. Windows 2003 sistemlerinde ise buna gerek yoktur.

Geliştirdiğimiz ListeFiyatinaGoreUrunlerEndPoint adlı nesneyi ve Sql Server 2005 üzerinde kayıtlı diğer EndPoint'leri görmek için http_endpoints tablosundan yararlanabiliriz. Bu sistem tablosu EndPoint'lere ait tüm bilgileri taşımaktadır.

![mk162_2.gif](/assets/images/2006/mk162_2.gif)

EndPoint nesnesi oluşturulurken pek çok anahtar kelime kullanılmaktadır. Temel amaç EndPoint nesnesinin hangi saklı yordamı yada fonksiyonu, hangi iletişim protokolüne göre sunacağını belirlemek ve diğer konfigurasyon ayarlarını yapmaktır. Bu anahtar kelimeler hakkında birazda olsa bilgi vermekte fayda olacağı kanısındayım.

Anahtar Sözcük
İşlevi

State
EndPoint için başlangıç durumunu belirtir. Started, Stoped veya Disabled olabilir. Eğer Stoped olarak ayarlanırsa, EndPoint'e erişmek isteyen istemciler bir çalışma zamanı istisnası alırlar. Disabled olması halinde ise EndPoint sistemde kalmaya ancak gelen taleplere cevap vermemeye başalayacaktır.

Http / Tcp
Bu kısımda iletişim protokolü tanımlanır ve bu protokol üzerinden gerekli konfigurasyon ayarları belirlenir. Http dışında tcp üzerindende servis verilebilmektedir.

Path
EndPoint için gerekli URL bilgisini tanımlar.

Authentication
İstemciler için güvenlik doğrulama modelini belirler. Basic, Digets, Ntlm, Kerberos, Integrated modlarından birisi olabilir.

Ports
Clear olması halinde http portu üzerinden hizmet verilmesini sağlar. SSL olması halinde ise https üzerinden hizmet verilir.

Site
Servisin host edildiği bilgisayarın adıdır.

Soap
Bu kısımda SOAP mesajına yönelik tanımlamalar yapılır ve servisin SOAP protokolünü kullanacağı belirtilir.

WebMethod
Saklı yordamın yada kullanıcı tanımlı fonksiyonumuzun istemciler tarafından kullanılabilmesi için gerekli metod adı tanımlamasıdır.

Wsdl
Wsdl desteğini belirtir.

Database
EndPoint'in yer aldığı veritabanı adını belirtir.

Namespace
SOAP mesajları için gerekli xml isim alanını (xml namespace) tanımlar.

Artık EndPoint'imiz hazır olduğuna göre bunu herhangibir istemci uygulama üzerinde test edebiliriz. Ancak teste başlamadan önce basit olarak EndPoint'in çalışıp çalışmadığını kontrol edebiliriz. Yazdığımız EndPoint'in kontrol işlemi için tarayıcı penceresinde, http://localhost/sql/ListeFiyatinaGoreUrunler?wsdl bilgisini yazmamız yeterlidir. Eğer aşağıdaki ekran görüntüsünde olduğu gibi wsdl dökümanına ulaşabiliyorsak EndPoint'imiz başarılı bir şekilde çalışıyor demektir.

![mk162_3.gif](/assets/images/2006/mk162_3.gif)

Şimdi bu hizmeti gerçek bir.Net uygulamasında nasıl kullanacağımızı incelemeye çalışalım. Web servisi hizmetini hangi tip istemcide sunacağımıza bakmaksızın dikkat etmemiz gereken önemli bir nokta vardır. Yukarıdaki saklı yordamda (stored procedure) olduğu gibi geriye bir sonuç kümesi dönüyorsa, EndPoint bunu bir object dizisi şeklinde döndürecektir. Bu yüzden EndPoint üzerinden gelen veriyi taşıyan DataSet nesnesini bu object dizisi içerisinde yakalayıp almamız gerekmektedir. Biz örnek olarak bir Windows Uygulamasını ele alacağız. İlk olarak servisimizi uygulamamıza Add Web Reference tekniğine göre tanıtmamız gerekiyor.

![mk162_5.gif](/assets/images/2006/mk162_5.gif)

Bu işlemin ardından solution explorer'a bakacak olursak, web servisinin, proxy sınıfının ve wsdl dökümanının da eklenmiş olduğunu görürüz.

![mk162_6.gif](/assets/images/2006/mk162_6.gif)

Uygulama kodumuz ise aşağıdaki gibidir.

```csharp
private void button1_Click(object sender, EventArgs e)
{
    UrunServis.ListeFiyatinaGoreUrunlerEndPoint srv = new UrunServis.ListeFiyatinaGoreUrunlerEndPoint();
    srv.Credentials = System.Net.CredentialCache.DefaultCredentials;
    object[] sonuclar = srv.ListeFiyatinaGoreUrunler(Convert.ToDouble(txtFiyat.Text));

    foreach (object guncelNesne in sonuclar)
    {
        if (guncelNesne.GetType().ToString() == "System.Data.DataSet")
            grdUrunler.DataSource = ((DataSet)guncelNesne).Tables[0]; 
    }
}
```

UrunServis referansı içerisinde yer alan ListeFiyatinaGoreUrunlerEndPoint servisine erişirken istemcinin aynı domain içerisinde olduğunu düşünerekten DefaultCredential uygulanmıştır. Nitekim EndPoint tanımlamamızda, authentication modu olaraktanda integrated seçeneğini kullanmıştık. Diğer taraftan normal web servislerinin kullanımında web metodlarının dönüş tipi ne ise, proxy nesneleri üzerinden de aynı tipi alabilmekteyiz. Oysaki burada geriye object tipinden bir dizi dönmektedir. Bu dizinin elemanlarına çalışma zamanında Visualizer yardımıyla bakacak olursak aşağıdaki şekildende görebileceğiniz gibi bir DataSet nesnesinin de var olduğunu görürüz.

![mk162_8.gif](/assets/images/2006/mk162_8.gif)

Dolayısıyla kodumuz içerisinde object dizisinin elemanları arasında dolaşıp tipinin string karşılığı, DataSet'in Framework içerisindeki tam adına eş düşen System.Data.DataSet ile karşılaştırılması gerekmektedir. Bu tespit yapıldıktan sonra, güncel object dizi elemanı DataSet tipine dönüştürülerek veri kaynağı olarak kullanılmış ve form üzerindeki gridView kontrolüne bu veri kümesi içerisindeki 0 indeksli DataTable bağlanmıştır. Uygulamamızı çalıştırdığımızda, Sql Server 2005 üzerinden EndPoint'imize başarılı bir şekilde erişebildiğimizi görürüz.

![mk162_7.gif](/assets/images/2006/mk162_7.gif)

Görüldüğü gibi artık Sql Server 2005 üzerinde saklı yordam (stored procedure) yada kullanıcı tanımlı fonksiyonları (user defined functions), http istemcilerine hizmet verecek şekilde web servisi olarak sunmak oldukça kolaydır. Tek yapmamız gereken EndPoint nesnelerini hazırlamak ve istemcilerde tüketmek olacaktır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.