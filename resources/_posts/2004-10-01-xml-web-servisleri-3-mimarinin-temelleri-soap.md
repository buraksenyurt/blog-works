---
layout: post
title: "Xml Web Servisleri - 3 ( Mimarinin Temelleri - SOAP)"
date: 2004-10-01 12:00:00 +0300
categories:
  - xml-web-services
tags:
  - xml-web-services
  - csharp
  - xml
  - dotnet
  - soap
  - web-service
  - http
  - visual-studio
  - asmx
---
Bu makalemizde, Xml Web Servislerinin mimarisine daha yakında bakmaya çalışacak ve SOAP (Simple Object Access Protocol) 'ı kısaca tanımaya çalışacağız. Bir web servisinin, istemci uygulamalar tarafından nasıl kullanılabildiğini anlamak, web servislerinin mimarisini iyi bilmekle mümkündür. Mimariyi kolay bir şekilde anlayabilmek için, daha önceki makalemizde geliştirdiğimiz web servisi ve istemci uygulamayı göz önüne alacağız. Herşeyden önce geliştirdiğimiz web servisi local olarak test edilebilen ve tarayıcı üzerinde çalışabilen bir asmx dosyasından ve buna bağlı Code-Behind dosyasından oluşmaktadır. Web servisini test etmek için, web servisinin bulunduğu adresteki asmx uzantılı dosyayı, tarayıcı penceresinden çalıştırmak yeterlidir. Bunun sonucunda, tarayıcı penceresinde bu web servisi hakkındaki bilgilere ulaşabilir ve içerdiği metodları görebiliriz.

Ancak burada yer alan Service Description bağlantısı bize başka bir olanak daha sağlamaktadır. Bu bağlantı yardımıyla web servisimizin tüm içeriğini anlatan bir WSDL dökümanına erişebiliriz. WSDL dökümanının en önemli yanı, XML tabanlı bir içeriğe sahip olmasıdır. Diğer yandan bu döküman, web servisinde kullanılabilecek tüm metodlara, parametrelere ve dönüş değerlerine ilişkin bilgileri içermektedir.

Peki bu WSDL dökümanı ne için oluşturulur? İşte bu noktada istemci uygulamaya bir göz atmakta fayda vardır. İstemci uygulamanın web servisini kullanabilmesi için, ilk önce web servisinin bulunduğu adrese başvurması gerekir. Bu başvurunun ardından web servisine ait referansı istemci uygulamaya eklediğimizde, bir takım yeni dosyalarında uygulamaya eklendiğini görürüz. Bu dosyalardan belkide en önemli olanı Reference.cs isimli dosyadır. Geliştirdiğimiz uygulama ele alındığında Reference.cs dosyasının içeriği aşağıdaki gibi olacaktır.

```csharp
using System.Diagnostics;
using System.Xml.Serializationg
using System;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Services;

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="Geometrik HesaplamalarSoap", Namespace="http://ilk/servis/")]
public class GeometrikHesaplamalar : System.Web.Services.Protocols.SoapHttpClientProtocol 
{
    public GeometrikHesaplamalar() 
    {
        this.Url = "http://localhost/GeoWebServis/GeoMat.asmx";
    }
    
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ilk/servis/DaireAlan", RequestNamespace="http://ilk/servis/", ResponseNamespace="http://ilk/servis/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public System.Double DaireAlan(System.Double r)
    {
        object[] results = this.Invoke("DaireAlan", new object[]{r});
        return ((System.Double)(results[0]));
    }
    
    public System.IAsyncResult BeginDaireAlan(System.Double r, System.AsyncCallback callback, object asyncState) 
    {
        return this.BeginInvoke("DaireAlan", new object[] {r}, callback, asyncState);
    }
    
    public System.Double EndDaireAlan(System.IAsyncResult asyncResult) 
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((System.Double)(results[0]));
    }
    
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ilk/servis/DaireCevre",     RequestNamespace="http://ilk/servis/", ResponseNamespace="http://ilk/servis/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public System.Double DaireCevre(System.Double r) 
    {
        object[] results = this.Invoke("DaireCevre", new object[] {r});
        return ((System.Double)(results[0]));
    }
    
    public System.IAsyncResult BeginDaireCevre(System.Double r, System.AsyncCallback callback, object asyncState) 
    {
        return this.BeginInvoke("DaireCevre", new object[] {r}, callback, asyncState);
    } 

    public System.Double EndDaireCevre(System.IAsyncResult asyncResult) 
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((System.Double)(results[0]));
    }
}
```

Visual Studio.NET tarafından otomatik olarak oluşturulan bu dosyada dikkat çekici noktalar vardır. Herşeyden önce, karşımızda, web servisimizin bir görüntüsü yer almaktadır. Web servisimizde yazdığımız metodlar kullandıkları parametreler ve daha başka bilgiler. Ancak önemli olan bu dosya sayesinde, istemci uygulamanın artık web servisine ait bir nesne örneğini oluşturup kullanabilecek olmasıdır. Dolayısıyla, istemci uygulamamıza web servisimize ait referansı eklediğimizde, istemci uygulamda bu servise ait bir nesne yapısı oluşturulabilmiştir. Bu sayede aşağıdaki gibi bir bildirim geçerli hale gelir.

```csharp
localhost.GeometrikHesaplamalar gh=new Istemci.localhost.GeometrikHesaplamalar();
```

Dahası, bu nesne üzerinden, web servisindeki metodları aynı isimler ile kullanabiliriz.

```csharp
lblAlan.Text=gh.DaireAlan(r).ToString();
lblCevre.Text=gh.DaireCevre(r).ToString();
```

Olaya daha detaylı bakıldığında, Reference.cs’ nin aslında, istemci ve web servisi arasındaki haberleşmeyi sağlayacak bir Proxy nesnesini oluşturmak amacıyla kullanıldığını söyleyebiliriz. Dolayısıyla, istemci uygulama bu servisi kullanmak istediğinde, yani bu servis üzerinden bir metodu çağırmak istediğinde, bu talebi proxy nesnesinden ister. Proxy nesnesi ise bu talebi, web servisine iletir. Web servisi gelen talebi değerlendirir ve ürettiği cevabı yine istemci uygulamadaki proxy nesnesine gönderir. Proxy nesneside sonuçları, uygulama ortamına iletir.

![mk100_1.gif](/assets/images/2004/mk100_1.gif)

Şekil 1. Proxy Nesnesini ve SOAP'ın Xml Web Servisi Mimarisindeki yeri.

Diğer bir sorun, proxy nesnesine gelen taleplerin, web servisine giderken hiç bir engel ile karşılaşmadan nasıl hareket edeceği ve geri geleceğidir. İstemci uygulama, proxy nesnesinde normal olarak talepte bulunur ve cevapları alır. Buradaki ilişki normal olarak bir nesne.metod ilişkisidir. Ancak proxy nesnesi bu mesajları, esnek, kolay okunabilir, herhangibir engele takılmayacak bir hale getirmek durumundadır. Bu iş için XML tabanlı bir bilgi akışı biçilmiş kaftandır. Yinede hareket edecek mesajların uygun bir formasyonda taşınmaları ve web servisinin anlayabileceği bir dilde ifade edilebilmeleri daha doğrudur. Bu noktada SOAP (Simple Object Access Protocol – Basit Nesne Erişim Antlaşması) devreye girer.

Web servislerinin kullanılmasında, web servisleri ve istemciler arasındaki haberleşmenin belirli standartlar çerçevesinde geliştirilmesi çok önemlidir. SOAP (Simple Access Object Protocol) işte bu noktada devreye giren ve web servisi-istemci sisteminin en önemli kısmını oluşturan bir yapı taşıdır. SOAP, web servisleri ve istemciler arasında gidip gelecek mesajların, XML tabanlı olarak belirlendiği standartlara uygun formatta taşınmasını sağlayan bir protokoldür. Çoğunlukla HTTP üzerinde çalışan SOAP, FTP ve SMTP gidi diğer iletişim protokolleri üzerinden de kullanılabilir.

SOAP protokolü, web servisleri ile istemciler arasında gerçekleştirilen veri alışverişinde, karşılıklı olarak akıcak mesajların nasıl ve ne şekilde paketleneceğini yada başka bir deyişle bilgilerin nasıl kapsülleneceğini belirtir. SOAP, özünde XML tabanlı mesajların oluşturulmasını belirtir. Bu nedenle SOAP protokolünü uygulayan mesajlar (ki bunlar SOAP Mesajı olarak adlandırılır), herhangibir ağ ortamında hiç bir sorunla karşılaşmadan uzak makineler arasında iletilebilirler. SOAP protokolü, 4 temel üzerine inşa edilmiştir.

![mk100_2.gif](/assets/images/2004/mk100_2.gif)

Şekil 2. SOAP Protokolünün Temelleri.

SOAP protokolünün dayandığı bu temel kurallar içerisinde en önemlisi Envelope kısmıdır. Bir SOAP mesajı mutlaka bir zarf olarak teşkil edilmeli ve Zarf kurallarına göre tasarlanmalıdır. Diğer temellerin uygulanması zorunlu değildir. Veri kodlama kuralları, özellikle serileştirilen nesneler için bir model sunar. Başka bir deyişle, tanımlanmış veri tiplerinin uygulama için kullanılma kurallarına karar verir. Bu katmandaki kuralların uygulanması opsiyoneldir. Mesaj değişim modeli, web servisi ile istemciler arasında değiş tokuş edilen mesajlar için bir istek/cevap deseni tanımlar. SOAP, Remote Procedure Call tip mekanizmasını esas alan bir veri değiş tokuş desenini kullansada bu bir zorunluluk değildir. Mesaj değişim modelide opsiyoneldir. Veri bağlama kuralları ile, SOAP’ın iletişim protokollerinin birbirlerine nasıl bağlanacağına dair tanımlamalar içerir. Veri bağlama kurallarıda opsiyoneldir.

Kısaca SOAP protokolü, mutlaka iletişimsel mesajların SOAP Zarf kurallarına uygun bir biçimde tasarlanmasını zorunlu kılar. SOAP mesajlarının zarf kurallarına uygun bir biçimde tasarlanması için aşağıdaki şekilde belirtilen format kullanılır. Burada bir SOAP zarfının temel yapısı belirtilmiştir.

![mk100_3.gif](/assets/images/2004/mk100_3.gif)

Şekil 3. SOAP Zarfının Temel Yapısı.

SOAP Zarfları bir SOAP mesajının başlık ve gövde olmak üzere iki ana kısımdan oluşması gerektiğini belirtir. En önemli kısım gövdedir. Burada, yapılan metod çağırımlarına ilişkin bilgiler ile çağrı sonucu istemcilere gönderilecek cevaplara ait xml tabanlı bilgiler yer alır.

SOAP mesajlarını anlamanın en iyi yolu, onları gerçek bir uygulamada takip etmektir. Bu amaçla, geliştirmiş olduğumuz Web servisini ve istemci uygulamamızı kullanacağız. İstemci ve web servisi arasında hareket eden SOAP mesajlarını takip edebilmek amacıyla Microsoft firmasının sunduğu SOAP Tookit aracını kullanabiliriz. Bu aracı bugün itibariyle [http://download.microsoft.com/download/2/e/0/2e068a11-9ef7-45f5-820f-89573d7c4939/soapsdk.exe](http://download.microsoft.com/download/2/e/0/2e068a11-9ef7-45f5-820f-89573d7c4939/soapsdk.exe) adresinden temin edebilirsiniz. SOAP mesajlarını takip edebilmek amacıyla istemci uygulamamızda ufak bir değişiklik yapmamız gerekiyor. Bunun için istemci uygulamda, web servisine ait proxy nesnesinin sınıf dosyası içerisinde (reference.cs) yer alan,

```csharp
this.Url = "http://localhost/GeoWebServis/GeoMat.asmx";
```

satırını

```csharp
this.Url = "http://localhost:8080/GeoWebServis/GeoMat.asmx";
```

şeklinde değiştirmeliyiz. Şimdi SOAP Toolkit 3.0 ile yüklenen Trace Utility aracını çalıştıralım. Ardından Trace Utility aracında, New menüsünden, Formatted Trace’ i seçelim.

![mk100_4.gif](/assets/images/2004/mk100_4.gif)

Şekil 4. Yeni bir Trace açıyoruz.

Karşımıza çıkacak olan aşağıdaki pencereyi bu hali ile bırakalım.

![mk100_5.gif](/assets/images/2004/mk100_5.gif)

Şekil 5. Trace Setup.

Ardından istemci uygulamamızı çalıştıralım ve web servisimizi kullanalım. Tekrardan Trace Utility penceremize dönelim. Aşağıdaki ekran görüntüsü elde ederiz. Görüldüğü gibi, trace utility aracı, web servisimiz ile istemci uygulamamız arasındaki SOAP mesajlarını yakalamıştır. Burada iki mesaj sekmesi görünmektedir. Bunların herbiri web servisimizdeki bir metoda karşılık gelmektedir.

![mk100_6.gif](/assets/images/2004/mk100_6.gif)

Şekil 6. SOAP Mesajları.

Örneğin Message #1 sekmesini inceleyelim. Üsteki XML bilgisi, istemcinin web servisine gönderdiği talebi (Request) göstermektedir. Alttaki mesaj ise, web servisinden istemciye gelen cevabı (Response) gösterir. İstemcinin talebini incelediğimizde aşağıdaki XML dökümanını elde ederiz.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
 <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
 <soap:Body>
 <DaireAlan xmlns="http://ilk/servis/">
  <r>10</r> 
  </DaireAlan>
  </soap:Body>
  </soap:Envelope>
```

Burada istemciden web servisine giden SOAP mesajı görülmektedir. Görüldüğü gibi mesaj tagı ile başlar. Burada istemci tarafından çağırılan metodun ismi ve bu metoda gönderilen parametre değeri yer almaktadır. Ayrıca metodun hangi isim alanı içinde yer aldığıda belirtilmiştir. Bu isim alanının web servisin içerisindeki WebService niteliği ile belirttiğimiz isim alanı olduğuna dikkat ediniz. Soap mesajımızın başlık kısmı ise, HTTP Header sekmesinde görülebilir. Buradaki XML satırları içerisinde en önemlisi tagının olduğu satırdır.

```xml
<soapaction>"http://ilk/servis/DaireAlan"</soapaction>
```

Soapaction tagı, kullanılan web servisinde belirtmiş olduğumuz xml isim alanının sonuna web servisinde yer alan metodun ismi eklenerek oluşturulmuştur. Burada, Soap mesajının web servisi tarafından çözümlenmesi sırasında, karşılık gelecek isim alanının (WebService sekmesinde belirttiğimiz) ve bu isim alanı içinden çağırılan metodun (WebMethod niteliği ile belirttiğimiz) tanımlamaları yer alır. Böylece web servisi gelen soap mesajına karşılık gelen metodu bulup çalıştırabilir. Gelelim web servisinden istemciye dönen SOAP Mesajına.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
 <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
 <soap:Body>
 <DaireAlanResponse xmlns="http://ilk/servis/">
  <DaireAlanResult>314</DaireAlanResult> 
  </DaireAlanResponse>
  </soap:Body>
  </soap:Envelope>
```

Görüldüğü gibi, web servisinden istemciye dönen Soap mesajında en dikkat çekici nokta Response ve Result anahtar kelimeleridir. İstemci tarafından çözlülecek olan bu Soap mesajı, istemci tarafından metodun çağırıldığı satıra dönecek geri dönüş değerlerini içerir. Yine buradaki gövdede yer alan metod tanımında kullanılan isim alanı ile (xmlns="http://ilk/servis/), istemciye ait proxy nesnesini oluşturan sınıf içerisindeki WebService niteliğinde kullanılan isim alanının aynı olduğuna dikkat edelim.

```csharp
[System.Web.Services.WebServiceBindingAttribute(Name="Geometrik HesaplamalarSoap", Namespace="http://ilk/servis/")]
```

Burada görüldüğü gibi SOAP mesajlarımız tek parametre alan metodlar ve tek sonuç içeren geri dönüş değerlerini istemci ve web servisi arasında belirli bir standart dahilinde taşımaktadır. Diğer yandan, özellikle değişken sayıda parametre alan ve geriye dizi veya veri kümesi döndürebilen SOAP mesajlarıda mümkündür. Bu durumu analiz edebilmek amacıyla web servisimize parametre olarak bir dizi alan ve geriye bu dizinin işlenmiş halini döndürecek bir metod ilave edelim.

```csharp
[WebMethod(Description="Daire Cevre Hesabini Dizi Elamanlarina Uygular.")] public double[] DaireCevreDizi( double[] r)
{
	int eleman_Sayisi=r.Length;      double[] dizi=new double[eleman_Sayisi];
	for(int i=0;i<eleman_Sayisi;i++)
	{
		dizi[i]=r[i]*pi*2;
	}
	return dizi;
}
```

Bu web metodumuz, istemciden double türünden bir dizi alacak ve bu dizideki elemanlara çevre hesaplaması işlemlerini uygulayacak. Sonuçlar ise yine bir dizi şeklinde istemci bilgisayara geri döndürülecek. İstemci uygulamamızda bu yeni metodu test edebilmemiz için, proxy nesnesinin, web servisinin yeni versiyonuna uygun bir şekilde güncellenmesi gerekir. Bunun için, istemci uygulamada Solution Explorer penceresinde, Web References sekemsinde localhost öğesine sağ tıklayarak açılan menüden, Update Web Reference’ i seçmemiz yeterli olacaktır.

![mk100_7.gif](/assets/images/2004/mk100_7.gif)

Şekil 7. Xml Web Servisini Güncellemek.

Bu durumda, istemci uygulamamız web servisimiz için oluşturduğu proxy nesnesini yeniliklere göre güncelleyecektir. Aksi takdirde web servisimize eklediğimiz metodu kullanamayız. Bu işlemlerin ardından, istemci uygulamamıza aşağıdaki kod satırlarını ilave edelim.

```csharp
double[] dizi=new double[3]; dizi[0]=1; dizi[1]=2; dizi[2]=3;   double[] diziSonuc=new double[3]; diziSonuc=gh.DaireCevreDizi(dizi); foreach(double eleman in diziSonuc) {      lblDizi.Text+=eleman.ToString()+" ";
}
```

Burada yaptığımız, 3 elemanlı double türünden bir dizi tanımlamak ve bu diziye atadığımız elemanları, web servisimizdeki metodumuza parametre olarak gönderip sonuçları bir label kontrolünde yazdırmak. Bizim için önemli olan kodların yazılış tarzından çok, web servisine bir dizinin gönderilmesi, orada işlenmesi ve sonuçların geri gelmesi sırasında, SOAP mesajlarının ne şekilde oluşacağıdır.

Şimdi Trace Utility ‘den SOAP mesajlarını yenide inceleyelim. Yerel makinede çalıştığımız ve proxy nesnemizide güncellediğimiz için, proxy sınıfındaki localhost tanımlamasını tekrardan localhost:8080 olarak değiştirmemiz, SOAP mesajlarını Trace Utility aracılığıyla yakalayabilmemiz açısından gerekli olabilir. SOAP Toolkit Trace Utility aracıyla, SOAP mesajlarına baktığımızda, istemciden web servisine DaireCevreDizi metodu için giden XML tabanlı bilgilerin aşağıdaki şekilde oluşturulduğunu görürüz.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
 <soap:Body>
 <DaireCevreDizi xmlns="http://ilk/servis/">
 <r>
  <double>1</double> 
  <double>2</double> 
  <double>3</double> 
  </r>
  </DaireCevreDizi>
  </soap:Body>
  </soap:Envelope>
```

Görüldüğü gibi web metoda gönderdiğimiz dizi elemanları double XML veri tipinden elemanlar olarak SOAP mesajının gövdesine katılmıştır. Aynı şekilde web servisinden geri dönen mesaja baktığımızda aşağıdaki sonucu elde ederiz.

```text
<?xml version="1.0" encoding="utf-8" ?> 
- <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
 <soap:Body>
 <DaireCevreDiziResponse xmlns="http://ilk/servis/">
 <DaireCevreDiziResult>
  <double>6.28</double> 
  <double>12.56</double> 
  <double>18.84</double> 
  </DaireCevreDiziResult>
  </DaireCevreDiziResponse>
  </soap:Body>
  </soap:Envelope>
```

Sonuçlar yine double XML veri tipi elemanı olarak geri döndürülmüştür. Şu ana kadar yaptığımız örneklerde, istemci uygulamanın web servisindeki sınıf ve metodları bir proxy nesnesi yardımıyla nasıl kullandığına şahit olduk. Peki istemci bir uygulama, kendisine web referansı olarak eklediği web servisine dair bir proxy nesnesini nasıl oluşturabildi? Bu sorunun cevabını Web Servislerinin SOAP’ tan sonra olmassa olmaz materyali olan WSDL verebilir. WSDL konusunu bir sonraki makalemizde incelemeye çalışacağız. Tekrarda görüşünceye dek hepinize mutlu günler dilerim.