---
title: "Asp.Net Temelleri : Etkili Trace Kullanımı"
pubDate: 2007-08-02 12:00:00
categories:
  - Asp.Net
tags:
  - Asp.Net
  - C#
  - XML
  - .NET
  - HTTP
  - Authentication
---

# Asp.Net Temelleri : Etkili Trace Kullanımı
Değerli Okurlarım Merhabalar,

Web uygulamalarında son kullanıcıların (End Users) şikayetçi olabileceği pek çok konu vardır. Bunlar arasında popüler olanlarından biriside sayfaların yavaş açılıyor olmasıdır. Nihayetinde son kullanıcıları her zaman için sabırsız ve acelesi olan kişiler olarak düşünmek doğru bir yaklaşım olacaktır. Sayfaların yavaş açılıyor yada geç cevap veriyor olmasının donanımsal yada çeşitli çevre faktörleri nedeniyle bilinen sebepleri vardır.

Söz gelimi bağlantı hızının düşük olması ilk akla gelen nedendir. Ancak geliştiriciler (developers) olarak bizlerinde üzerine düşen önemli görevler vardır. Sonuç itibariyle bir geliştirici, mininum donanım gereksinimleri karşılandığı takdirde hızlı ve yeterli performasta çalışabilen bir web uygulaması geliştiriyorsa, donanım kapasite ve yeteneklerinin daha yüksek olduğu son kullanıcılarda aynı web uygulamasının çok daha iyi sonuçlar vereceği düşünülebilir.

> Trace mekanizmasını kullanmak için küçük bir neden; örneğin talep (request) edilen sayfanın üretilmesi ve HTML çıktısının hazırlanması uzun zaman alıyor olabilir. Sayfanın üretimi sırasında hangi adımların çok zaman aldığını görmek için Trace mekanizmasından yararlanılabilir.

Elbetteki bir web uygulamasının hızını, performansını ve cevap verebilme yeteneklerini etkiliyecek pek çok faktör vardır. Yine örnek olarak düşünülürse, 10 kullanıcılı bir intranet ortamında gayet iyi çalışan bir web uygulaması, internet'e açıldığında karşılaşabileceği n kullanıcı talebi sonrasında beklenen performansı gösteremeyebilir. Buradaki en büyük etken kullanıcı sayısıdır. Bu nedenle web uygulamalarını ne kadar iyi programladığımızı düşünsekte, sonuçlara bakıldığında gerek test gerekse gerçek ortamlarda istediğimiz sonuçları alamadığımızı görebiliriz.

Dolayısıyla test aşamasında uygulamanın genelini izleyebilmek ve sonuçları analiz ederek gerekli tedbirleri alabilmek son derece önemli bir konudur. Bu gibi durumlarda web uygulamasının genelinin yada problemli olabileceği düşünülen sayfaların çalışma zamanı ve sonrasındaki hareketlerinin izlenmesi adına Asp.Net modelinde Trace mekanizasından faydalanılmaktadır. İşte bu makalemizde Trace kavramını derinlemesine incelemeye çalışacağız.

Trace mekanizması ile sayfa (Page) veya uygulama seviyesinde (Application Level), kullanıcıdan gelen bir talebin işlenmesi sırasında ve sonuçların elde edilmesinin sonrasında gerekli host bilgileri yakalanabilir. Bu bilgiler değerlendirilerek sorunun nerede olduğu daha kolay bir şekilde tespit edilebilir.

> Trace; HTTP talebi ile ilgili olaraktan sunucu (server) taraflı çalışma zamanı (run-time) ve sonrası detay bilgilerinin alınmasını sağlayan bir Asp.Net mekanizmasıdır.

Trace mekanizması sadece sayfa ve uygulama seviyesinde değil, bileşen seviyesindede (Component Level) ele alınabilir. Nitekim, web uygulamalarında sayfaların çoğu buton arkası programlama yerine bileşenleri ele alır. Çok doğal olarak bileşenler içerisinde meydana gelebilecek sorunların ele alınması sırasında yine Trace mekanizması ele alınabilir. Trace ile elde edilen sonuçlar üretilen genellikle sayfa sonlarına HTML bilgisi olarak eklenebilirler. Bunun dışında Trace.axd isimli özel dosyalardanda uygulama genelinde, talep sırasına göre sayfaların izlenmesi sağlanabilir.

İlk olarak Trace işlemlerini sayfa seviyesinde ele almaya çalışacağız. Bir sayfanın Trace çıktısını yakalamak için iki farklı yöntem vardır. Bunlardan birincisi Page direktifinde yer alan Trace niteliğine true değeri atanmasıdır. Diğer yol ise kod tarafında Trace nesne örneği üzerinden IsEnabled özelliğine true değerini atamaktır.

Direktik içerisinden Trace mekanizmasının açılması;

```text
<%@ Page Language="C#" Trace="true"%>
```

Kod tarafından Trace mekanizmasının açılması;

```csharp
protected void Page_Load(object sender, EventArgs ea)
{
    Page.Trace.IsEnabled = true; 
}
```

Sonuç olarak bu iki kullanım şekline göre sayfanın sonuna Trace bilgileri aşağıdaki ekran görüntüsünde olduğu gibi eklenecektir.

![mk216_1.gif](images/mk216_1.gif)

Bu çıktıda tüm Trace bilgisi gösterilmemektedir. Temel olarak Trace çıktısında yer alan kısımlar aşağıdaki tabloda olduğu gibidir.

Trace Çıktısında Yer Alan Bölümler

Request Details
Bu kısımda talep ile ilişkili genel bilgiler yer alır. Örneğin talep tipi (request type) ilk çağrıda Get iken sayfa sunucuya gönderildikten sonra Post değerini alır. Bir başka örnek detay bilgisi olarakta Session Identity (SessionId) değeri verilebilir.

Trace Information
Bu kısımda özellikle sayfanın yaşam döngüsü (Page Life Cycle) içerisinde yer alan olayların genel süreleri yer alır. Buradaki bilgilerden hareket ederek sayfanın üretimi sırasında hangi kısımların ne kadar süre harcadığı görülebilir.

Control Tree
Sayfa içerisindeki kontrollerin ağaç yapısını verir. Böylece özellikle form içerisinde kullanılan başka taşıyıcı kontroller (container controls) var ise, bunların içerisinde yer alan alt kontrollerin elde edilmesi için ne kadar derine inilebileceği kolayca tespit edilebilir.

Session State
Kullanıcıya ait oturum (Session) içerisinde tutulan verilerin değerlerinin ve tiplerinin gösterildiği kısımdır.

Application State
Application nesnesi içerisindeki anahtarlar (keys) ve değerlerinin (values) gösterildiği kısımdır.

Request Cookies Collection
Sayfaya gelen talep sırasındaki çerezlere ait bilgilerin tutulduğu koleksiyondur.

Response Cookies Collection
Sayfadan istemciye dönen cevap içerisinde yer alan çerezlere (Cookies) ait bilgilerin tutulduğu koleksiyondur.

Headers Collection
İstemciden sunucuya gelen HTTP paketindeki başlık bilgileri görülebilir. Örneğin AcceptEncoding bilgisi.

Response Headers Collection
İstemciye dönen HTTP paketindeki başlık bilgilerini içerir. Örneğin üretilen içerik tipi (Content Type) görülebilir.

Form Collection
Sunucudan istemciye gönderilen form bilgileri görülebilir. Örneğin üretilen VIEWSTATE değerine bakılabilir.

Querystring Collection
Sayfadan istenen query string bilgileri var ise bunların adları ve o anki değerleri görülebilir.

Server Variables
Sunucu değişkenleri elde edilebilir. Örneğin, sunucu uygulamanın fiziki yolu (APPLPHYSICALPATH), yerel ip adresi (LOCALADDR) gibi.

Aynı bilgilere ulaşmak için adres satırında Trace.axd dosyasıda talep edilebilir.

> Trace.axd, WebResource.axd benzeri bir dosyadır. Dolayısıyla çalışma zamanında özel şekilde ele alınır. Asp.Net çalışma ortamı Trace.axd taleplerinin TraceHandler isimli sınıfa ait nesne örneklerine devredilerek karşılanmasını sağlar.
> ![mk216_11.gif](images/mk216_11.gif)
> Dolayısıyla söz konusu talep sonrası oluşan ekran çıktısı TraceHandler sınıfı tarafından hazırlanır. Makinedeki ana web.config dosyasının içeriğine bakıldığında bu açıkça görülebilir. Burada dikkat edilmesi gereken noktalardan birisi sadece Trace.axd için böyle bir handler'ın yazılmış olmasıdır. Trace.axd ve WebResource.axd dışında gelecek taleplerHttpNotFoundHandler tarafından ele alınmaktadır.
> ![mk216_10.gif](images/mk216_10.gif)

Aşağıdaki ekran görüntüsünde Trace.axd'nin talep edilmesinin örnek sonuçları gösterilmektedir.

![mk216_2.gif](images/mk216_2.gif)

Kod tarafında Trace bilgilerini ele almak için Page nesne örneği üzerinden Trace özelliği kullanılmaktadır. Aslında Trace özelliği doğrudan TraceContext sınıfına ait bir nesne örneği döndürmektedir. TraceContext sınıfı sealed olarak işaretlenmiş bir tip olduğundan türetilerek (inherit) özelleştirilemez. Bu sınıfın Framework içerisindeki yeri şekilsel olarak aşağıdaki gibidir.

![mk216_3.gif](images/mk216_3.gif)

TraceContext sınıfının üyeleri göz önüne alındığında, Write ve Warn metodları kod içerisinden Trace çıktısına bilgi yazdırmak amacıyla kullanılmaktadır. Bunların arasındaki tek fark Warn metodunun yazıyı kırmızı punto ile basıyor olmasıdır. Diğer taraftan her iki metodda 3 farklı aşırı yüklenmiş versiyona sahiptir. Bu versiyonlar yardımıyla yazdırılan bilginin kategorisi (Category), içeriği (Message) ve o anda yakalanmış bir istisna (Exception) var ise bu istisna nesne örneği Trace çıktısına gönderilebilir. IsEnabled özelliği ile daha öncedende belirtildiği gibi Trace'in etklinleştirilmesi veya pasif moda çekilmesi sağlanabilir. TraceMode özelliği TraceMode enum sabiti tipinden değerler alabilmektedir.

![mk216_4.gif](images/mk216_4.gif)

Bu değerler sayesinde Trace çıktısında yer alan Trace Information kısmındaki bilgilerin kategori veya süre bazlı olarak sıralanıp sıralanmayacağı belirlenebilir. Bu üyeler dışında TraceContext sınıfı özellikle bileşenlerin içerisinde ele alınmak istendiğinde yapıcı metoddan bir örnek oluşturulması gerekmektedir. Bunun için yapıcı metod HttpContext tipinden bir parametre alır. Bunu ilerleyen kısımlarda ele alacağız. TraceContext sınıfının birde TraceFinished isimli olayı vardır.

TraceContextEventHandler temsilcisinin tanımladığı yapıya uygun olay metodu çağırılabilir. Bu olay TraceContext sınıfına.Net 2.0 ile birlikte katılmıştır. TraceFinished olayı, Trace bilgileri toplandıktan sonra TraceContext sınıfının kendisi tarafından tetiklenir. TraceContextEventArgs tipinden olan ikinci parametre sayesinde Trace ile ilgili veriler elde edilebilir ve bu sayede farklı veri kaynaklarına yazılmaları sağlanabilir. Aşağıdaki kod parçasında bu olayın kullanımı örneklenmeye çalışılmaktadır.

```csharp
protected void Page_Load(object sender, EventArgs ea)
{
    Page.Trace.IsEnabled = true;
    Page.Trace.TraceFinished += new TraceContextEventHandler(Trace_TraceFinished);
}

void Trace_TraceFinished(object sender, TraceContextEventArgs e)
{
    IEnumerator numarator=e.TraceRecords.GetEnumerator();
    while (numarator.MoveNext())
    {
        TraceContextRecord record = (TraceContextRecord)numarator.Current;
        Response.Write(record.Category + " : " + record.Message + "<br/>");
    }
}
```

TraceContextEventArgs tipinden e değişkeni üzerinden elde edilen koleksiyon içerisindeki her bir eleman TraceContextRecord sınıfına ait birer nesne örneğidir. Bu tip yardımıyla Trace Information kısmındaki kategori, mesaj, istisna tipi ve uyarı olup olmadığı (IsWarning) bilgilerine erişilebilir. Aşağıdaki ekran görüntüsünde bu bilgiler yer almaktadır.

![mk216_5.gif](images/mk216_5.gif)

Bu kodun yer aldığı sayfa ilk talep edildiğinde aşağıdaki ekran çıktısında yer alan içerik elde edilir.

![mk216_6.gif](images/mk216_6.gif)

Post işleminden sonra ise TraceFinished içerisinden yakalanan mesajların sayısı sayfanın yaşam döngüsü nedeni ile artacak ve aşağıdakine benzer bir ekran çıktısı elde edilecektir.

![mk216_7.gif](images/mk216_7.gif)

Söz konusu olay metodu ile her ne kadar Trace Information ile ilgili çok az bilgiye ulaşsakta zaten geri kalan verilerin çoğu HttpResponse veya HttpRequest gibi tipler yardımıyla elde edilebilmektedir. Dolayısıyla bu olay içerisinde bu tipler aracılığıyla elde edilen veriler başka veri ortamlarına da yazdırılabilirler.

Şimdi Trace mekanizmasını farklı yollar ile incelemeye devam edeceğiz. İlk olarak sayfa seviyesinde Trace bilgilerini izlemenin nasıl faydası olabiliceğini görmeye çalışacağız. Bu amaçla aşağıdaki web sayfası ve kodları göz önüne alınabilir. (Bu ve sonraki asp.net sayfalarında, işlemlerin kolay takip edilmesi amacıyla inline-coding tekniği kullanılmıştır.)

```text
<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

protected void btnCek_Click(object sender, EventArgs e)
{
    string urunAdlari=""; 
    using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
    {
        SqlCommand cmd = new SqlCommand("Select Top 10000 TransactionId From Production.TransactionHistory", conn);
        conn.Open();
        SqlDataReader reader = cmd.ExecuteReader(); 
        while (reader.Read())
        {
            urunAdlari += reader["TransactionId"].ToString()+"|";
        }
        reader.Close();
    }
    Session.Add("TumKategoriler", urunAdlari);
} 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Untitled Page</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <asp:Button ID="btnCek" runat="server" Text="Veriyi Çek" OnClick="btnCek_Click" />
            </div>
        </form>
    </body>
</html>
```

Web sayfamızda yer alan düğmeye basıldığında, TransactionHistory tablosundaki ilk 10000 TransactionId değeri elde edilir ve bunlar bir string içerisinde birleştirilerek Session'a atılır. Kod her ne kadar anlamlı gözükmesede (ki gözükmediği ortada:)) ortaya çıkardığı sonuçlar nedeniyle kayda değerdir. Nitekim sayfa talep edildikten sonra düğmeye basıldığında sayfanın uzun bir sürede çıktı verdiği görülecektir. Burada herhangibir hata görünmemektedir. Ancak sayfanın çıktısının üretilmesi ve istemciye gelmesinin neden uzun sürdüğüde incelenmelidir. İşte bu amaçla bu sayfa üzerinde Trace işlemi gerçeklenmelidir. Hatırlanacağı üzere bunu kod veya direktif içerisinde yapabileceğimizi belirtmiştik. Bu nedenle Page direktifinde yer alan Trace niteliğine true değerini verelim ve kodlarımızı aşağıdaki gibi değiştirelim.

```csharp
using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
{
    SqlCommand cmd = new SqlCommand("Select Top 10000 TransactionId From Production.TransactionHistory", conn);
    conn.Open();
    Trace.Warn("TransactionId Cekme", "TransactionId değerleri çekilmeye başlanacak");
    SqlDataReader reader = cmd.ExecuteReader(); 
    while (reader.Read())
    {
        urunAdlari += reader["TransactionId"].ToString()+"|";
    }
    Trace.Warn("TransactionId Cekme", "TransactionId değerleri çekildi...");
    reader.Close();
}
```

Burada Warn metodu kullanılarak urunAdlari toplanmadan önce ve toplandıktan sonra Trace çıktısına kategori bazlı değerler yazdırılmaktadır. Bunun sonucunda sayfa çıktısı aşağıdaki gibi olacaktır.

![mk216_8.gif](images/mk216_8.gif)

Sonuçları irdelerken From First (s) ve From Last (s) kısımlarındaki süreler çok önemlidir. From First (s) ile sayfanın talepten sonraki yaşam döngüsü başladığından beri geçen toplam süre ifade edilir. Form Last (s) ise, bir önceki işlem ile son işlem arasındaki geçen süre farkıdır. Böylece kod yardımıyla sayfanın çalışma zamanı çıktısına bakılmış ve süre uzamasının nerede olduğu tespit edilebilmiştir. Görüldüğü gibi string bilgisini oluştururken + operatörü nedeni ile verinin oluşumu son derece fazla zaman almıştır. Aslında burada alınacak tedbir son derece basittir. + operatörü yerine StringBuilder kullanmak. Bunun için kod aşağıdaki gibi değiştirilebilir.

```csharp
StringBuilder builder = new StringBuilder();
using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
{
    SqlCommand cmd = new SqlCommand("Select Top 10000 TransactionId From Production.TransactionHistory", conn);
    conn.Open();
    Trace.Warn("TransactionId Cekme", "TransactionId değerleri çekilmeye başlanacak");
    SqlDataReader reader = cmd.ExecuteReader(); 
    while (reader.Read())
    {
        builder.Append(reader["TransactionId"].ToString());
        builder.Append("|");
    }
    Trace.Warn("TransactionId Cekme", "TransactionId değerleri çekildi...");
    reader.Close();
}
Session.Add("TumKategoriler", builder.ToString());
```

Buna göre sonuçlar aşağıdaki ekran görüntüsündeki gibi olacaktır.

![mk216_9.gif](images/mk216_9.gif)

Görüldüğü gibi kod içerisinde şüpheli bulunulan noktalarda uygulanacak teknikler ile sayfanın çalışma zamanındaki hali çok daha kolay bir şekilde izlenebilmektedir.

> Trace sınıfına ait Write ve Warn metodları uygulama içerisinde pek çok yerde kullanılabilir. Buna göre Trace mekanizmasının geçersiz kılınması haline, söz konusu metodlar görmezden gelineceği için, tüm uygulama kodunu gözden geçirerek bu metodlara ait satırların kaldırılmasına gerek kalmayacaktır.

Uygulama seviyesinde Trace mekanizmasını aktif kılabilmek için web.config dosyasında trace elementi kullanılmalı ve enabled özelliğine true değeri atanmalıdır. Bunun dışında trace elementi içerisinden belirlenebilecek bazı ayarlamalarda yapılabilir. Böylece web uygulaması içerisindeki tüm sayfalar için izleme yapılabilir.

![mk216_12.gif](images/mk216_12.gif)

trace elementinin içerisinde kullanılabilecek nitelikler ve anlamları ise aşağıdaki tabloda olduğu gibidir.

trace Elementine Ait Genel Özellikler

requestLimit
trace log içerisinde uygulamaya ait kaç talebin (request) saklanacağı belirtilir. Limit aşılması halinde eğer mostRecent özelliğinin değeri false ise uygulama yeniden başlatılana veya trace log bilinçli bir şekilde temizlenene kadar log'a bilgi atılmaz. Bu niteliğin varsayılan değeri 10 dur. Maksimum olarak 10000 değeri verilebilir. 10000' den büyük bir değer verilmesi halinde ise, bu değer otomatik olarak 10000' e çekilir.

pageOutput
true ise Trace bilgileri web uygulaması içerisindeki sayfaların sonuna eklenir. false olması halinde ise izleme bilgileri sadece Trace.axd üzerinden takip edilebilir.

localOnly
true olmaslı halinde Trace çıktısını istemciler göremez. Sadece web uygulamasının olduğu makinedeki kullanıcı görebilir. Bu bir anlamda Trace çıktısını sadece geliştiricinin izleyebilmesi anlamınada gelebilir ki güncel projelerde sıkça başvurulan bir yöntemdir.

enabled
true olması haline uygulama bazında (Application Level) izleme modu açık olacaktır.

mostRecent
varsayılan değeri false olan bu niteliğe true değeri verilirse, requestLimit aşılması halinde gelen talepler son elde edilen taleplerin üzerine yazılır. Böylece son taleplerin görülebilmesi sağlanmış olur. Bu nitelik (attribute) Asp.Net 2.0 ile birlikte yeni gelmiştir.

traceMode
Daha öncedende değinildiği gibi, Trace Information kısmında yer alan bilgilerin, kategori veya süre bazlı sıralanıp sıralanmayacağını belirtir.

writeToDiagnosticTrace
Varsayılan değeri false olan bu özellik Asp.Net 2.0 ile birlikte gelmiş olup, Trace mesajlarının System.Diagnostics alt yapısına gönderilip gönderilmeyeceğini belirlemekte kullanılır.

Aşağıda örnek bir trace elementi içeriği yer almaktadır.

```xml
<?xml version="1.0"?>
<configuration>
    <appSettings/>
    <connectionStrings/>
    <system.web>
        <trace enabled="true" requestLimit="5" mostRecent="true" pageOutput="false" localOnly="true" writeToDiagnosticsTrace="true" />
        <compilation debug="true"/>
        <authentication mode="Windows"/>
    </system.web>
</configuration> 
```

Buna göre son 5 talebe ait trace bilgileri tutulacak, sayfa çıktısı verilmeyecek bir başka deyişle trace.axd ile talep edilebilecek, yanlızca host makinedeki kullanıcı trace.axd'ye bakabilecek ve bilgiler System.Diagnostics alt yapısına devredilebilecektir. Dolayısıyla uygulama çalıştırıldıktan sonra söz konusu izleme bilgileri aşağıdaki gibi olacaktır.

![mk216_13.gif](images/mk216_13.gif)

Burada sayfalara ait izleme detaylarına bakmak için View Details linki kullanılabilmektedir. Çıktıda yer alan bilgilere bakıldığında taleplerin zamanı (Time of Request), durumu-status code (örneğin talep edilen sayfa başarılı bir şekilde yüklendiyse 200, olmayan bir sayfa talep edildiyse 404...), HTTP'nin hangi metoduna göre (POST, GET...) talep edildiği (Verb) ve talep sırası (No) gibi bilgiler yer alır.

Geliştirilen web uygulaması dağıtılırken trace.axd dosyasının hiç bir şekilde talep edilememesi sağlanabilir. Bunun için web.config dosyasında yer alan system.web elementi altında aşağıdaki değişikliği yapmak yeterlidir.

```xml
<system.web>
    <httpHandlers>
        <remove verb="*" path="trace.axd"/>
    </httpHandlers>
</system.web>
```

Buna göre söz konusu web.config dosyasının içeren uygulamada herhangibir şekilde Trace.axd dosyası talep edilirse aşağıdaki ekran görüntüsü elde edilecektir.

![mk216_14.gif](images/mk216_14.gif)

Gelelim izleme ile ilişkili diğer konulara. Bazı durumlarda kod içerisinde kullandığımız Trace ifadelerinin sadece istisnalar oluştuğunda tutulmasını isteyebiliriz. Buna ek olarak, trace içerisine atılacak istisna bilgilerinin sadece host makinedeki kullanıcıya gösterilmesi istenebilir. Trace bilgisini sadece yerel kullanıcıya göstermek için localOnly özelliği kullanılabilir. Ancak burada durum biraz daha farklıdır. Nitekim, trace basılmakta ama içeride istisna oluşması halinde gösterilen içerik sadece yerel kullanıcı için oluşturulmak istenmektedir. Bu vakkayı çözmek için Trace ifadelerinin catch blokları içerisinde ele alınacağı ortadadır. Diğer taraftan Write veya Warn metodlarının üçüncü parametreleri burada önemlidir. Nitekim üçüncü parametre oluşan istisna (Exception) referansını taşımaktadır. Exception tipini trace çıktısına vermek dışında, talepte bulunan kullanıcınında host makineden geldiğini tespit etmek gerekir. Bu nedenle aşağıdaki adımlar izlenebilir.(Olayın daha kolay anlaşılabilmesi için konu şema ile desteklenmiştir)

1. Request ile istemcinin Host adresi öğrenilir. Burada Request.UserHostAddress'den faydalanılabilir.

2. Elde edilen adresin 127.0.0.1 olup olmadığına bakılır. Öyleyse talep yerel makineden gelmiştir ve trace açılabilir.

3. Talep yerel makineden gelmiyorsa Requset.ServerVariables ("LOCALADDR") ile elde edilen ip adresi ve Request.UserHostAddress ile elde edilen istemci adresi karşılaştırılır. Bu yerel host adresinin 127.0.0.1' den farklı olmasına karşı alınan bir tedbirdir. Eğer eşitlerse talebin yine yerel makineden geldiği anlaşılabilir.

4. Eğer taleplerin yerelden geldiği anlaşıldıysa çıktı üretilir.

![mk216_15.gif](images/mk216_15.gif)

Kod tarafında ise örnek olarak aşağıdaki sayfa düşünülebilir. Burada bilinçli olarak bir istisna (Exception) oluşturulmaktadır.

```text
<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected bool TraceYazilsinmi()
    {
        string userHostAddress = Request.UserHostAddress;
        if (userHostAddress == "127.0.0.1")
            return true;
        else
        {
            string localAddress = Request.ServerVariables.GetValues("LOCAL_ADDR").ToString();
            if (localAddress == userHostAddress)
                return true;
            else
                return false; 
        }
    }

    protected void btnBaglantiAc_Click(object sender, EventArgs e)
    {
        SqlConnection conn = null; 
        try
        {
            conn = new SqlConnection("data source=.;database=" + txtVeritabani.Text + ";integrated security=SSPI");
            conn.Open();
            Response.Write("Bağlantı açıldı");
        }
        catch (Exception excp)
        {
            if (TraceYazilsinmi())
            {
                Trace.IsEnabled = true;
                Trace.Warn("Developer İçin Hata Bilgisi", "Bağlantı açılması sırasında hata oluştu", excp); 
            } 
        }
        finally
        {
            if (conn != null
                && conn.State == ConnectionState.Open)
                    conn.Close(); 
        }
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Veritabanı Adı : <asp:TextBox ID="txtVeritabani" runat="server" />
            <asp:Button ID="btnBaglantiAc" runat="server" Text="Bağlantı Aç" OnClick="btnBaglantiAc_Click" />
        </div>
    </form>
</body>
</html>
```

Uygulamada kullanıcı textbox kontrolünden girdiği veritabanı adı için bir bağlantı açmaya çalışmaktadır. Eğer bağlantının açılması sırasında bir hata oluşursa ve kullanıcı host makineden talepte bulunmuşsa Trace çıktısının etkinleştirilmesi ve istisna mesajının buraya yazdırılması sağlanır. Burada istemcinin Ip adresini tedarik edebilmek için HttpRequest sınıfının UserHostAddress özelliği kullanılır. LocalAddr değeri ilede sunucu değişkenlerinden (Server Variables) sunucu ip adresi elde edilmektedir. Nitekim sunucu ip adresi 127.0.0.1' den farklıda olabilir. O halde talep eden istemcinin ip adresi ile yerel ip adresinin eşit olup olmadığına da bakılmalıdır. Uygulamayı test edip, örneğin olmayan bir veritabanı adı girdiğimizde sonuç sayfası aşağıdaki gibi olacaktır.

![mk216_16.gif](images/mk216_16.gif)

Dikkat edilecek olursa istisna bilgisi detayaları ile birlikte Trace Information kısmında özel kategori adı ve mesajı ile birlikte görülmektedir. Bu tarz bir çalışma zamanı bilgisi elbetteki geliştirici (Developer) açısından önemlidir. Aynı sonuçlar çok doğal olarak Trace mekanizması olmadan da tespit edilebilir. Buradaki temel amaç Trace mekanizmasını kullanarak, sayfalarda oluşabilecek hataların kesin yerlerini ve konumlarını daha kolay tespit edebilmektir.

Gelelim Trace mekanizması ile ilgili diğer bir konuya. Çok doğal olarak web uygulamalarında harici bileşenler (Components) kullanılır. Bileşenden kastımız çoğunlukla bir sınıf kütüphanesi (class library) veya ayrı bir sınıf dosyasıdır. Çok doğal olarak bu bileşenler içerisindeki bazı süreçler Trace mekanizması içerisinde ele alınmak istenebilir. Örneğin ilk uygulamada gerçekleştirdiğimiz string birleştirme işleminin ayrı bir sınıf içerisinde bir metod olarak ele alındığını düşünelim.

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Data.SqlClient;

public class VeriBileseni
{
    public string GetTransactionIdString()
    {
        HttpContext ctx = HttpContext.Current;
        StringBuilder builder = new StringBuilder();
        using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
        {
            SqlCommand cmd = new SqlCommand("Select Top 10000 TransactionId From Production.TransactionHistory", conn);
            conn.Open();
            ctx.Trace.Warn("TransactionId Cekme", "TransactionId değerleri çekilmeye başlanacak");
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                builder.Append(reader["TransactionId"].ToString());
                builder.Append("|");
            }
            ctx.Trace.Warn("TransactionId Cekme", "TransactionId değerleri çekildi...");
            reader.Close();
        }
        return builder.ToString();
    }
    public VeriBileseni()
    {
    }
}
```

Burada dikkat edilmesi gereken en önemli nokta Trace sınıfını kullanmak için HttpContext nesnesinin nasıl elde edildiğidir. Bileşenlerde Trace bilgisi ele alınıyorsa, bu bileşenin kullanıldığı HTTP içeriğinin kullanılması gerekmektedir. Bu nedenle HttpContext nesne örneği için statik (static) Current özelliğinden faydalanılmıştır. Böylece bileşeni o anda kullanan sayfa içeriğine ulaşılmış olunur. Bundan sonra ise Warn veya Write gibi metodlar kullanılabilir.

> Bileşenin ayrı bir sınıf kütüphanesi (class library) olarak tasarlanması durumunda,System.Web.dll assembly'ının referans edilmesi gerekecektir. Bunun ise doğuracağı önemli sonuçlardan birisi şudur; bilşenen içerisindeki izleme alt yapısı web bağımlı hale gelmektedir. Bu nedenle alternatif bir yaklaşım olarak ayrı bir dinleyici mekanizması özel olarak geliştirilebilir.

Peki söz konusu bileşen sadece web tabanlı kullanılmıyorsa. Bu durumda HttpContext gerekli işlevsellikleri sağlamak için uygun olmayacaktır. Dolayısıyla farklı bir yol izlemek gerekmektedir..Net Framework izleme mesajları için dinleyiciler (Listener) kullanır. İstenirse web.config aracılığıyla yada programatik olarak yeni dinleyiciler eklenebilir. Normal şartlarda Trace.Write gibi bir metod çağrısı yapıldığında TraceListener koleksiyonundaki tüm dinleyiciler mesajları alıp işlemeye başlarlar. O halde Trace Listener web.config ile açık bir şekilde belirtilirse, System.Web.dll assembly'ını projeye referans etmeden ve HttpContext tipini kullanmadan Trace çıktıları web uygulamasına doğru gönderilebilir.

Asp.Net 1.1 ile geliştirme yapıyorsak eğer, bu işlemler için özel bir dinleyici yazmamız gerekecektir. Ne varki Asp.Net 2.0 ile sadece bu iş için tasarlanmış WebPageTraceListener isimli bir sınıf gelmektedir. Bu sınıfın web.config dosyasında belirtilmesi ve bileşen içerisinde System.Diagnostics isim alanı altında yer alan Trace sınıfının kullanılması yeterlidir. Böylece bileşenimiz trace çıktısı verirken web'e bağımlı olmaktan kurtulmuş olacaktır. Örnek olarak az önce geliştirilen VeriBileseni sınıfını ayrı bir sınıf kütüphanesi olarak aşağıdaki gibi tasarladığımızı düşünelim.

![mk216_17.gif](images/mk216_17.gif)

```csharp
using System;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;

public class BagimsizVeriBileseni
{
    public string GetTransactionIdString()
    {
        StringBuilder builder = new StringBuilder();
        using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
        {
            SqlCommand cmd = new SqlCommand("Select Top 10000 TransactionId From Production.TransactionHistory", conn);
            conn.Open();
            Trace.Write("TransactionId Cekme", "TransactionId değerleri çekilmeye başlanacak");
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                builder.Append(reader["TransactionId"].ToString());
                builder.Append("|");
            }
            Trace.Write("TransactionId Cekme", "TransactionId değerleri çekildi...");
            reader.Close();
        }
        return builder.ToString();
    }
}
```

Burada dikkat edilecek olursa herhangibir şekilde System.Web referansı kullanılmamaktadır. Bunun yerine System.Diagnostics isim alanı ve burada yer alan Trace sınıfı ele alınmaktadır. Bir önceki bileşenden farklı olarak Warn metodu kullanılamamaktadır. Bunun nedeni System.Diagnostics isim alanında yer alan Trace sınıfının Warn metodunun olmayışıdır. Diğer önemli noktalardan biriside, System.Diagnostics.Trace sınıfındaki Write metodu versiyonlarından belirli bir Exception nesne örneğininin fırlatılmasının mümkün olmayışıdır. Ancak istisna mesajı gönderilmesi sağlanabilir. Bu işlemin ardından web uygulamasına ait web.config dosyasında aşağıdaki değişiklikler yapılmalıdır.

```xml
<?xml version="1.0"?>
<configuration>
    .
    .
    .
    <system.diagnostics>
        <trace>
            <listeners>
                <add name="WebPageTraceListener" type="System.Web.WebPageTraceListener,System.Web,Version=2.0.3600.0,Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
            </listeners>
        </trace>
    </system.diagnostics>
    .
    .
    .
</configuration>
```

Burada listeners bilgisi eklenirken system.diagnostics isim elementi içerisindeki trace elementi kullanılmaktadır. add elementi içerisinde yer alan type kımsında WebPageTraceListener sınıfının tam adı (Qualified Name) belirtilmektedir. Bildiğiniz üzere Qualified Name'i oluşturan değerler tip adı, assembly adı, versiyon numarası, kültür ve publicKeyToken bilgisidir. Bu durumu test etmek için az önceki örnektekine benzer bir web sayfası aşağıdaki gibi geliştirilebilir.

```xml
<%@ Page Language="C#" Trace="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void btnCek_Click(object sender, EventArgs e)
    {
        BagimsizVeriBileseni veriBln = new BagimsizVeriBileseni();
        Session.Add("TransactionIDler", veriBln.GetTransactionIdString());
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnCek" runat="server" Text="Veriyi Çek" OnClick="btnCek_Click" />
        </div>
    </form>
</body>
</html>
```

Trace çıktısı ise aşağıdaki gibi olacaktır.

![mk216_18.gif](images/mk216_18.gif)

Trace mimarisi ile ilgili olaraktan, ele alınabilecek başka konularda bulunmaktadır. Örneğin Trace bilgilerini başka ortamlara aktarmak, mail gönderimi gerçekleştirilmesi gibi. Tatile çıktığım şu günlerde bu kadar bilginin yeterli olacağı kanısındayım. Dönüşte Trace mimarisinin ikinci makalesi ile devam edeceğiz. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde trace mimarisini tanımaya, gerekliliklerini vurgulamaya çalıştık. Sayfa seviyesinde ve uygulama seviyesinde trace işlemlerinin nasıl yapılacağını, sadece istisna oluştuğunda yanlız yerel makineye trace bilgisinin nasıl verileceğini, bileşen bazında trace'lerin kullanılmasını ve bileşenin web ortamından bağımsız olabilecek şekilde ele alınabilmesini incelemeye çalıştık. Nihayetinde bu uzun makaleyi buraya kadar sabırla okuduğunuz için teşekkür eder bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/EtkiliTraceveDebug.zip)
