---
layout: post
title: "Ado.Net Data Services Ders Notları - 3 (İstemci Geliştirmek)"
date: 2008-10-06 12:00:00 +0300
categories:
  - ado-net-data-services
tags:
  - ado-net-data-services
  - csharp
  - dotnet
  - ado-net
  - linq
  - wcf
  - wpf
  - windows-forms
  - silverlight
  - xml
  - soap
  - rest
  - json
  - web-service
  - xml-web-services
  - http
  - javascript
  - performance
  - generics
  - visual-studio
---
Hatırlayacağınız gibi daha önceki iki ders notumuzda Ado.Net Data Service örneklerinin nasıl geliştirilebileceğini incelemeye çalışmıştık. Hatırlatmak gerekirse, Ado.Net Data Service'ler ile verilerin Entity Data Model (EDM) veya Custom LINQ Provider bazlı katmanlar üzerinden REST modeline göre sunulması mümkün olmaktadır. Bu noktada söz konusu servislerin WCF'in REST modelini kullanan ve Ado.Net üzerine odaklanmış bir açılımı olduğu görüşünde hem fikir olabiliriz. Ne varki Servis Yönelimli Mimari (Service Oriented Architecture-SOA) temelli çözümlerde yap-bozun en önemli iki parçasını servis ve istemciler oluşturmaktadır. Bir başka deyişle, servislerin tamamlayıcısı olan ve ilgili hizmetleri kullanacak istemci uygulamalar (Client Applications) olmalıdır. İşte bu yazımızda istemci uygulamaları göz önüne alacağız.

Ado.Net Data Service ve istemci arasında geçen bu hikayede, anahtar öneme sahip bir kaç kelimede yer almaktadır. WCF, Ado.Net, REST vb. Bunlar az çok istemcilerin kimler olabileceğinide ortaya çıkartan terimlerdir. Aslında bir servis istemcisinin herhangibir uygulama olabilmesi istenir. Platform kriterleri gözetilmeksizin. Fakat geçmiş zamanlarda sadece belirli platformlara yönelik çözümler de ele alınmamış değildir ki halen daha popüler olarak pek çok alt yapıda kullanılmaktadır. Buna verilebilecek en güzel örnek belkide.Net Remoting çözümleridir..Net Remoting temelli uygulamalar sadece.Net tabanlı istemci ve sunucuları baz almaktadır. Bu bir kısıtlamadır ama performans ve verimlilik gibi avantajlarıda getirmektedir. Ancak zaman ilerledikçe farklı tipte platformların ortaklaşa haberleşebilmesi daha büyük önem arz etmeye başlamıştır. Buda Xml Web Service'lerin popüler olmasının nedenlerinden birisidir:) Ama uzun zamandır elimizde çok daha güçlü bir kozun olduğunu da belirtmek isterim; Windows Communication Foundation.

Tekrardan sihirli kelimelerimize dönelim. WCF kelimesi, geliştireceğimiz servisin WCF kurallarının bir sonucu olarak ortaya çıktığının açık bir göstergesidir. Buda kendi içerisinde WCF'in nimetlerini barındıracak bir servis çözümünü ifade eder ki buna JSON (JavaScript Object Notation), Syndication, Web Programming Model gibi pek çok önemli kriterde dahil olur. Dolayısıyla WCF servislerini ele alabilen tüm istemci çeşitleri bu senaryoda olasıdır. Ancak Ado.Net kelimesi, servisimizi belirli bir yöne doğru odaklamaktadır. Buna göre geliştirilen servis tamamen verilerin (Data) sunumu üzerine konuşlandırılmaktadır. Bu zorunluluk olmamakla birlikte bütünlüğü sağlayıcı bir hedef olarak görülmelidir. Buda istemci tipini belirleyici diğer bir etkendir. Ancak REST (REpresentationalStateTransfer) kelimesi olayı biraz daha belirginleştirmektedir. Söz konusu istemciler REST modeline göre talepte bulunabilmelidir. Yani HTTP protokolü üzerinde GET,HEAD,POST,DELETE gibi metodlara göre talepte bulunabilmeli ve gelen sonuçlarıda irdeleyebilmelidirler.

Doğruyu söylemek gerekirse bu kelimeleri bir kenara bırakıp herhangi çeşit istemci uygulama ele alınabilir diyerekten işin içerisinden de sıyrılabiliriz:) Yinede güncel teknolojiler göz önüne alındığında aşağıdaki maddelerde yer alan istemci tiplerinin dikkat çekeceği ortadadır.

- Ajax Tabanlı Web Sayfaları (Ajax Based Web Pages)
- Silverlight Nesneleri
- WPF (Windows Presentation Foundation), WinForms gibi Windows Uygulamaları
- Diğer Servisler (WCF Servisleri, Xml Web Servisleri,.Net Remoting Uygulamaları, Windows Servisleri vb...)
- Sınıf Kütüphaneleri-Class Libraries

Bu tipler çoğaltılabilir. Ancak benimde dikkatimi çeken ve özellikle üzerinde durulmaya değer çeşitler Ajax tabanlı web sayfaları ve Silverlight nesneleridir ki bunlar şu zaman itibariyle son derece popüler uygulamalardır. Yanlız dikkat edilmesi gereken başka noktalarda vardır. Söz gelimi bir Ado.Net Data Service örneği farklı servisler tarafındanda tüketilebilir. Böyle bir durumda tüketici servisin kendisi, aslında tükettiği servis için bir istemci olmaktadır. Yine extreme senaryolar göz önüne alınabilir. Söz gelimi Active Directory hizmetini özel bir LINQ Provider ile güvenli bir şekilde farklı bir lokasyona bir Ado.Net Data Service olarak sunabiliriz. Örneğin dünya üzerindeki bir otele ait tüm nesnel verilerin Active Directory kökenli olaraktan tek bir merkezde tutulduğunu düşünün. Diğer lokasyonlardaki oteller bu merkezi verileri kullanmak isteyecektir. Bu noktada tüketici istemciler bir servis olup söz konusu hizmeti kapalı ağ içerisinde (Intranet diyebiliriz) ele alabilir ve diğer istemcilere sunabilir. Ki bu kapalı ağ istemcileride söz konusu lokasyondaki otelin içerisinde yer alan çeşitli tipteki uygulamalardır. Bu tabiki gerçek bir vaka değil ancak sizlerde bu cümlede bir kaç dakikalığına durup çeşitli Ado.Net Data Service senaryoları düşünebilir ve bunları yakın çevrenizdeki yetkin kişiler ile tartışarak analiz edebilirsiniz.

İstemci hangi çeşitten olursa olsun servis ile olan iletişimini kod seviyesinde kolaylaştırmak açısından genellikle Proxy nesneleri göz önüne alınır. Bu bir zorunluluk değildir. Nitekim REST modeline göre servise gidecek olan HTTP paketlerinin manuel olarak hazırlanıp gönderilmeside mümkündür. Öyleki bu işlem için HttpWebRequest yada HttpWebResponse tipleri kolayca göz önüne alınabilir. Bir anlamda örneğin, XML Web Servislerinde bir SOAP (Simple Object Access Protocol) paketinin bahsettiğimiz tipler ile hazırlanıp gönderilmesinden ve geri gelen cevabın açılarak ele alınmasından farklı bir işlem değildir. Ne varki Proxy kullanımı kodlamacının işini oldukça kolaylaştırır. Çünkü bu sayede geliştirici bildik kodları yazarken sanki kendi ortamındaki bir nesneyi kullanıyormuş hissine kapılır. Gelip giden paket içerikleri ile uğraşmak zorunda kalmaz. Halbuki söz konusu servis talepleri, proxy tarafından servisin anlayacağı paketler haline getirilerek gönderilir. Benzer şekilde servisten gelen paketlerde proxy tarafından açılarak istemcideki çalışma zamanı (RunTime) nesnelerine devredilir. Bu konuda aşağıdaki şekil biraz daha aydınlatıcı bilgi verebilir.

![mk260_1.gif](/assets/images/2008/mk260_1.gif)

Tabi burada proxy tiplerinin geliştirme zamanında eklenmesi gibi bir zorunluluk yoktur. Bazı uygulama çeşitlerinde örneğin Ajax temelli web sayfalarında söz konusu proxy tiplerine ait nesne örnekleri çalışma zamanında transparant olarak oluşturulup kullanılabilirler. Ajax tabanlı bir web istemcisi üzerinden bir Ado.Net Data Service'in kullanımı çokda kolay değildir. İşin özellikle benim açımdan keyifsizleştiği nokta istemci tarafı için javascript kodları döktürülmesi gerekliliğidir. Ajax tabanlı bir web formunda bir Ado.Net Data Service'inin nasıl kullanılabileceğini ilgili [görsel dersten](http://www.csharpnedir.com/videoindir.asp?id=118) takip edebilirsiniz.

Peki biz bu yazımızda neler yapacağız? Aslında yukarıdaki listede yer almayan bir istemci uygulama geliştiriyor olacağız:) Tahmin edeceğiniz üzere bir Console uygulaması. Sonuçta amacımız bir istemci uygulamada basit REST taleplerinde bulunabilmek. Bunun içinde görsel detayların fazla olmadığı ve odağın tamamen koda kaydığı bir ortam kullanmamız öğrenmemiz açısından önemli olacaktır. İşte Console uygulaması seçmemizin (yada seçmemin) nedenide budur? Her zaman olduğu gibi basit bir Ado.Net Data Service'i geliştireceğiz.

Senaryomuzda yine AdventureWorks veritabanını ve buradaki ProductSubCategory, Product tablolarını ele alacağız. Bu tablolar arasındaki bire çok ilişki istemci tarafında ilişkisel veri çekme işlemlerini analiz etmemizi sağlayacaktır. Ado.Net Data Service'in nasıl geliştirileceğini daha önceki notlarımızda ve görsel derslerimizde yeterince ele almıştık. Bu nedenle sadece EDM (Entity Data Model) grafiğinin, AdventureWorksServices.svc.cs kodlarının ve Solution içeriğininin aşağıdakilere benzer olmasına özen göstermeniz yeterli olacaktır.(Servis uygulamasının bir WCF Service şablonu olduğunu hatırlatalım.)

EDM Grafiği;

![mk260_2.gif](/assets/images/2008/mk260_2.gif)

Solution İçeriği;

![mk260_3.gif](/assets/images/2008/mk260_3.gif)

AdventureWorksServices.svc.cs içeriği;

```csharp
using System;
using System.Data.Services;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using AdventureWorksModel;

public class AdventureWorksServices 
    : DataService<AdventureWorksEntities>
{
    public static void InitializeService(IDataServiceConfiguration config)
    {
        config.SetEntitySetAccessRule("*", EntitySetRights.All);
    }
}
```

Sonrasında ise istemci Console uygulaması için gerekli proxy tiplerini üreteceğiz. Proxy üretimi için iki farklı seçeneğimiz bulunmaktadır. Bunlardan birisi WCF Servislerindende aşina oluğumuz Add Service Reference seçeneğidir. Bu seçeneği kullandığımızda karşımıza gelen dialog penceresinde Ado.Net Data Service adresinin yazılması yeterli olacaktır. Elbette geliştirilen örnek gereği Discover menüsünden Services in Solution seçeneği ele alınabilir. Nitekim servis ve istemci uygulamalarımız aynı solution içerisinde yer almaktadır.

![mk260_4.gif](/assets/images/2008/mk260_4.gif)

Bu noktada normal bir WCF Service referansı eklerken karşımıza çıkan seçeneklerden tamamının aktif olmadığı hemen göze çarpmaktadır. Öyleki bir WCF servisi bu teknik ile eklenirken aktif olan Advanced düğmesi Ado.Net Data Service eklenirken aktif değildir. Buda olay bazlı asenkron ayarlamalar, erişim belirleyicileri (Internal veya Public) gibi bazı seçeneklerin kullanılamadığı anlamına gelmektedir. Ekleme işlemi sonrasında istemci uygulama tarafında servis ile ilişkili proxy referanslarının oluşturulduğu açık bir şekilde görülebilir.

![mk260_5.gif](/assets/images/2008/mk260_5.gif)

Yine burada edmx uzantılı tip dikkati çekmektedir. Bu açıkçası bir XML içeriğidir ve istemci tarafına indirilmiş olan serileştirilebilir tipler ile ilgili eşleştirmeleri üzerinde taşımaktadır. Yine dikkat çekici noktalardan birisi istemci tarafı için bir config dosyası oluşturulmamış olmasıdır. Nitekim bu modelde EndPoint kullanımı söz konusu değildir. Zaten talepler basit HTTP metodları olacak şekilde servise ulaştırılmaktadır ki bu aşamada üretilen taşıyıcı (Container) sınıf devreye girmektedir (AdventureWorksEntities). Oluşturulan tiplere bakıldığında ise aşağıdaki sınıf diagramında yer alan açılımların oluştuğu görülür. Dikkat edileceği üzere servis tarafından sunulan entity tipleri istemci tarafındada oluşturulmuştur. Asıl yüklenici tip ise AdventureWorksEntities isimli DataServiceContext türevli sınıftır. Bu sınıf sayesinde CRUD operasyonlarının tamamı kolay bir şekilde istemci tarafında ele alınabilir.

![mk260_6.gif](/assets/images/2008/mk260_6.gif)

Diğer Proxy üretme tekniği ise SvcUtil aracının Ado.Net Data Service'ler için geliştirilmiş olan versiyonu DataSvcUtil komut satırı programıdır. Komut satırından proxy üretimi için Visual Studio 2008 Command Prompt üzerinden DataSvcUtil aracının aşağıdaki resimde olduğu gibi kullanılması yeterli olacaktır. Çıktı AdventureProxy.cs isimli dosya içerisine yapılmaktadır. Uri parametresinden sonra ise Proxy üretimi için kaynak olan Ado.Net Data Service adresi verilmiştir.

![mk260_7.gif](/assets/images/2008/mk260_7.gif)

Elbette elimizde Visual Studio 2008 gibi bir IDE olmadığı durumlarda proxy üretimi için DataSvcUtil aracını kullanmak gerekmektedir. Aksi durumda ise Add Service Reference seçeneği çok daha mantıklıdır. Sonuç olarak ben örneğimizde Add Service Reference seçeneğini ele aldım.

Artık ve nihayet istemci tarafındaki kodlarımızı geliştirmeye başlayabiliriz. Öncelikli olarak küçük bebek adımları ile başlamakta yarar vardır. Örneğin tüm ProductSubCategory listesini elde etmek istediğimizi düşünelim. Bu durumda kodlarımızı aşağıdaki gibi geliştirmemiz yeterli olacaktır.

```csharp
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Services.Client;
using ClientApp.AdventureSpace;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Öncelikli olarak Proxy nesnesi örneklenir.
            // Parametre olarak Ado.Net Data Service' in URL bilgisi kullanılır.
            AdventureWorksEntities proxy = new AdventureWorksEntities(new Uri("http://localhost:1740/AdventureServices/AdventureWorksServices.svc"));

            // CreateQuery metodu parametre olarak Entity adını almaktadır.
            // Metodun döndürdüğü sonuç kümesi DataServiceQuery tipi ile ele alınabilir.
            // İstenirse var anahtar kelimeside göz önüne alınabilir. Her iki durumdada for döngüsü çalışacaktır.
            DataServiceQuery<ProductSubcategory> subCategories=proxy.CreateQuery<ProductSubcategory>("ProductSubcategory");
            // var subCategories = proxy.CreateQuery<ProductSubcategory>("ProductSubcategory");

            // Elde edilen sonuç kümesinin her bir elemanı ProductSubcategory sınıfı tipindendir.
            foreach (ProductSubcategory subCategory in subCategories)
            {
                //Her bir alt kategorinin Name ve ProductSubcategoryID özelliklerinin değerleri yazdırılır.
                Console.WriteLine("{0} : {1}",subCategory.ProductSubcategoryID,subCategory.Name);
            }
        }
    }
}
```

Bunun sonucu olarak aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk260_8.gif](/assets/images/2008/mk260_8.gif)

Şimdi işi biraz daha ilerletelim. Söz gelimi bu alt kategorilerin isimlerine göre tersten sıralı bir şekilde gelmesini istediğimizi düşünelim. Bu durumda CreateQuery metodu içerisinde ProductSubcategory?$orderby=Name desc şeklinde bir ifade kullanmamız kaçınılmazdır. Ne varki CreateQuery metodu sadece Entity adları ile çalışmaktadır ve bu nedenle ek parametreler alamaz. Dolayısıyla bu denemenin sonucu olarak aşağıdaki ekran görüntüsünde yer alan istisnaya (Exception) düşülür.

![mk260_9.gif](/assets/images/2008/mk260_9.gif)

Öyleyse çare nedir? Parametrik bir sorgu söz konusu ise eğer bu durumda Execute metodunun kullanılması gerekmektedir. Bir başka deyişle kodları aşağıdaki şekilde değiştirmek yeterli olacaktır.

```csharp
AdventureWorksEntities proxy = new AdventureWorksEntities(new Uri("http://localhost:1740/AdventureServices/AdventureWorksServices.svc"));

// Parametrik sorgu gönderimi için Execute metodu kullanılmalıdır.
var subCategories = proxy.Execute<ProductSubcategory>(new Uri("/ProductSubcategory?$orderby=Name desc", UriKind.Relative));

foreach (ProductSubcategory subCategory in subCategories)
{
    Console.WriteLine("{0} : {1}",subCategory.ProductSubcategoryID,subCategory.Name);
}
```

Program çalıştırıldığında aşağıdaki ekran görüntüsü elde edilir. Dikkat edileceği üzere alt kategoriler isimlerine göre tersten sıralı olacak şekilde elde edilmektedir.

![mk260_10.gif](/assets/images/2008/mk260_10.gif)

Bu sorgular son derece basittir. İşi biraz daha karıştırmaya ne dersiniz? Örneğin A dan Z'ye sıralanmış alt kategorilerden ilk üçünü ve bunlara bağlı ürünleri elde etmek istediğimizi düşünelim. Bu noktada ProductSubcategory ve Product tipleri arasındaki ilişki (Association) son derece önemlidir. Bu sonuçları elde etmek için kodları ilk etapta aşağıdaki gibi geliştiririz.

```csharp
AdventureWorksEntities proxy = new AdventureWorksEntities(new Uri("http://localhost:1740/AdventureServices/AdventureWorksServices.svc"));

var subCategories = proxy.Execute<ProductSubcategory>(new Uri("/ProductSubcategory?$orderby=Name&$top=3", UriKind.Relative));

foreach (ProductSubcategory subCategory in subCategories)
{
    Console.WriteLine("{0} : {1}",subCategory.ProductSubcategoryID,subCategory.Name);
    // O andaki alt kategoriye bağlı ürünleri gezmek için Product özelliğinden yararlanılır.
    foreach (Product product in subCategory.Product)
    {
        Console.WriteLine("\t {0}, {1}, {2}",product.ProductID.ToString(),product.Name,product.ListPrice.ToString());
    }
}
```

Ancak program çalıştırıldığında hiç beklenmedik bir sonuç elde edilir. Aynen aşağıdaki resimde olduğu gibi. Dikkat edileceği üzere sadece Alt kategori adları ve ID değerleri elde edilmiş bağlı olan ürün listeleri gelmemiştir.

![mk260_11.gif](/assets/images/2008/mk260_11.gif)

Sebep son derece açıktır. Çünkü sadece ProductSubcategory içeriği servis tarafından istenmiştir. Bunlara bağlı Product nesne toplulukları alınmamıştır. Görsel derslerimizdende hatırlayacağınız üzere expand anahtar kelimesininin kullanılmasının sebebide budur. Dolayısıyla kodda aşağıda görüldüğü gibi küçük bir değişiklik yapmak gerekecektir.

```csharp
var subCategories = proxy.Execute<ProductSubcategory>(new Uri("/ProductSubcategory?$orderby=Name&$top=3&$expand=Product", UriKind.Relative));
```

Bu haliyle kod çalıştırıldığında aşağıdaki sonuçlar elde edilir.

![mk260_12.gif](/assets/images/2008/mk260_12.gif)

Makalenin bu kısımlarında içimden "böylesine önemli ve bir o kadar da güzide bir teknoloji içerisinde LINQ kullanılmaz mı?" diye geçirmiyor değilim. Tahmin ediyorumki sizlerinde bu yönde bazı beklentileri vardır. Öyleyse gelin kolları sıvayalım ve aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Services.Client;
using ClientApp.AdventureSpace;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AdventureWorksEntities proxy = new AdventureWorksEntities(new Uri("http://localhost:1740/AdventureServices/AdventureWorksServices.svc"));

            var subCategories = from sc in proxy.ProductSubcategory
            orderby sc.Name descending
            select sc;
    
            foreach (ProductSubcategory subCategory in subCategories)
            {
                Console.WriteLine("{0} : {1}",subCategory.ProductSubcategoryID,subCategory.Name);
            }
        }
    }
}
```

Bu kez gördüğünüz gibi Execute yada CreateQuery gibi metodlar kullanmadık. Bunların yerine doğrudan bir LINQ sorgusu yazdık ve işte sonuç;

![mk260_14.gif](/assets/images/2008/mk260_14.gif)

Aslında yazılan LINQ sorgusu istemci tarafında bir HTTP ifadesinin oluşturulmasına ve servise doğru gönderilmesine neden olmaktadır. Öyleki debug modda subCategories değişkenine bakıldığında aşağıdaki ekran görüntüsünde olduğu gibi bir QueryString ifadesi oluştuğu gözlemlenir.

![mk260_13.gif](/assets/images/2008/mk260_13.gif)

Buna göre ProductSubcategory ve bunlara bağlı ürünlerin elde edilmesi için yazılmış olan kod parçasında LINQ ifadelerini aşağıdaki gibi kullanarak aynı sonuçların elde edilebileceği açık bir şekilde görülebilir.

```csharp
AdventureWorksEntities proxy = new AdventureWorksEntities(new Uri("http://localhost:1740/AdventureServices/AdventureWorksServices.svc"));

// Take metodu ile A...Z ye sıralanmış listenin ilk 3 elemanı alınmış olunur.
var subCategories = (from sc in proxy.ProductSubcategory 
orderby sc.Name 
select sc).Take<ProductSubcategory>(3);

// Elde edilen alt kategoriler dolaşışır
foreach (ProductSubcategory subCategory in subCategories)
{
    Console.WriteLine("{0} : {1}",subCategory.ProductSubcategoryID,subCategory.Name);
    // O andaki alt kategoriye bağlı ürünlerin çekilmesi için LoadProperty metodu kullanılır. İkinci parametre ilişkinin taşındığı özellik adıdır.
    proxy.LoadProperty(subCategory, "Product");
    // Artık o andaki alt kategori için yüklenen Product satırları dolaşılabili
    foreach (Product product in subCategory.Product)
    {
        Console.WriteLine("\t{0} {1} {2}",product.ProductID.ToString(),product.Name,product.ListPrice.ToString());
    }
}
```

Bu kod parçasında tek dikkat edilmesi gereken nokta LoadProperty özelliğinin kullanımıdır. Nitekim bu özellik ile ilişkisel veriler yüklenmediği takdirde alt kategoriye bağlı ürün listeleri servis tarafında çekilmez.

> İster Execute metodu olsun ister LINQ sorgusu olsun, ilişkisel özelliklerce taşınan verilerin çekilmesi için sırasıyla expand anahtar kelimesinin yada LoadProperty metodunun kullanılması gerekir.

Artık sizde farklı sorgulama örnekleri deniyerek istemci tarafında neler yapılabileceğini analiz edebilirsiniz. Görüldüğü üzere bir Ado.Net Data Service'in istemci tarafından ele alınması standart bir servis kullanımına çok benzemektedir. Proxy tipleri burada işi kolaylaştırmakla birlikte LINQ sorgularınında kullanılabiliyor olması kişisel görüşüme göre son derece önemlidir.

Böylece bugünkü ders notlarımızında sonuna gelmiş bulunuyoruz. Bu ders notlarımızda basit bir istemcinin nasıl geliştirilebileceğini incelemeye çalıştık. Konu ile ilişkili olaraktan ilgili [görsel dersi](http://www.csharpnedir.com/videoindir.asp?id=115) takip etmenizi öneririm. Bir sonraki ders notlarımızda istemci tarafında CRUD (CreateReadUpdateDelete) operasyonlarının nasıl ele alınabileceğini analiz etmeye çalışacağız; ve eğer mümkün olursa özel LINQ Provider kullanılması halinde, servis tarafında Insert, Update, Delete oparasyonlarına olanak sağlamak için neler yapılması gerektiğine değiniyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örneği indirmek için tıklayın](/assets/files/2008/DevelopingAstoriaClient.rar)