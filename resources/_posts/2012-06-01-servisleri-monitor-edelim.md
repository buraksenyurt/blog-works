---
layout: post
title: "Servisleri Monitor Edelim"
date: 2012-06-01 01:12:00 +0300
categories:
  - wcf
tags:
  - wcf
  - csharp
  - dotnet
  - linq
  - windows-forms
  - xml
  - web-service
  - xml-web-services
  - http
  - iis
  - authentication
  - generics
  - testing
  - asmx
---
Banka gibi, pek çok farklı sistemin bir arada yer aldığı ve çalıştığı, çoğu zaman heterojen yapıda olan büyük çaplı çözümlerde, servislerin sıklıkla kullanıldığını görürüz. Çok basit bir operasyonel uygulama bile, çalışacağı veri kümesini sadece veritabanı kaynağı üzerinden değil, sistem içerisinde yer alan başka kanallardan da almak durumunda kalabilir. Tam tersi durumda söz konusudur. Gerçekleşen bir toplu işlemin içerisinde, akışın çeşitli noktalarında yine servisler devreye girerek diğer sistemlerin haberdar edilmesi de söz konusudur. İşte böyle durumlarda, sistem içerisindeki parçalar arasındaki entegrasyonun sağlanabilmesi amacıyla, servis bazlı çözümlere sıklıkla başvurulur.

[![504349_tft_screen_close-up_1](/assets/images/2012/504349_tft_screen_close-up_1_thumb.jpg)](/assets/images/2012/504349_tft_screen_close-up_1.jpg)


İşin içerisine servisler girdiğinde, bunların anlık durumunlarını izlemek, ayakta olup olmadıklarını görmek veya zaman içerisindeki hareketliliklerine bakarak istikrarlı yapılarının nasıl olduğunu analiz etmek isteyebiliriz. Pek tabi bunun için birden çok 3ncü parti tool olduğunu biliyoruz. Söz gelimi IIS tarafında Windows Server AppFabric aracından yararlanılarak son derece etkili ve gelişmiş izleme ve kontrol mekanizmaları gerçekleştirilebilmektedir. Lakin bazı bankacılık sistemlerinde teknoloji adaptasyonu beklendiği kadar hızlı değildir. Windows Server AppFabric gibi bir tool’ un geçişi, yıllarca XP üzerinde çalışan bankanın Windows 7’ ye geçişindeki gecikme kadar sancılı ve sıkıntılı olabilir. Hatta bankanın bir sonraki teknolojik yenilenme sürecinde ortada Windows Server AppFabric’ ten tamamen farklı bir ürün de bulunabilir

![Undecided](/assets/images/2012/smiley-undecided.gif)

Peki böyle bir durumda ne yapabiliriz?

Elbette kendi başımızın çaresine bakmamız gerekecektir. Bir başka deyişle Monitoring aracını kendi imkanlarımızla yazmayı düşünebiliriz. İşte bu yazımızda ASMX tabanlı servisleri hatta HTTP/HTTPS protokolüne göre çalışan WCF servislerini nasıl izleyebileceğimizi görmeye çalışacak ve bununla ilişkili çekirdek bir tool yazacağız. İlk soru şu;

“Bir servisin ayakta olduğunu anlamak için ne yapabiliriz?”

Bazı servisler kendi durumlarını belirli periyotlarda çeşitli monitoring araçlarına bildirmek üzere genişletilmiştir. Hatta bazılarının çalışma zamanı motoru bunu default olarak sunmaktadır. Bazı monitoring araçlarıda, listelerinde yer alan servisleri belirli periyotlarda veya yöneticinin karar verdiği zaman aralıklarında kontrol ederek hayatta olup olmadıklarını anlamaya çalışır. Biz bu yolu tercih ediyor olacağız. Teorimiz ise oldukça basit. Servis URL adresine bir talepte bulunacağız. Eğer bir Exception/Error dönmüyorsa bir başka deyişle bir response alabiliyorsak servisimizin ayakta olduğunu düşünebiliriz. Öyleyse ilk etapta bir URL adresi ile gelen sayfaya nasıl request atabiliriz buna bir bakalım. Bu amaçla Kernel isimli bir kütüphane (Class Library) projesi oluşturalım ve içerisine Analyst isimli bir sınıf ekleyelim.

[![analyst](/assets/images/2012/analyst_thumb.png)](/assets/images/2012/analyst.png)

```csharp
using System; 
using System.Text; 
using System.Collections.Generic; 
using System.Net;

namespace Kernel 
{ 
    public class Analyst 
    { 
        private Exception Poke(string serviceAddress) 
        { 
            Exception result = null;

            HttpWebRequest request = null; 
            HttpWebResponse response = null;

            try 
            { 
                request = (HttpWebRequest) WebRequest.Create(serviceAddress); 
                request.Timeout = 2000; 
                response = (HttpWebResponse) request.GetResponse(); 
            } 
            catch (Exception exception) 
            { 
                result = exception; 
            } 
            finally 
            { 
                if(response!=null) 
                    response.Close(); 
            }

            return result; 
        } 
    } 
}
```

Poke isimli metodumuz bir adres bilgisini alıp oluşan bir Exception var ise bunu geriye döndürmek üzere tasarlanmıştır. Eğer bir hata söz konusu değilse null değer dönecektir. Metodumuz içerisinde HttpWebRequest tipini kullanarak gelen adrese doğru bir Http talebinde bulunulmaktadır. Ardından bu request üzerinden belirtilen timeout süresi içerisinde bir response çekilip çekilemediğine bakılır. Eğer response geliyorsa söz konusu adresteki resource’ un ayakta olduğunu düşünebiliriz.

Şimdi yazmış olduğumuz bu metodu bir test edelim isterseniz. Bunun için Unit Test projesi oluşturarak ilerleyebiliriz. Örnek olarak aşağıdaki test metodları göz önüne alınabilir.

```csharp
/// <summary> 
/// Olmayan bir web adresi için test yapar 
///</summary> 
[TestMethod()] 
[DeploymentItem("Kernel.dll")] 
public void PokeTestFail() 
{ 
    Analyst_Accessor target = new Analyst_Accessor(); 
    string serviceAddress = "http://www.yok.com/yok.asmx"; 
    Exception actual; 
    actual = target.Poke(serviceAddress); 
    Assert.AreNotEqual(null, actual); 
}

/// <summary> 
/// Var olan bir adres için test yapar 
///</summary> 
[TestMethod()] 
[DeploymentItem("Kernel.dll")] 
public void PokeTestOk() 
{ 
    Analyst_Accessor target = new Analyst_Accessor(); 
    string serviceAddress = "http://www.w3schools.com/webservices/tempconvert.asmx"; 
    Exception actual; 
    actual = target.Poke(serviceAddress); 
    Assert.AreEqual(null, actual); 
}
```

İlk test metodumuzda servis adresi olarak olmayan bir URL bilgisi giriyoruz. Bu teste göre Poke metodundan bir Exception nesne örneğinin dönmesini beklemekteyiz. Diğer taraftan ikinci test metodu içerisinde [http://www.w3schools.com/webservices/tempconvert.asmx](http://www.w3schools.com/webservices/tempconvert.asmx) adresine bir talepte bulunuyoruz. Burada beklediğimiz ise herhangibir Exception’ ın dönmemesi. Bir başka deyişle Poke metodunun null değer döndürmesini bekliyoruz. Testlerimizi çalıştırdığımızda aşağıdaki sonuçları almış olmalıyız.

[![testreport](/assets/images/2012/testreport_thumb.png)](/assets/images/2012/testreport.png)

Sonuç itibariyle iki test de başarılı bir şekilde tamamlandı. Dolayısıyla bu çekirdek fonksiyonelliğimizin işe yarayacağını düşünebiliriz. Öyleyse artık uygulamamızın kalan kısmını geliştirmeye devam edelim.

Sistemsel olarak takip etmek istediğimiz servisler ve bu servislerin tarih içerisindeki yaşam durumlarını öğrenmek istiyoruz. Buna göre servislerimizin bilgisini ve her servisin tarih içerisindeki durum raporlarını tutacağımız bir yapı tasarlamamız gerekiyor. Aslında aşağıdaki tip modeli düşünüldüğünde söz konusu ilişkiyi bir ölçüde kurduğumuzu ifade edebiliriz.

[![modeldiagram](/assets/images/2012/modeldiagram_thumb.png)](/assets/images/2012/modeldiagram.png)

Source tipi içerisinde servise ait adres bilgisini, tipini saklıyor olacağız. Bununla birlikte bir servisin tarih içerisindeki hareketliliklerini de saklamak istediğimizden arada bire çok ilişkiyi kuracağımız ikinci bir tipimiz daha yer alıyor. SourceHistory tipi içerisinde ise, yapılan Poke işlemine ait bilgiler tutulmaktadır. Bu işlemin ne zaman yapıldığı, Exception var ise buna ait bilgi, hangi servisin kontrol ediliği ve servisin o anki durumu gibi bilgiler tutulmaktadır. Dikkat edileceği üzere iki tip arasında bir Association ilişkisi de kurulmuş durumdadır. Şimdi dilerseniz Analyst tipimizin geri kalan metodlarını yazmaya çalışalım. İlk olarak toplu bir servis listesi üzerinde işlem yapacak olan ana fonksiyonelliğimizi geliştirerek ilerleyebiliriz.

[![analyst2](/assets/images/2012/analyst2_thumb.png)](/assets/images/2012/analyst2.png)

```csharp
using System; 
using System.Text; 
using System.Collections.Generic; 
using System.Net;

namespace Kernel 
{ 
    public class Analyst 
    { 
        public List<Source> Sources { get; set; }

        public Dictionary<Source,SourceHistory> GetReport() 
        { 
            Dictionary<Source, SourceHistory> result = new Dictionary<Source, SourceHistory>(); 
            Exception exception = null;

            foreach (Source source in Sources) 
            { 
                exception = Poke(source.Address); 
                SourceHistory history = new SourceHistory 
                                            { 
                                                ServiceId=source.SourceId, 
                                                CheckDate=DateTime.Now, 
                                                CurrentServiceState = exception!=null?ServiceState.Dead:ServiceState.Live, 
                                                Exception=exception, 
                                                Source = source, 
                                                SourceHistoryId = Guid.NewGuid() 
                                            }; 
                result.Add(source,history); 
            }

            return result; 
        }

…
```

GetReport isimli metodumuz Analyst sınıfı içerisinde tanımlı olan Sources özelliğinin tüm içeriğini dolaşarak, her bir Source örneği için Poke metodunu çağırmakta ve sonuçları generic bir Dictionary koleksiyonu içerisinde toplamaktadır. Yazmış olduğumuz metodun işe yarayıp yaramadığını görmek için yine bir test metodunu kullanabiliriz. Aynen aşağıda olduğu gibi

![Wink](/assets/images/2012/smiley-wink.gif)

```csharp
/// <summary> 
/// Bir servis adres listesi için gerekli testi yapar 
///</summary> 
[TestMethod()] 
public void GetReportTestOk() 
{ 
    Analyst target = new Analyst(); 
    target.Sources = new List<Source> 
                        { 
                             new Source 
                                 { 
                                     Address = "http://www.yok.com/yok.svc", 
                                     Histories = null, 
                                     ServiceName = "Yok Servisi", 
                                     ServiceType = ServiceType.WcfService, 
                                     SourceId = 1 
                                 }, 
                             new Source 
                                 { 
                                     Address = "http://www.w3schools.com/webservices/tempconvert.asmx", 
                                     Histories = null, 
                                     ServiceName = "Temp Convert", 
                                     ServiceType = ServiceType.XmlWebService, 
                                     SourceId = 2 
                                 } 
                         }; 
    Dictionary<Source, SourceHistory> actual; 
    actual = target.GetReport(); 
    Assert.AreEqual(target.Sources.Count, actual.Keys.Count); 
}
```

Test metodumuzun çalışması sonrasında beklentimiz koleksiyon içerisinde yer alan her bir Source örneğine karşılık metoddan geriye bir karşılığının dönmesi. Bir başka deyişle GetReport metodunun dönüş referansına ait Count değeri ile Sources özelliğine atanan koleksiyonun Count değerlerinin eşit olmasını bekliyoruz. Testimizi çalıştırdığımızda geçiyor olmalıyız.

[![testreport2](/assets/images/2012/testreport2_thumb.png)](/assets/images/2012/testreport2.png)

Elimizde temel fonksiyonellikler mevcut gibi. Ama halen daha eksiklikler var. Söz gelimi Analyst tipimizin tutarlı veriler ile çalışması gerekiyor. Yazımızın başında belirttiğimiz üzere biz XML Web Servislerini ve HTTP tabanlı WCF servislerini kontrol etmeyi planlıyoruz. Bu durumda Http veya https ile başlamayan ve bunlara ek olarak svc veya asmx ile bitmeyen adresleri işleme katmamalıyız. Bunu koleksiyonu oluşturacağımız yerde yapacağımız bir kontrolle engelleyebileceğimiz gibi Analyst sınıfı içerisinde de ele alabiliriz. Dilerseniz ikinci seçeneği göz önüne alarak sınıf yapımızı biraz daha değiştirelim.

```csharp
using System; 
using System.Collections.Generic; 
using System.Net; 
using System.Text.RegularExpressions;

namespace Kernel 
{ 
    public class Analyst 
    { 
        //public List<Source> Sources { get; set; } 
        private List<Source> Sources = new List<Source>();

        public void AddSource(Source source) 
        { 
            Regex regex = new Regex(@"(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?"); 
            string address = source.Address; 
            if(regex.IsMatch(address)) 
                if(address.EndsWith(".asmx")||address.EndsWith(".svc")) 
                    Sources.Add(source); 
        }
```

İlk olarak dışarıdan erişilebilen Sources koleksiyonunu private hale getirdik. Nitekim eleman ekleme işlemi sırasında çalıştırılması gereken bir doğrulama işlemi söz konusu. Bu koleksiyona eleman ekleme adımını AddSource metoduna verdik. Bu metod içerisinde Regex ifadesinden yararlanarak, gelen Source örneğine ait address bilgisinin geçerli bir URL olup olmadığını kontrol etmekteyiz. Bu işlemin ardından iş kuralımıza göre adresin svc veya asmx uzantılı olup olmadığına bakıyoruz. Eğer bu kriterler sağlanıyorsa güncel Source nesne örneğinin ilgili koleksiyona eklenmesi sağlanıyor. Tabi yapmış olduğumuz bu değişiklik nedeni ile test metodumuzu da güncellememiz gerekecek. Keşke en başından düşünseymişiz değil mi?

![Embarassed](/assets/images/2012/smiley-embarassed.gif)

```csharp
/// <summary> 
/// Bir servis adres listesi için gerekli testi yapar 
///</summary> 
[TestMethod()] 
public void GetReportTestOk() 
{ 
    Analyst target = new Analyst(); 
    target.AddSource(

        new Source 
            { 
                Address = "http://www.yok.com/yok.sv", 
                Histories = null, 
                ServiceName = "Yok Servisi", 
                ServiceType = ServiceType.WcfService, 
                SourceId = 1 
            } 
        );

    target.AddSource(new Source 
                         { 
                             Address = "ftp://www.w3schools.com/webservices/tempconvert.asmx", 
                             Histories = null, 
                             ServiceName = "Temp Convert", 
                             ServiceType = ServiceType.XmlWebService, 
                             SourceId = 2 
                         } 
        ); 
    Dictionary<Source, SourceHistory> actual; 
    actual = target.GetReport(); 
    Assert.AreNotEqual(0, actual.Keys.Count); 
}
```

Dikkat edileceği üzere iki adreste istediğimiz normlara uygun değil. İlk adres sv ile biterken ikinci adresimiz ftp ile başlıyor. Buna göre test metodumuzun Fail etmesi gerekmektedir. Çünkü geçer şartımız Keys.Count değerinin 0 dan farklı olmasıdır. Buradaki senaryoda iki adreste geçersiz olduğundan koleksiyona eklenmeyecek ve eleman sayısı 0 olarak dönecektir.

[![testreport3](/assets/images/2012/testreport3_thumb.png)](/assets/images/2012/testreport3.png)

Artık Analyst sınıfımızı terk edip arayüz tarafına geçebiliriz. Windows Forms şablonu olarak tasarlayacağımız Monitor uygulaması farklı bir proje de olabilir. Nitekim Kernel isimli çekirdek kütüphanemiz herhangibir projeye referans edilerek kullanılabilir.

Form tasarımımız son derece sade. Sadece bir DataGridView kontrolü bulunmaktadır. Kod içeriğini ise aşağıdaki gibi geliştirmeyi düşünebiliriz.

```csharp
using System; 
using System.Configuration; 
using System.Linq; 
using System.Windows.Forms; 
using Kernel; 
using System.Linq;

namespace Monitor 
{ 
    public partial class frmMonitor : Form 
    { 
        private Analyst jackRyan = new Analyst();

        public frmMonitor() 
        { 
            InitializeComponent(); 
        }

        private void frmMonitor_Load(object sender, EventArgs e) 
        { 
            int interval = 10000; 
            Int32.TryParse(ConfigurationManager.AppSettings["Interval"], out interval);

            timer1.Enabled = false;

            LoadSamples();

           timer1.Enabled = true; 
        }

        private void LoadSamples() 
        { 
            jackRyan.AddSource( 
                new Source 
                    { 
                        Address = "http://www.w3schools.com/webservices/tempconvert.asmx", 
                        Histories = null, 
                        ServiceName = "Temp Convert", 
                        ServiceType = ServiceType.XmlWebService, 
                        SourceId = 1 
                    } 
                ); 
           jackRyan.AddSource( 
                new Source 
                    { 
                        Address = "http://www.w3schools.com/webservices/yok.asmx", 
                        Histories = null, 
                        ServiceName = "Empty Name", 
                        ServiceType = ServiceType.None, 
                        SourceId = 2 
                    } 
                ); 
            jackRyan.AddSource( 
                new Source 
                    { 
                        Address = "http://www.webservicex.net/stockquote.asmx", 
                        Histories = null, 
                        ServiceName = "Stock Quote Service", 
                        ServiceType = ServiceType.XmlWebService, 
                        SourceId = 3 
                    } 
                );

            jackRyan.AddSource( 
                new Source 
                    { 
                        Address = "http://www.webservicex.net/RealTimeMarketData.asmx", 
                        Histories = null, 
                        ServiceName = "Real Time Market Data Service", 
                        ServiceType = ServiceType.XmlWebService, 
                        SourceId = 4 
                    } 
                ); 
        }

        private void timer1_Tick(object sender, EventArgs e) 
        { 
            var report = jackRyan.GetReport().Select((s, h) => new 
                                                                  { 
                                                                       s.Key.ServiceName 
                                                                      ,s.Key.Address 
                                                                       ,s.Value.CheckDate 
                                                                       ,s.Value.CurrentServiceState 
                                                                       ,s.Value.Exception 
                                                                   }).ToList(); 
            grdReport.DataSource = report; 
        } 
    } 
}
```

Windows uygulamamızda bir Timer nesnesinden yararlanılmaktadır. Form ilk yüklenirken takip edilecek olan servis listesi yüklenir. Ardından config dosyasında belirtilen Interval değerine göre Timer nesne örneğinin Tick metodu belirli periyotlarda devreye girerek servislerin durum bilgisini çeker. Sonuçlar bir anonymous type içerisine örneklenip liste haline getirilerek DataGridView kontrolünde gösterilmektedir. Örneği çalıştırdığımızda 10 saniye de bir yapılan tetiklemeler sonucu, ilgili servislerin anlık bilgileri görüntülenecektir.

[![monitoring](/assets/images/2012/monitoring_thumb.png)](/assets/images/2012/monitoring.png)

Buraya kadar yaptıklarımızı düşündüğümüzde uygulamamızın eksik kalan pek çok kısmı olduğu fark edilmektedir. Söz gelim;

- Servis bilgilerinin aslında bir Repository ortamı üzerinden yükleniyor olması ve yine History bilgilerinin de bu ortama kayıt ediliyor olması çok daha doğru bir yaklaşım olacaktır. Burada bir SQL veritabanı gibi ilişkisel yapıyı veri üzerinde kurgulayabileceğimiz bir sistem son derece yararlı olabilir.
- Buna ek olarak Windows uygulamasının daha responsible olması için Timer tipi yerine BackgroundWorker kontrolünün kullanılması göz önüne alınabilir.
- Diğer yandan DataGridView kontrolü dışında daha farklı bir DashBoard görünümü tasarlanabilir. Örneğin grafiksel bir gösterim yapılabilir.

Bu tip kısımların geliştirilmesini siz değerli okurlarıma bırakıyorum. Hatta sıkı takipçilerim için bir ödev olsun. Ben belki bir sonraki makalede bu geliştirmeleri de ele alabilirim. Kim bilir

![Laughing](/assets/images/2012/smiley-laughing.gif)

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Wink](/assets/images/2012/smiley-wink.gif)

[Monitoring.zip (2,27 mb)](/assets/files/2012/Monitoring.zip)