---
title: "Derinlemesine Session Kullanımı - 2"
pubDate: 2005-01-07 22:00:00
categories:
  - Asp.Net
tags:
  - Asp.Net
  - C#
  - .NET
  - ADO.NET
  - SQL Server
  - XML
  - Performance
  - DataSet
---

# Derinlemesine Session Kullanımı - 2
Değerli Okurlarım, Merhabalar.

Bir önceki makalemizde hatırlayacağınız gibi, Session nesnelerinin kullanımını incelemeye başlamıştık. Bu makalemizde ise, Session nesnelerinin nerelerde saklanabildiğine değinmeye çalışacağız. Varsayılan olarak Session nesneleri In-Proc (işlem içi) modunda saklanırlar. Yani web sayfasının çalıştığı asp.net work process'in içinde, dolayısıyla bu işlerin çalıştığı web sunucularındaki bellek alanlarında tutulurlar. Bu özellikle Session nesnelerine bilgi yazma ve okumada önemli bir avantajdır.

Nitekim, erişim doğrudan ram üzerindeki bölgelere doğru olduğu için diğer bahsedeceğimiz modlara nazaran göreceli olarak oldukça hızlı bir erişim söz konusudur. Lakin, web sunucusunun başına bir şey gelmesi halinde (örneğin sunucunun bir anda restart olması gibi) bellekte tutulan tüm Session nesneleri bir anda kaybedilir. Bu da çok sayıda kullanıcının açtığı oturumlara ait bilgilerin tamamının kaybolması anlamına gelmektedir. Bunu basit bir örnek ile gösterebiliriz. Aşağıdaki web uygulmasında, Session nesnesine bir değer atanmaktadır. Session'ın time-out süresi varsayılan halinde (Yani 20 dakika olarak) bırakılmıştır.

default.aspx kodları ve Form'un ekran görüntüsü;

```csharp
private void Page_Load(object sender, System.EventArgs e)
{
    if(Session["Bilgi"]!=null)
    {
        Label1.Text=Session["Bilgi"].ToString();
    }
}
private void btnEkle_Click(object sender, System.EventArgs e)
{
    Session["Bilgi"]="Deneme";
    Label1.Text=Session["Bilgi"].ToString();
}
```

![mk112_1.gif](images/mk112_1.gif)

WebForm2.aspx kodları ve Form'un ekran görüntüsü;

```csharp
private void Page_Load(object sender, System.EventArgs e)
{
    if(Session["Bilgi"]!=null)
        Label1.Text=Session["Bilgi"].ToString();
}
```

![mk112_2.gif](images/mk112_2.gif)

Bu örneği çalıştırdığımızda, default.aspx sayfasında Ekle butonuna basarsak Session nesnemize Deneme bilgisi eklenecektir. Eğer WebForm2.aspx sayfasına geçersek, Session bilgisinin Label kontrolüne yazıldığını görürüz.

![mk112_3.gif](images/mk112_3.gif)

Şimdi web uygulamamıza ait Global.asax dosyasında herhangibir değişiklik yapıp uygulamamızı yeniden derleyelim. Ben örnek olarak pek bir anlam ifade etmeyen bir yorum satırı girdim ve uygulamayı yeniden derledim. Tabi bunu yaparken, web tarayıcımızda sayfalarımız açık halde bulunmalıdır.

```csharp
protected void Application_Start(Object sender, EventArgs e)
{
    //YORUM SATIRIDIR.
}
```

Şimdi linklerimize tıklayarak sayfalar arasında tekrar gezindiğimizde, henüz time-out süresi dolmamış olan Session nesnelerinin kaybedildiğini ve Label kontrollerinde Session nesnesine ait içeriğin yazmadığını görürüz. Kısacası, In-Proc modunda olan (Yani işlem içi - In Process) Session nesneleri kaybedilmiştir. Elbetteki Session nesnelerinin bu şekilde kaybolması dışında da oluşabilecek istisnalar vardır. Örneğin sunucunun istem dışı bir şekilde kapanması gibi.

Asp.Net ile birlikte durum yönetiminde (state management), Session nesnelerinin saklanabilmesi için iki teknik daha geliştirilmiştir. Bu teknikler yardımıyla, durum nesnelerinin yukarıdaki gibi nedenlerden ötürü kaybolmalarının önüne geçilebilmektedir. Bu tekniklerden bir tanesi Session nesnelerinin bir Sql Veritabanında tutulduğu SQLServer modu, diğeri ise Session nesnelerinin ASP.NET State Service Windows Servisinde tutulduğu StateServer modudur. İlk olarak SQLServer modunu inceleyeceğiz.

SQLServer modunda, Session nesnesine ait tüm bilgiler bir Sql Sunucusunda bu iş için özel olarak hazırlanmış bir veritabanında tutulmaktadırlar. Böylece, Session nesnelerine ait içerik, istenen süre kadar (aylarca bile olabilir) fiziki bir disk bölgesinde saklanabilmektedir. Bu ayrıca, veritabanının başka bir sunucuda konuşlandırılmasıyla, Web Çiftliklerinin (Web Farms) yapısına uygun bir oluşumada imkan tanır. Böylece, Web Sunucusunda oluşabilecek aksaklıklardan doğacak sorunlar Sql Sunucusunu etkilemeyecek, dolayısıyla Session'lar korunmuş olacaktır. Elbette bu sistemin de dezavantajı vardır.

![dikkat.gif](images/dikkat.gif)
Session nesnelerini ayrı bir sunucudaki veri tabanında tutmak her ne kadar güvenlik ve tutarlılık açısından yüksek performans sağlasada, bilgilere erişimin In-Proc moda göre daha yavaş olmasınada neden olur. Bu elbetteki verinin okunması veya yazılması için sürekli veritabanına doğru atılan turların bir sonucudur.

SQLServer modunda kullanılan ASPState isimli veritabanında Session nesnelerinin yazılma, silinme gibi işlemleri için kullanılan stored procedure'ler yer alır. Session nesnelerine ait asıl içerik ise tempdb isimli veritabanında yer alan tablolarda tutulmaktadır. Microsoft.NET Framework bu veritabanını ve içeriğini kurmak için gerekli Sql kodlarını içeren script dosyalarını içerir. Windows XP sistemlerinde bu dosyaya (InstallSqlState.sql) D:\WINDOWS\Microsoft.NET\Framework\v1.1.4322\ adresinden ulaşabilirsiniz. Bu sql script dosyasını Sql Query Analyzer ile çalıştırdığımızda, Sql Sunucusunda ASPState isimli bir veritabanı oluşturulduğunu görürüz. Ayrıca Session nesnelerini tutacak olan tablolarda tempdb veritabanı içerisine eklenirler.

![mk112_4.gif](images/mk112_4.gif)

Burada bizi asıl ilgilendiren kısım, tempdb veritabanında oluşturulan ASPStateTempSessions isimli tablodur. Nitekim bu tablo Session verilerini saklamak üzere kullanılmaktadır. Tablonun yapısı aşağıdaki şekilde görüldüğü gibidir.

![mk112_5.gif](images/mk112_5.gif)

Dikkat edecek olursanız, SessionId isimli alan tablonun Primary Key alanıdır. Bu alanda, web sunucu tarafından otomatik olarak oluşturulan ASP.NETSessionId değeri tutulmaktadır. Bunun dışında Session'ın yaratıldığı tarih, oturumun sona ereceği süre bilgisi gibi veriler dışında Session nesnesinin içeriğinin saklanacağı iki önemli alan daha vardır. Bunlar SessionItemShort ve SessionItemLong alanlarıdır. Her iki alandan hangisinin kullanılacağı, Session nesnesine atanan verinin büyüklüğüne göre belirlenmektedir. İşte bu noktada karşımıza önemli bir sorun çıkar.

![soru.gif](images/soru.gif)
Bir DataSet içeriğini Session nesnesine aktardığımızda, SessionItemShort veya SessionItemLong alanlarından hangisi kullanılırsa kullanılsın, Session nesnesinin içerdiği veri nasıl olurda tek bir alan içerisine sığdırılabilir?

İşte burada bir önceki makalemizde bahsettiğimiz gibi Session nesnesinin taşıyacağı verinin serileştirilebilir olması gerekliliği ortaya çıkmaktadır. Böylece ister binary olarak ister XML olarak DataSet nesnesinin içeriği serileştirilebilir ve tek bir alan içerisine yazılıp okunabilir. Bu elbetteki Session ile taşımak istediğimiz her nesne örneği için geçerli bir durumdur. (Örneğin kendi yazdığımız bir sınıf için.)

![dikkat.gif](images/dikkat.gif)
Out-of-Proc (İşlem dışı) modlarda, Session nesnesine atanan nesnelerin mutlaka serileştirilebilir (Serializable) olmaları gerekmektedir.

Şimdi basit olarak yukarıda işlediğimiz örneğimizde kullandığımız Session nesnesini, SQLServer modunda saklayalım. Varsayılan olarak, Session nesneleri In-Proc modda tutulduklarından web.config dosyasında yer alan sessionState boğumunun standart içeriği aşağıdaki gibidir.

```text
<sessionState 
    mode="InProc"
    stateConnectionString="tcpip=127.0.0.1:42424"
    sqlConnectionString="data source=127.0.0.1;Trusted_Connection=yes"
    cookieless="false" 
    timeout="20" 
/>
```

Mode özelliğinin değeri varsayılan olarak InProc'tur. Yani, Session nesneleri işlem içinde tutulmaktadır. Session bilgilerini veritabanına yazabilmek için sessionState boğumunu aşağıdaki gibi düzenlememiz yeterli olacaktır.

```text
<sessionState 
    mode="SQLServer"
    stateConnectionString="tcpip=127.0.0.1:42424"
    sqlConnectionString="data source=127.0.0.1;user id=sa;password=123456"
    cookieless="false" 
    timeout="20" 
/>
```

Burada dikkat etmemiz gereken en önemli nokta, sqlConnectionString özelliğinin aldığı bağlantı cümleciğidir. Bu özellikte, sql sunucusunun bulunduğu lokasyon data source ile belirtilmektedir. Varsayılan olarak Sql sunucusunun localhost üzerinde bulunduğu düşünüldüğünden bu değer 127.0.0.1 ip değerini alır. Ancak Web Çiftliği (Web-Farm) gibi sistemlerde eğer Sql Sunucusunun bulunduğu adres farklı ise data source değerini bu adrese göre ayarlamamız gerekecektir. Diğer taraftan, Ado.Net'te olduğu gibi bağlantının yapılacağı veritabanı adının burada belirtilmesine gerek yoktur. Bunun sebebi SQLServer modunda Session nesnelerinin nereye yazılacaklarının zaten belli olmasıdır. Bir diğer husus ise, mutlaka ve mutlaka bağlantıyı belli bir kullanıcı adı ve şifresi üzerinden yapmamızın güvenlik açısından daha sağlıklı olacağıdır.

Şunu da hatırlatmakta fayda var. InstallSqlState script'i her ne kadar session yönetimi için gerekli veritabanı düzenlemelerini yapsada, ASPNET kullanıcısına ASPState veritabanındaki stored procedure'leri çalıştırma ve tempdb içindeki ASPStateTempSessions ile ASPStateTempApplications tabloları için gerekli Select, Insert, Update, Delete komutlarını yürütebilme izinlerini vermemiz gerekiyor.

![mk112_6.gif](images/mk112_6.gif)

Aksi takdirde ASPNET kullancısının bu sp ve sql komutlarını çalıştırma yetkisi olmayacağından web uygulamamızda aşağıdakine benzer türden hata sayfaları ile karşılaşabiliriz.

![mk112_7.gif](images/mk112_7.gif)

Gerekli izinleride verdikten sonra artık uygulamamızı çalıştırabiliriz. Uygulamamızı çalıştırdığımız sırada eğer Sql Profiler ile arka planda olanları izlersek hemen bir sp'nin çalıştırıldığını ve sp'ye parametre olarak bir GUID'in aktarıldığını görürüz. Buradaki GUID, web sunucusu tarafından üretilen ASP.NETSessionId'den başka bir şey değildir. Session nesnesi ilk kez yaratıldığından ilgili tabloya INSERT işlemini uygulayan bir sp çalışmaktadır.

![mk112_8.gif](images/mk112_8.gif)

Tam bu noktada ASPStateTempSessions tablomuza bakacak olursak, yukarıdaki sp'ye aktarılan Id'değerini SessionId alanına almış yeni bir satır oluşturulduğunu görürüz.

![mk112_9.gif](images/mk112_9.gif)

Elbetteki bu noktada, Session'ımıza henüz bir bilgi aktarmadığımız için tabloda yer alan ilgili alanlara herhangibir bilgi yazılmamıştır. default.aspx sayfasında ekle butonuna basarsak, başka bir sp'nin bu kez var olan SessionId'li satırı güncellemek üzere çalıştırıldığını ve parametre olarakta encrypt edilmiş Session içeriğinin gönderildiğini görürüz.

![mk112_10.gif](images/mk112_10.gif)

Dolayısıyla Session içeriği tabloda yer alan ilgili satıra (ASP.NET_SessionId değerine sahip olan satır) yazılmış olacaktır. Eğer makalemizin başındaki örneğimizde yaptığımız gibi, Global.asax dosyasında bir değişiklik yapıp uygulamayı yeniden derleyip time-out süresinden önce Session'ları okumak istersek, Session'lara ait değerlerin kaybolmadığını kolayca tespit edebiliriz. Session'ların yaşam süreleri dolduğunda otomatik olarak silindiklerini biliyoruz. InstallSqlState script'i ayrıca zaman aşımına uğramış Session'ların otomatik olarak kaldırılması için gerekli bir job nesnesinide sql sunucusuna yükler. (Job nesnesinin çalışabilmesi için Sql Server Agent servisinin çalışıyor olması gerekmektedir.)

![mk112_11.gif](images/mk112_11.gif)

Şimdi aşağıdaki örneği inceleyelim. Bu örneğimizde, Session nesnesine bizim tanımladığımız bir nesne örneğini aktarıyoruz.

Personel isimli sınıfımız;

```csharp
public class Personel
{
    private string mAd;
    private string mSoyad;

    public string Ad
    {
        get
        {
            return mAd;
        }
        set
        {
            mAd=value;
        }
    }

    public string Soyad
    {
        get
        {
            return mSoyad;
        }
        set
        {
            mSoyad=value;
        }
    }

    public Personel(string ad,string soyad)
    {
        mAd=ad;
        mSoyad=soyad;
    }

    public Personel()
    {

    }
}
```

default.aspx;

```csharp
private Personel pOku;

private void Page_Load(object sender, System.EventArgs e)
{

    if(Session["Eleman"]!=null)
    {
        pOku=new Personel();
        pOku=(Personel)Session["Eleman"];
        Label1.Text=pOku.Ad+" "+pOku.Soyad;
    }
}

private void btnEkle_Click(object sender, System.EventArgs e)
{
    Personel p1=new Personel("Burak Selim","Şenyurt");
    Session["Eleman"]=p1;
}
```

WebForm2.aspx

```csharp
private Personel pOku;

private void Page_Load(object sender, System.EventArgs e)
{
    if(Session["Eleman"]!=null)
    {
        pOku=new Personel();
        pOku=(Personel)Session["Eleman"];
        Label1.Text=pOku.Ad+" "+pOku.Soyad;
    }
}
```

default.aspx sayfasında Session nesnemize Personel sınıfından p1 isimli nesne örneğimizi aktarıyorz. Her iki sayfada da Session nesnesinin içeriğini okurken, Personel sınıfından bir nesne örneğine açıkça bir dönüştürme işlemi yaptığımıza dikkat edelim. Çünkü Session nesnesi, kendisine atanan verileri object tipinde taşımaktadır. Şimdi default.aspx sayfamızı tarayıcı penceresinde açar ve Ekle butonuna basarsak aşağıdaki hata mesajını alırız.

![mk112_12.gif](images/mk112_12.gif)

Sorun gayet açıktır. Personel sınıfımızın serileştirilebilir bir nesne olması gerekmektedir. Bu nedenle Personel sınıfımıza Serializable niteliğini (Attribute) eklememiz gerekiyor.

```csharp
[Serializable]
public class Personel
{
   .
   .
   .
```

Şimdi Ekle butonuna tekrardan basarsak ve sayfalar arasında gezersek, Personel sınıfına ait nesne örneğinin başarılı bir şekilde taşındığını görürüz.

Session nesnelerini işlem dışından saklayabileceğimiz bir diğer seçenekte ASP.NET State Service isimli windows servisinin kullanılmasıdır. Bu kullanımda çoğunlukla, State Service başka bir sunucu üzerinde çalıştırılır ve diğer web sunucuları tarafından ortaklaşa kullanılır. Dolayısıyla, çalışan Asp.Net work processor'dan ayrı process'ler söz konusudur. Bu ayrı process'ler State Server üzerinde konuşlandırılır.

![mk112_13.gif](images/mk112_13.gif)

Session nesnelerini ASP.NET State Service ile kontrol edebilmek için öncelikle bu servisin çalıştırılması gerekir.

![mk112_14.gif](images/mk112_14.gif)

Servisin çalıştırılmasının ardından Web.config dosyasında da sessionState boğumunun özelliklerini aşağıdaki gibi değiştirmeliyiz.

```text
<sessionState 
    mode="StateServer"
    stateConnectionString="tcpip=127.0.0.1:42424"
    sqlConnectionString="data source=127.0.0.1;integrated security=SSPI"
    cookieless="false" 
    timeout="20" 
/>
```

StateServer modunda, State Server olarak kullanılacak sunucunun tcpip adresi ve ilgili port numarası stateConnectionString özelliğinde belirleniz. Biz burada local makineyi kullanıyoruz. Buradaki 42424 port numarası, ASP.NET State Service servisinin kullandığı varsayılan port numarasıdır. Dilersek bu numarayı değiştirmemiz mümkün. Bunun için,sistemdeki registery ayarlarına inmemiz gerekiyor. Hot Key Local Machine sekmesinde \System\CurrentControlSet\Services\aspnetstate\Parameters\ altındaki Port elemanının değerini değiştirmemiz yeterlidir.

![mk112_15.gif](images/mk112_15.gif)

Bu değişikliklerden sonra Session nesnelerini ASP.NET State Service'ın kontrolü altında tutulmak üzere kullanabiliriz. Bu servis yardımıyla tuttuğumuz Session nesneleri için de serileştirilebilme şartı aranmaktadır, bunuda hatırlatalım.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
