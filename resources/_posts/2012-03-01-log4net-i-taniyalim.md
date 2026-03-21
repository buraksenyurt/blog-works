---
layout: post
title: "Log4Net’ i Tanıyalım"
date: 2012-03-01 07:22:00 +0300
categories:
  - dotnet-framework-4-0
tags:
  - .net
  - csharp
  - cross-cutting
  - logging
  - concern
  - log4net
  - nlog
---
Herkesin kendine has bir parmak izi vardır. DNA gibi benzersizdir. Her ne kadar bazı ajanlı filmelerinde bu izler silinebilse de (belki de silinebilyordur):) Krimonoloji labaratuvarlarından tutunda, şirketlerdeki giriş kapılarına kadar pek çok noktada parmak izlerimiz devreye girer. Hatta günümüzde kullandığımız bilgisayarların çoğunun açılması için parmak izi kullanılabilmektedir.

[![910898_who_am_i_](/assets/images/2012/910898_who_am_i__thumb.jpg)](/assets/images/2012/910898_who_am_i_.jpg)


Parmak izinin sahibi, sistem içerisinde yaptığı hareketliliklere ait pek çok bilgi bırakır geriye. Ne zaman gelmiş, nerede durmuş, hangi eşyayı tutmuş, kaç saat çalışmış, bilgisayarını ne zaman açmış vs…Bu tip bilgiler bazı senaryolarda çok kritiktir ve önemli anlamlar ifade etmektedir.

Ancak izlenen sadece parmak izi sahip insanlar değildir. Zaman zaman sistemlerin ve onun parçası olan aktörlerin de (cihazlar veritabanları, uygulamalar örneğin) takip edilmesi ve toplanan bilgilerden yola çıkılarak, ya oluşan hataların giderilmesi ya da gelecek için gerekli yönün nasıl olacağına karar verilmesi aşamalarında da bilgi toplamak önem kazanır. Gelelim bu geniş evrenden bizim dünyamıza. Yani yazılıma.

Geliştirdiğimiz uygulama çözümlerde, programın herhangibir zaman diliminde neler yaptığının bilgisini tutmak, geriye dönük yapılan araştırmalarda, performans ölçümlerinde, bug’ ların ayıklanmasında veya bir sonraki iterasyon için gerekli backlog’ ların oluşturulmasında önemli bir unsurdur.

Benzer şekilde sistem içerisine dahil olan kullanıcıların veya sistemin parçası olan diğer aktörlerin, zaman dilimi içerisindeki hareketliliklerini de kayıt altına almamız önemlidir. Nitekim bu şekilde aktörlerin geriye dönük izlerine ulaşabilir, bazı yasal süreçlerdeki ispatlardan tutunda, ürünün sonraki versiyonları için gerekli kullanıcı deneyimi raporlarının çıkartılmasına kadar pek çok noktada faydalı verilere ulaşabiliriz. Burada loglama kavramı devreye girmekte olup bilginin toplanması noktasında önemli bir Concern olarak karşımıza çıkmaktadır.

Dolayısıyla loglama uygulamalar için hayati bir anlama sahiptir. Üstelik mimari açıdan baktığımızda, katmanlar arasında bir kıstas (Concern) olarak da göz çarpmaktadır. Söz gelimi çok basit anlamda SOA (Service Oriented Architecture) tabanlı bir mimari modeli düşündüğümüzde, pek çok katman arasında dikine ilişki taşıyan kıstaslardan birisi de Loglama parçasıdır ve Cross-Cutting düzleminde yer alan enstrümanlardan birisidir. Aşağıdaki şekilde bu durum özetlenmektedir.

[![l4n_1](/assets/images/2012/l4n_1_thumb.png)](/assets/images/2012/l4n_1.png)

Dikkat edileceği üzere Sunum (Presentation), servis (Service), iş (Business) ve veri (Data) katmanlarının tamamı, Cross-Cutting içerisinde yer alan bloklara erişebilmektedir. Logging bloğu dışında diğer Concern’ lerde söz konusudur elbette. Örneğin güvenlik, performans odaklı Caching, hata yönetimi vb…

Gelelim Loglama konusuna.

Yazılım evrenimizde Loglama amacıyla kullanılan pek çok yardımcı araç da bulunmaktadır. Bunların çoğu ücretsiz birer kütüphanedir ve kullanımları da son derece basittir. Söz gelimi Microsoft Enterprise Library ile birlikte gelen Caching Application Block, Log4Net, NLog ve diğerleri. Loglama işlemleri için kullanılan bloklar, Cross Cutting’ de yer alan diğerleri gibi Inversion Of Control prensibini başarılı bir şekilde uygulayan ve dolayısıyla Dependency Injection kavramını içeren yapılardır. Daha doğrusu bu şekilde tasarlanmaları çok daha doğrudur.

Genellikle loglama amacıyla kullanılan araçlar, loglama stratejilerini, uygulama yeniden derlenmeye gerek kalmadan çalışma zamanında değişiklikler yapılabilmesine olanak tanımaktadır. Burada çok doğal olarak uygulama harici konfigurasyon ayarlarının kullanımı söz konusudur. Özellikle XML tabanlı olarak tutulan ve bu sayede basit bir editör ile dahi düzenlenebilecek konfigurasyon dosyaları bulunmaktadır.

.Net Framwork tarafındaki masaüstü uygulamalar (Console, Windows, WPF, Windows Service vb) için app.config dosyası söz konusu iken, Web tabanlı uygulamalarda Web.config dosyası ön plana çıkmaktadır. Ancak çoğu loglama aracı, başka bir XML konfigurasyon dosyasının da App.config veya Web.config içerisinden işaret edilmesine izin vermektedir.

Loglama stratejisinin konfigurasyon dosyaları yardımıyla kolayca belirlenmesi bu işin ilk önemli adımlarından birisidir. Genellikle bu noktadan sonra, loglamanın hangi kaynağa doğru yapılacağına dair bir takım kararlar verilmeli ve uygun sınıf oluşumları sağlanmalıdır. Loglama popüler olarak metin tabanlı dosyalara düz yazı (Plaint Text – ki en az yer kaplayan ve performanslı olan çözümlerdendir) veya XML gibi formatlarda aktarılabilir. İstenirse bir veritabanı üzerinde ilişkisel anlamda tablolara yazılması da sağlanabilir. Dosya ve veritabanı sistemlerine ek olarak servis veya Windows Event sistemi odaklı çözümler de düşünülebilir. Yani log bilgilerinin bir servis aracılığıyla farklı ve çoğunlukla arkasında nasıl bir sistem olduğunu bilmediğimiz ortamlara aktarılması ya da Windows Event Log’ larda uygulama bazlı olacak şekilde ayrıştırılarak saklanması da söz konusudur.

Çok doğal olarak bu işleri üstlenmesi ve seçilen provider’ lara göre ilgili ortamlara log bilgilerini çalışma zamanında göndermesi gereken bir takım akıllı tiplerin olması gerekmektedir. Dolayısıyla ilgili loglama araçlarının bu tip desteğini sunduğunu da ifade edebiliriz.

Konfigurasyon ile loglama stratejisini belirlemek, uygun tiplerin devreye girerek bir veya daha fazla ortama log mesajı atılabilmesine olanak sağlamaktadır. Bundan sonraki adımda ise geliştiricinin uygun tiplere ait nesne örneklerini oluşturarak, uygulamanın ilgili ve doğru yerlerinde log atması yeterli olacaktır.

Tabi unutulmaması gereken bir husus da, her yerde her şeyi ve her zaman için loglamamak gerekliliğidir. Nitekim her şeyli loglamak bir süre sonra bir veri çöplüğünün oluşmasına ve bilginin ayrımının zorlaşmasına neden olmaktadır. Ayrıca performans açısından adım başı loglamanın maliyeti düşünülmelidir. Bazen sadece test senaryolarının işletildiği sırada kullanılan logların ürünün Production ortamına çıkmasında kaldırılması veya kullanılmaması öngörülebilir. Kısacası loglama stratejimizi belirlerken, neyi, ne zaman, hangi vaka da, ne şekilde loglayacağımıza da karar vermek gerekebilir.

Şimdi lafı fazla uzatmadan makalemizin asıl konusuna gelelim. Bu yazımızda ücretsiz olarak indirip kullanabileceğimiz loglama araçlarından birisi olan log4Net’ e değiniyor olacağız. [Bu adresten ücretsiz](http://logging.apache.org/log4net/download_log4net.cgi) olarak indirebileceğiniz ürün basit bir dll kütüphanesidir ve 3 önemli unsurun yerine getirilmesini beklemektedir. (Ben makaleyi yazdığım tarih itibariyle ürünün 1.2.11.0 versiyonunu değerlendiriyor olacağım)

[![l4n_2](/assets/images/2012/l4n_2_thumb.png)](/assets/images/2012/l4n_2.png)

Log4Net ürününü indirdiğimizde beraberinde oldukça büyük bir XML konfigurasyon dosyası da gelmektedir. Bu dosya içerisinde, yapılacak konfigurasyon ayarlarına ait tüm detaylı bilgiler yer alır. Log4Net’ de diğer pek çok loglama aracında olduğu gibi, loglanacak bilgilnin seviyelendirilmesini önerir. Bu anlamda Fatal, Error, Warn, Info, Debug, All ve Off gibi çeşitli seviyeler sunar. Konfigurasyon dosyasını oluşturmak son derece kolaydır. Örneğin aşağıdaki konfigurasyon içeriğinde veritabanına log mesajı yazacak şekilde yapılmış ayarlamalar vardır.

```csharp
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
  <configSections> 
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler,log4net, Version=1.2.10.0, Culture=neutral, PublicKeyToken=1b44e1d426115821" /> 
  </configSections> 
  <log4net> 
  <appender name="AdoNetAppender" type="log4net.Appender.AdoNetAppender"> 
  <bufferSize value="100" /> 
  <connectionType value="System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /> 
  <connectionString value="data source=.;initial catalog=SpeedyShop;integrated security=true;" /> 
  <commandText value="INSERT INTO ApplicationLog ([Date],[Thread],[Level],[Logger],[Message],[Exception]) VALUES (@log_date, @thread, @log_level, @logger, @message, @exception)" /> 
  <parameter> 
    <parameterName value="@log_date" /> 
    <dbType value="DateTime" /> 
    <layout type="log4net.Layout.RawTimeStampLayout" /> 
  </parameter> 
  <parameter> 
    <parameterName value="@thread" /> 
    <dbType value="String" /> 
    <size value="255" /> 
    <layout type="log4net.Layout.PatternLayout"> 
      <conversionPattern value="%thread" /> 
    </layout> 
  </parameter> 
  <parameter> 
    <parameterName value="@log_level" /> 
    <dbType value="String" /> 
    <size value="50" /> 
    <layout type="log4net.Layout.PatternLayout"> 
      <conversionPattern value="%level" /> 
    </layout> 
  </parameter> 
  <parameter> 
    <parameterName value="@logger" /> 
    <dbType value="String" /> 
    <size value="255" /> 
    <layout type="log4net.Layout.PatternLayout"> 
      <conversionPattern value="%logger" /> 
    </layout> 
  </parameter> 
  <parameter> 
    <parameterName value="@message" /> 
    <dbType value="String" /> 
    <size value="4000" /> 
    <layout type="log4net.Layout.PatternLayout"> 
      <conversionPattern value="%message" /> 
    </layout> 
  </parameter> 
  <parameter> 
    <parameterName value="@exception" /> 
    <dbType value="String" /> 
    <size value="2000" /> 
    <layout type="log4net.Layout.ExceptionLayout" /> 
  </parameter> 
</appender> 
  <root> 
    <level value="DEBUG"/> 
    <appender-ref ref="AdoNetAppender"/> 
  </root> 
  </log4net> 
</configuration>
```

Burada dikkat edileceği üzere Appender sekmesinde AdoNetAdppender isimli bir kısım tanımlanmıştır. Bu appender, connectionString bilgisine göre yerel sunucu üzerinde yer alan SpeedyShop isimli veritabanındaki Log tablosuna kayıt atacak şekilde tasarlanmıştır.

Sorgu cümlesine dikkat edilecek olarak @ harfi ile başlayan parametrelerinin değerleri % ile başlayan özel keyword’ lerdir. Örneğin, @loglevel için %level, uygulamanın thread bilgisine ait değer için %thread gibi özel keyword’ ler kullanılmaktadır. % ile başlayan anahtar kelimeler log4Net’ e özgü yapılmış tanımlamalardır. %date, %level, %exception, %newline, %identity, %method ve benzerleri gibi, uygulama ortamında otomatik olarak loglama katına veri taşıyan sabitlerde mevcuttur.

Yukarıdaki config dosyasında Appender isimli bir kavramdan bahsettik. Veritabanı odaklı çalışan dışında dosyaya yazma amacıyla kullanılabilen FileAppender veya özellikle Console tabanlı test projelerinde ekrana log bilgisi vermek için kullanılan ConsoleAppender gibi versiyonlar da mevcuttur.

[![l4n_3](/assets/images/2012/l4n_3_thumb.png)](/assets/images/2012/l4n_3.png)

Object Browser yardımıyla Log4Net kütüphanesine baktığımızda pek çok Appender tipinin tanımlanmış olduğunu görürüz. Udp, SMTP, ASP.Net Trace bazlı vb gibi pek çok Appender bulunmaktadır.

Peki bu tipleri nasıl devreye alacağız? Gelin basit bir Console uygulaması üzerinden ilerleyelim ve Log4Net için alışılageldiği üzere bir HelloWorld örneği geliştirelim. İlk olarak projemize Log4Net Assembly’ ının referans edilmesi gerekmektedir.

[![l4n_4](/assets/images/2012/l4n_4_thumb.png)](/assets/images/2012/l4n_4.png)

İkinci olarak uygun bir loglama stratejisi belirlemeli ve App.config dosyasında buna ait ayarlamalar yapılabilmelidir. Bu örneğimizde loglarımızı dosya bazlı olarak tutuyor olacağız. Bu sebepten dolayı FileAppender tipini ele alacağız. Dolayısıyla app.config dosyasının içeriğini aşağıdaki gibi geliştirebiliriz.

```xml
<?xml version="1.0"?> 
<configuration> 
  <configSections> 
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler,log4net, Version=1.2.10.0, Culture=neutral, PublicKeyToken=1b44e1d426115821"/> 
  </configSections> 
  <log4net> 
    <appender name="FileAppender" type="log4net.Appender.FileAppender"> 
      <file value="Logs.txt" /> 
      <appendToFile value="true" /> 
      <lockingModel type="log4net.Appender.FileAppender+MinimalLock" /> 
      <layout type="log4net.Layout.PatternLayout"> 
        <conversionpattern value="%date [%thread] %-5level  – %message%newline" /> 
      </layout> 
      <filter type="log4net.Filter.LevelRangeFilter"> 
        <levelMin value="INFO" /> 
        <levelMax value="FATAL" /> 
      </filter> 
    </appender> 
    <root> 
    <level value="DEBUG"/> 
    <appender-ref ref="FileAppender"/> 
  </root> 
  </log4net> 
<startup> 
   <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/> 
</startup> 
</configuration>
```

Loglanacak metinsel içerik layout boğumu içerisinde yer alan conversionPattern elementine ait value niteliğinde belirtilmektedir. Önce tarih bilgisi ve arkasından sırasıyla thread, log seviyesi, mesaj ve alt satıra geçme işlemleri uygulanmaktadır. Loglamayı yaparken minimum seviyemiz INFO dur. Maksimum ise FATAL olarak set edilmiştir. Bu basit ayarlamalardan sonra uygulamanın kod tarafına geçerek gerekli Setup işlemlerini yazabiliriz. İşte kodumuz.

```csharp
using System; 
using System.Data; 
using System.Data.SqlClient; 
using log4net;

namespace HelloLog4Net 
{ 
    class Program 
    { 
        static ILog log = log4net.LogManager.GetLogger(typeof(Program));

        static void Main(string[] args) 
        { 
            log4net.Config.BasicConfigurator.Configure();

            SqlConnection connection = null; 
            try 
            { 
                const string connectionString = @"data source=.\SQLEXPRESS;database=AdventureWorks;user id=sa;pwd=1234."; 
                log.Warn(String.Format("Bağlantı açılacak. Connection String :{0}",connectionString)); 
                connection=new SqlConnection(connectionString); 
                if(connection.State==ConnectionState.Closed) 
                    connection.Open();

                log.Info(String.Format("Bağlantı durumu : {0}",connection.State)); 
            } 
            catch (Exception excp) 
            { 
                log.Error(excp.Message); 
            } 
            finally 
            { 
                if (connection != null && connection.State == ConnectionState.Open) 
                { 
                    connection.Close(); 
                    log.Debug(String.Format("Finally bloğundayız. Bağlantı durumu {0}", connection.State)); 
                } 
            }

            log.Info("Program sonu"); 
        } 
    } 
}
```

Dikkat edileceği üzere loglama nesne örneği üretilirken LogManager tipinin GetLogger metodu kullanılmakta ve sonuç ILog arayüzü referansına taşınmaktadır. İşte size IoC örneği:) Kodun ilerleyen kısımlarında konfigurasyon ayarlarının okunması işlemi gerçekleştirilir ve sonrasında istenilen noktalardan çeşitli seviyelerde log mesajları, konfigurasyon dosyasında belirtildiği üzere ilgili dosyaya yazdırılır. Son olarak AssemblyInfo dosyasında yapmamız gereken basit bir nitelik (Attribute) bildirimi daha söz konusudur.

```text
… 
[assembly: AssemblyVersion("1.0.0.0")] 
[assembly: AssemblyFileVersion("1.0.0.0")] 
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
```

Uygulamayı çalıştırdığımızda aynı zamanda bir ekran çıktısı aldığımızı da görürüz. Varsayılan olarak ConsoleAppender’ da devrededir. Tabiki bunu kapatabilirsiniz.

[![l4n_5](/assets/images/2012/l4n_5_thumb.png)](/assets/images/2012/l4n_5.png)

Test dosyasının içeriği de aşağıdaki gibi olacaktır.

[![l4n_6](/assets/images/2012/l4n_6_thumb.png)](/assets/images/2012/l4n_6.png)

Görüldüğü üzere log4Net kullanılarak çok basit bir şekilde loglama işlemleri yapılabilmekte ve uygulamanın herhangibir noktasından feedback’ ler verilebilmektedir. Siz siz olun uygulamalarınızda loglama konusunu ihmal etmeyin. Nitekim hataları ayıklama, kullanıcı ve sistem gibi genel aktörlerin hareketliliklerini izleme ve backlog oluşturma noktasında hayati öneme sahip bir mevzudur. İlerleyen makalelerimizde Log4Net’ in farklı kullanımlarına da değinmeye çalışıyor olacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[HelloLog4Net.zip (2,28 mb)](/assets/files/2012/HelloLog4Net.zip)