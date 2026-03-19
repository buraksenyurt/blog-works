---
title: "Web Servisleri İçin Dört Püf Nokta"
pubDate: 2006-03-14 10:00:00
categories:
  - XML Web Services
tags:
  - XML Web Services
  - C#
  - .NET
  - XML
  - SOAP
  - Web Service
  - HTTP
  - IIS
  - Authentication
  - Performance
  - Caching
  - Serialization
  - DataSet
---

# Web Servisleri İçin Dört Püf Nokta
Değerli Okurlarım Merhabalar,

Web Servislerini yazmak ve kullanmak, çoğu zaman bir web service projesi oluşturmak ve istemci tarafında Add Web Reference tekniği ile oluşturulan proxy sınıfını kullanmaktan ibaret basit bir mimari olarak düşünülür. Ancak sanılanın aksine Web servislerinin yazılmasında ve kullanılmasında dikkate değer çeşitli püf noktalar vardır. İşte bu makalemizde bu püf noktalardan dördünü maddeler halinde incelemeye çalışacağız.

1 - Bir web metodunun overload edilmesi (aşırı yüklenmesi) standart bir metodun overload edilmesinden daha farklıdır.

Örnek bir web servisinde aşağıdaki gibi aşırı yüklenmiş (overload) iki web metodumuz olduğunu göz önüne alalım. Bu metodların aşırı yüklenmiş olduklarını rahatlıkla söyleyebiliriz. Nitekim, her iki metodunda imzaları farklıdır. (Aşırı yüklemede (Overloading) metod imzasının, metodun aldığı parametre sayısı ve parametrelerin tiplerine bağlı olduğunu anımsayalım.)

```csharp
[WebMethod()]
public double Carp(double x,double y)
{
    return x*y;
}

[WebMethod()]
public int Carp(int x,int y)
{
    return x*y;
}
```

Normal şartlar altında bu metodları çalışma zamanında bir problem olmadan yürütebilmemiz gerekir. Ancak metodları içeren herhangibir web servisini çağırdığımızda, çalışma zamanında aşağıdaki ekran ile karşılaşırız.

![mk151_1.gif](images/mk151_1.gif)

Sebep web servisinde yer alan SOAP mesajlarının yapısı ile ilgilidir. Aynı isimli metodlar SOAP mesajları içerisine aynı isimler ile gömülmeye çalışıldığından ve birbirlerinden ayırt edilebilmeleri için xml içerisinde var olan bir overload mekanizması olmadığından böyle bir problem yaşanmıştır. Bu sorunu çözmek için hata mesajında belirtildiği gibi WebMethod niteliğinin (attribute) MessageName özelliğinden yararlanırız. Bu özellik ile her iki metodu birbirinden benzersiz olacak şekilde ayırabiliriz. Bu nedenle yukarıdaki örnek kod parçasında yazdığımız metodlara ait WebMethod niteliklerini aşağıdaki gibi yeniden düzenlemeliyiz.

```csharp
[WebMethod(MessageName="CarpForDoubles")]
public double Carp(double x,double y)
{
    return x*y;
}

[WebMethod(MessageName="CarpForIntegers")]
public int Carp(int x,int y)
{
    return x*y;
}
```

Bu haliyle web servisi sorunsuz bir şekilde çalışacaktır. Değişikliğin yaptığı etkileri kullandığınız web servisinin wsdl dökümanında daha rahat izleyebilirsiniz.

2 - Aralarında kalıtımsal ilişki olan nesneler söz konusu olduğunda, taban sınıfa (base class) ait nesne örneklerinden oluşan dizi tiplerinin döndürüldüğü web metodlarında uymamız gereken kurallar vardır.

Çok basit olarak aralarında kalıtımsal ilişki bulunan aşağıdaki Sekil, Dortgen ve Ucgen sınıflarını göz önüne alalım. Şekildende görüleceği üzere, Dortgen ve Ucgen sınıfları Sekil taban sınıfından türeyen (derived) sınıflarımızdır.

![mk151_2.gif](images/mk151_2.gif)

Sınıflar;

```csharp
public class Sekil
{
}
public class Dortgen:Sekil
{
}
public class Ucgen:Sekil
{
}
```

Şimdi bu ilişkiyi kullanan aşağıdaki gibi bir web metodumuz olduğunu düşünelim. Kalıtımın bir etkisi olarak, Sekil sınıfına ait bir nesne örneği, kendisinden türeyen Dortgen ve Ucgen sınıfı tipinden nesne örneklerini taşıyabilmektedir. SekilleriAl isimli web metodumuz aslında Sekil sınıfı tipinden bir diziyi geriye döndürmektedir. Ancak bu dizi kodlardanda görebileceğiniz gibi Dortgen ve Ucgen tipinden nesne örneklerini taşımaktadır.

```csharp
[WebMethod()]
public Sekil[] SekilleriAl()
{
    Sekil[] sekiller=new Sekil[3];
    sekiller[0]=new Dortgen();
    sekiller[1]=new Ucgen();
    sekiller[2]=new Dortgen();
    return sekiller;
}
```

Bu web metodunu çağırdığımızda çalışma zamanında bir istisna (excpetion) alırız. İstisnanın sebebi son derece basittir. Web metodunun çalışma zamanında üreteceği çıktıya göre, web metodundan mutlaka ve mutlaka Sekil tipinden elemanlar barındırdan bir dizi tipi döndürüleceği düşünülmektedir. Bunu web servisimizin WSDL (Web Service Description Language) dökümanında daha kolay görebiliriz.

![mk151_3.gif](images/mk151_3.gif)

Dikkat ederseniz, Web metodumuzun geri döndüreceği tip içerisinde tanımlanan ArrayOfSekil kompleks tipi (Complex Type), sadece Sekil tipinden değerler almaktadır. Dolayısıyla Sekil tipinin, Dortgen veya Ucgen tipinden nesne örneklerini barındırabileceğini, dolayısıyla ArrayOfSekil kompleks tipinin yapısı içerisinde bu tiplerinde yer alabileceğini XML tarafında söylememiz gerekmektedir. İşte bu amaçla System.Xml.Serialization isim alanında yer alan Xmlnclude niteliğinden yararlanabiliriz. Web metodumuzu aşağıdaki hali ile güncelleyelim.

```csharp
[WebMethod()]
[XmlInclude(typeof(Dortgen))]
[XmlInclude(typeof(Ucgen))]
public Sekil[] SekilleriAl()
{
    Sekil[] sekiller=new Sekil[3];
    sekiller[0]=new Dortgen();
    sekiller[1]=new Ucgen();
    sekiller[2]=new Dortgen();
    return sekiller;
}
```

Şimdi WSDL dökümanımıza yeniden bakacak olursak ArrayOfSekil isimli kompleks tipin Dortgen ve Ucgen tiplerinide barındırabilecek şekilde tanımlandığını görürürüz.

![mk151_4.gif](images/mk151_4.gif)

Diğer taraftan web metodumuz artık başarılı bir şekilde çalışacaktır.

3 - Web metodlarında performans için Caching kullanabiliriz.

Web uygulamalarını geliştirirken performansı arttırıcı tedbirlerden birisi olarak caching mekanizmalarına başvururuz. Dilersek bu tekniği web servislerinde yer alan web metodların döndürdüğü sonuçlar içinde uygulayabiliriz. Web metodların WebMethod niteliğinin (attribute) CacheDuration isimli özelliği saniye cinsinden ön bellekleme süresini belirtir. Bu özellik yardımıyla bir web metodun döndüreceği sonuçları, web sunucusunun ön belleğine alabiliriz. Bu da performans olarak web metodunu kullanan istemcilere hızlı cevap dönmesi anlamına gelmektedir. Nitekim cevaplar hazır olarak ara bellekte tutulan çıktılardan döndürülür.

Basit olarak aşağıdaki kod parçasında, Northwind veritabanında yer alan Order Details tablosundan belirli bir OrderID'ye ait satırlar bir DataSet nesnesi içerisinde geri döndürülmektedir. CacheDuration burada ön bellekleme süresini 120 saniye olarak belirtmektedir. Buna göre yapılan sorgu sonucu döndürlecek olan veri kümesi web sunucusunun ön belleğinde 120 saniye süreyle tutulacaktır.

```csharp
[WebMethod(CacheDuration=120)]
public DataSet GetOrderDetails(int OrderId)
{
    using(SqlConnection con=new SqlConnection("data source=LONDON;database=Northwind;integrated security=SSPI"))
    {
        using(SqlCommand cmd=new SqlCommand("Select OrderID,ProductID,UnitPrice,Quantity,Discount From [Order Details] Where OrderID=@OrderID",con))
        {
            cmd.Parameters.Add("@OrderID",SqlDbType.Int);
            cmd.Parameters["@OrderID"].Value=OrderId;
            SqlDataAdapter da=new SqlDataAdapter(cmd);
            DataSet ds=new DataSet();
            da.Fill(ds,"Siparisler");
            return ds;
        }
    }
}
```

Şimdi bu web metodu barındırdan bir servisi kullanan basit bir console uygulamamız olduğunuzu düşünelim. Console uygulamamızın kodları aşağıdaki gibidir.

```csharp
static void Main(string[] args)
{
    AltinSerivisi.AltinServis altin=new UsingCacheDuration.AltinSerivisi.AltinServis();
    DataSet ds=altin.GetOrderDetails(10250);
    foreach(DataRow dr in ds.Tables[0].Rows)
    {
        for(int i=0;i<ds.Tables[0].Columns.Count;i++)
        {
            Console.Write(dr[i].ToString()+" ");
        }
        Console.WriteLine();
    }
}
```

İlk olarak uygulamamızı çalıştıralım ve elde ettiğimiz değerlere bakalım. Daha sonra Order Details tablosunda 10250 numaralı siparişe ait bilgilerde ufak bir değişiklik yapalım. Örneğin Quantity değerini 10' dan 20' ye çıkartalım. 120 saniyelik süre dolmadan metodumuzu tekrar çağıracak olursak, Quantity değerinin halen daha 10 olarak geldiğini görürüz. Bunun sebebi verinin o an için web servisinin bulunduğu web sunucusunun ön belleğinden geliyor oluşudur. Elbetteki 120 saniyelik bu önbellekleme süresi sonunda Quantity değerinin yenilendiğini görebiliriz. Ön belleğe alma işlemi bu örnekte görüldüğü gibi sadece parametrik bazda değil, parametresiz web metodları içinde geçerlidir.

4 - Web Servisi tarafında security için en basit haliyle Windows Authentication'ı kullandığımızda istemci tarafında da yapmamız gerekenler vardır.

Web servisleri, web uygulamalarında olduğu gibi bir web sunucu üzerinden yayımlanırlar. Bu anlamda bakıldığında bir web uygulaması için söz konusu olan authentication seçeneklerini, web servisleri içinde ele alabiliriz. Özellikle windows tabanlı doğrulama göz önüne alındığında istemci tarafında çalışan uygulamaların, güvenlik bilgilerini doğru bir şekilde gönderebiliyor olması gerekir. İlk olarak doğrulama metodu Basic Authentication olarak ayarlanmış bir web servisimiz olduğunu düşünelim. IIS tarafında ilgili web servisimiz için gerekli güvenlik ayarlarının aşağıdaki şekilde görüldüğü gibi olması gerekmektedir.

![mk151_5.gif](images/mk151_5.gif)

Şimdi istemci tarafında bu web servisini kullanacak olan uygulamamızı göz önüne alalım. Web metodumuzu çağırabilmemiz için öncelikle bu servise istemci tarafından bir güvenlik belgesi göndermemiz gerekir. (Credential) Bu güvenlik belgesini taşıyacak olan tip ICredential arayüzüdür. İstemci tarafında bu bilgiyi hazırlayabilmek için ICredential arayüzünü (Interface) uygulayan NetworkCredential tipini kullanabiliriz. Aşağıdaki kod parçasında GoldServis isimli web servisimizdeki GetOrderDetails isimli metodu çağırmadan önce, servisi çalıştırmak için gerekli credential bilgisinin nasıl eklendiği gösterilmektedir.

```csharp
AltinSerivisi.AltinServis altin=new UsingCacheDuration.AltinSerivisi.AltinServis();
System.Net.NetworkCredential credential=new System.Net.NetworkCredential("admin","admin1234");
altin.Credentials=credential;
DataSet ds=altin.GetOrderDetails(10250);
foreach(DataRow dr in ds.Tables[0].Rows)
{
    for(int i=0;i<ds.Tables[0].Columns.Count;i++)
    {
        Console.Write(dr[i].ToString()+" ");
    }
    Console.WriteLine();
}
```

Eğer istemci uygulama doğru şifre ve kullanıcı bilgisini gönderemez ise HTTP 401: Access Denied istisnasını alırız. Ancak doğru güvenlik bilgilerinin gönderilmesi sonucunda web metodu başarılı bir şekilde çalıştırılacaktır.

Bu makalemizde kısaca web servislerini kullanırken dikkate alabileceğimiz bir kaç noktaya değindik. Aynı isimli aşırı yüklenmiş metodların kullanılma tarzını, web metodlardan geriye dönen kompleks tiplerin içerisinde kalıtım ilişkisi olduğu takdirde bunu Xml tarafınada bildirmemiz gerektiğini, performans için caching'i kullanabileceğimizi ve sunucu tarafında bir authentication olması halinde istemci için gerekli username ve password bilgilerinin nasıl hazırlanıp gönderilebileceğini incelemeye çalıştık. Elbetteki web servisleri ile ilgili dikkate değer daha pek çok nokta var. Örneğin SoapExtension yardımıyla soap mesajlarının şifrelenmesi gibi. Bu ve benzer diğer konulara ilerki makalelerimizde değinmeye çalışacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.
