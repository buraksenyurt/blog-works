---
layout: post
title: "WorldBank, OData ve ASP.Net Web API HttpClient Kullanımı"
date: 2012-05-14 06:30:00 +0300
description: "Microsoft önce servis odaklı yaklaşımını değiştirerek Windows Communication Foundation altyapısını duyurdu. Hemen ardından WCF hızla gelişti ve pek çok Microsoft ürününün servis uç noktalarında yerini almaya başladı. Derken WCF’ e Web programlama modeli(Web Programming Model) için destekler eklendi. Artık REST(Representational State Transefer) odaklı servisleri yayınlamak ve hatta kullanmak mümkün hale gelmeye başladı. Çok basit anlamda HTTP protokolünün GET,POST,PUT,DELETE gibi metodlarına göre hizmet verebilen ve bu nedenle bir proxy ihtiyacını ortadan kaldırıp platform bağımsızlığı getiren servis yayınlama modeline destek söz konusu idi. Söz konusu model daha da geliştirildi. Özellikle Code Plex tarafında WCF Rest Service API’ si duyuruldu ve programlama modeli Astoria kod adlı WCF Data Service’ lerinde çekirdek yapı taşı haline geldi."
categories:
  - aspnet-web-api
tags:
  - aspnet-web-api
  - csharp
  - dotnet
  - aspnet
  - aspnet-mvc
  - wcf
  - windows-forms
  - xml
  - rest
  - json
  - web-api
  - http
  - javascript
  - concurrency
  - generics
---
# WorldBank, OData ve ASP.Net Web API HttpClient Kullanımı
[![World_Bank](/assets/images/2012/World_Bank_thumb.jpg)](/assets/images/2012/World_Bank.jpg) Merhaba Arkadaşlar,

1999 yılında girdiğim yüksek lisans (MBA-Master of Business Administrator) programını tamamlarken, bitirme projemde “Türkiye’ nin Dünya Bankası borçlanmalarını” ele almaya çalışmıştım. Haliyle o dönemlerde ve geçmişte, ülkemizin Worldbank üzerinden yaptığı borçlanmalara ait istatistiki bilgilere oldukça fazla ihtiyacım vardı. O kütüphane bu kütüphane gezmek dışında, dünya bankası internet sitesinden yayınlanan istatistik bazlı raporları da değerlendirmeye alıyordum. Yaklaşık olarak 60 sayfalık bir döküman oluşturmayı başarmıştım. Sunumumu yaptım, vardığım sonuçları değerli hocalarım ile paylaştım

![Wink](/assets/images/2012/smiley-wink.gif)

Zaman hızla geçti tabi ki. Bir baktım Microsoft.Net teknolojileri ile uğraşıyor ve yazılım geliştirici olarak kariyerimi devam ettiriyorum. Zaman hızla geçiyordu geçmesine ancak ondan daha hızlı hareket etmek isteyen de bir teknoloji vardı ortada..Net Framework platformunun gelişmesine ayak uydurabilmek gerçekten zorlaşıyordu. Ama tabi hepimiz için ortaya kullanışlı ve vizyonumuzu geliştiren ürünler çıkarttıkları da bir gerçekti.

Bugüne baktığımızda pek çok dünya firmasının dışarıya açık kaynaklı veri sunarken (özellike OData-Open Data Protocol formatında), Microsoft’ un da bu veri sunumlarını etkin bir şekilde ele almamız için getirdiği yeniliklerini görmekteyiz. Şöyle bir kaç sene geriye gidelim dilerseniz

![Wink](/assets/images/2012/smiley-wink.gif)

Microsoft önce servis odaklı yaklaşımını değiştirerek Windows Communication Foundation altyapısını duyurdu. Hemen ardından WCF hızla gelişti ve pek çok Microsoft ürününün servis uç noktalarında yerini almaya başladı. Derken WCF’ e Web programlama modeli (Web Programming Model) için destekler eklendi. Artık REST (Representational State Transefer) odaklı servisleri yayınlamak ve hatta kullanmak mümkün hale gelmeye başladı. Çok basit anlamda HTTP protokolünün GET,POST,PUT,DELETE gibi metodlarına göre hizmet verebilen ve bu nedenle bir proxy ihtiyacını ortadan kaldırıp platform bağımsızlığı getiren servis yayınlama modeline destek söz konusu idi. Söz konusu model daha da geliştirildi. Özellikle Code Plex tarafında WCF Rest Service API’ si duyuruldu ve programlama modeli Astoria kod adlı WCF Data Service’ lerinde çekirdek yapı taşı haline geldi.

Söz konusu web programlama modeli odaklı WCF servisleri o kadar popüler olmaya başladı ki, onu Asp.Net MVC gibi data-centric uygulamalarda daha sık görmeye başladık. Hal böyle olunca WCF takımı, WCF Web API isimli bir kütüphaneyi kullanıma sundu. Şu an geldiğimiz noktada ise bu yapı isim değiştirerek (ASP.NET Web API) doğrudan ASP.NET MVC 4.0 Beta sürümüne entegre edildi. Artık.Net Framework 4.5 sürümünde ASP.NET Web API gömülü olarak gelecek ve 2012 sonundan itibaren WCF Web API’ ye olan destek kalkacak.

İster WCF Web API olsun, ister gündemimize yeni yeni giren ASP.NET Web API olsun, birbirlerinden ayrıldıkarı yönler olduğu kadar, ortak noktalarıda bulunmakta. Örneğin, OData, XML, JSON gibi çıktı üretimleri sunan REST servislerinin daha kolay kullanımı için geliştirilmiş olan HttpClient tipi

![Wink](/assets/images/2012/smiley-wink.gif)

Şimdi buraya kadar yazdıklarımızı bir toparlayalım. Dünya bankası artık verilerini OData formatında olacak şekilde dış dünyaya sunmakta. Bu anlamda Developer’ lar için bir web sayfaları bile bulunuyor (Developer’ lar için kaynak [http://data.worldbank.org/node/209](http://data.worldbank.org/node/209)) Bu sayfada söz konusu Worldbank Data API’ sinin nasıl kullanılacağı ve hatta URL bazlı sorguların nasıl gönderileceği anlatılmakta. Eee, elimizde bu servisin verisini kullanabilmek için ASP.NET Web API ile de gelen HttpClient tipi var. Daha ne bekliyoruz öyleyse

![Wink](/assets/images/2012/smiley-wink.gif)

Gelin bir kaç dünya bankası verisini.Net kodlarımız ile sorgulayalım.

Önce örnek bir kaç sorguyu tarayıcı uygulama üzerinden göndermeye çalışalım. Örneğin ilk 50 ülkenin bilgilerini çekelim. Bunun için aşağıdaki sorguyu kullanmamız yeterli olacaktır.

[http://api.worldbank.org/countries](http://api.worldbank.org/countries)

işte sonuç,

[![worldb_8](/assets/images/2012/worldb_8_thumb.png)](/assets/images/2012/worldb_8.png)

Görüldüğü üzere XML tabanlı olarak ilk 50 ülkenin bilgilerine ulaşmış durumdayız. page, perpage gibi nitelikleri de sorgulara parametre olarak katarak sayfalar arasında gezinebiliriz de (Örneğin 2nci sayfaya gitmeyi ve her sayfada 25 ülke göstermeyi bir deneyin ![Wink](/assets/images/2012/smiley-wink.gif))

Tabi istersek bu sorguyu JSON (JavaScript Object Notation) formatında da ele alabiliriz.

[http://api.worldbank.org/countries?format=json](http://api.worldbank.org/countries?format=json)

Bu durumda daha küçük boyutlu bir içeriğe ulaşmış oluruz.

[![worldb_7](/assets/images/2012/worldb_7_thumb.png)](/assets/images/2012/worldb_7.png)

Teori gördüğünüz gibi oldukça basit. HTTP Get metoduna göre gönderdiğimiz sorgular sonucunda XML veya JSON formatında içeriklere ulaşabiliyoruz. Peki bunu kod tarafında nasıl kullanabiliriz?

Bu amaçla örnek bir Solution üzerinden hareket edeceğiz. Solution içerisinde temel sorgu fonksiyonellikleri ile POCO (Plain Old CLR Object) tiplerini barındıran bir kütüphane ile bunu kullanan bir Windows Forms uygulaması olması yeterli olacaktır. Sınıf kütüphanesine NuGet aracı ile veya dışarıdan harici olarak ilgili referansları da eklememiz gerekmektedir. Bu referanslar aşağıdaki şekilde görüldüğü gibidir.

[![worldb_9](/assets/images/2012/worldb_9_thumb.png)](/assets/images/2012/worldb_9.png)

WorldbankLib isimli sınıf kütüphanemizin içeriğini aşağıdaki sınıf diagramı (Class Diagram) ve kod parçalarında olduğu gibi geliştirebiliriz.

[![worldb_1](/assets/images/2012/worldb_1_thumb.png)](/assets/images/2012/worldb_1.png)

Country sınıf;

```csharp
namespace WorldBankLib 
{ 
    /// <summary> 
    /// Ülke bilgilerini taşır 
    /// </summary> 
    public class Country 
    { 
        /// <summary> 
        /// ülkenin adı 
        /// </summary> 
        public string Name { get; set; } 
        /// <summary> 
        /// ülkenin başkenti 
        /// </summary> 
        public string CapitalCity { get; set; } 
        /// <summary> 
        /// Enlem 
        /// </summary> 
        public string Latitude { get; set; } 
        /// <summary> 
        /// Boylam 
        /// </summary> 
        public string Longtitude { get; set; }

        public override string ToString() 
        { 
            return string.Format("{0}\t\t{1}\t\t({2};{3})" 
                , Name 
                , CapitalCity 
                , Latitude 
                , Longtitude 
                ); 
        } 
    } 
}
```

CountryIncome sınıfı;

```csharp
namespace WorldBankLib 
{ 
    /// <summary> 
    /// Gelir düzeyi bilgilerini içerir 
    /// </summary> 
    public class CountryIncome 
    { 
        /// <summary> 
        /// Ülkenin adı 
        /// </summary> 
        public string Name { get; set; } 
        /// <summary> 
        /// Gelir düzeyi seviyesi 
        /// </summary> 
        public string IncomeLevel { get; set; } 
        /// <summary> 
        /// Bağlı bulunulan bölge 
        /// </summary> 
        public string Region { get; set; } 
    } 
}
```

Topic sınıfı;

```csharp
namespace WorldBankLib 
{ 
    /// <summary> 
    /// Topic bilgilerini verir 
    /// </summary> 
    public class Topic 
    { 
        /// <summary> 
        /// Topiğin adı 
        /// </summary> 
        public string Name { get; set; } 
        /// <summary> 
        /// Kaynak bilgisi(Education gibi) 
        /// </summary> 
        public string Source { get; set; } 
        /// <summary> 
        /// Kaynağa ait bir açıklama bilgisi 
        /// </summary> 
        public string SourceNote { get; set; } 
    } 
}
```

ve asıl fonksiyonellikleri üstlenen CommonQueries sınıfı;

```csharp
using System.Collections.Generic; 
using System.Json; 
using System.Net.Http;

namespace WorldBankLib 
{ 
    /// <summary> 
    /// Worldbank OData servisi için genel sorguları içerir 
    /// </summary> 
    public static class CommonQueries 
    { 
        #region Temel sorgularımız

        // İlk 50 ülkenin bilgilerini JSON formatında verir 
        private static string _first50Countries = "http://api.worldbank.org/countries?format=json"; 
        //{0} yerine gelen ülkenin bilgisini JSON formatında verir. Bu bilgi BR gibi ülke kodu şeklindedir 
        private static string _specificCountry = "http://api.worldbank.org/countries/{0}?format=json"; 
        //ilk 50 ülkenin gelir düzeyi durumları JSON formatında verilir 
        private static string _incomeLevels = "http://api.worldbank.org/countries?incomeLevels&format=json"; 
        // {0} yerine(Örneğin Education için 4) gelen konuya ait olacak şekilde bazı topic bilgileri json formatında çekilir. 
        private static string _Topic = "http://api.worldbank.org/topics/{0}/indicators?format=json";

        #endregion

        /// <summary> 
        /// Http Get taleplerini göndermemizde devreye giren yardımcı tipimiz 
        /// </summary> 
        private static HttpClient _client = new HttpClient();

        /// <summary> 
        /// İlk 50 ülkenin ad, başkent, enlem ve boylam bilgilerini bulur 
        /// </summary> 
        /// <returns>ülke listesi döner</returns> 
        public static List<Country> GetFirst50Countries() 
        { 
            List<Country> countries = new List<Country>();

            _client.GetAsync(_first50Countries) 
                .ContinueWith((r) => 
                { 
                    HttpResponseMessage responseMessage = r.Result; 
                    responseMessage.EnsureSuccessStatusCode();

                    responseMessage.Content.ReadAsAsync<JsonArray>(). 
                        ContinueWith((rt) => 
                                        { 
                                            foreach (var country in rt.Result[1]) 
                                            { 
                                                countries.Add( 
                                                    new Country() 
                                                        { 
                                                            Name = country.Value["name"].ToString(), 
                                                            CapitalCity =country.Value["capitalCity"].ToString(), 
                                                            Latitude =country.Value["latitude"].ToString(), 
                                                            Longtitude =country.Value["longitude"].ToString() 
                                                        } 
                                                    ); 
                                            } 
                                        }); 
                }).Wait(); 
            return countries; 
        }

        /// <summary> 
        /// Belirli bir ülkeye ait isim, başkent, enlem ve boylam bilgilerini verir 
        /// </summary> 
        /// <param name="countryCode">Ülke kodu(brazilya için BR gibi)</param> 
        /// <returns>ülke bilgisi</returns> 
        public static Country GetCountry(string countryCode) 
        { 
            Country resultCountry=null;

            _client.GetAsync(string.Format(_specificCountry,countryCode)) 
                .ContinueWith((r) => 
                { 
                    HttpResponseMessage responseMessage = r.Result; 
                    responseMessage.EnsureSuccessStatusCode();

                    responseMessage.Content.ReadAsAsync<JsonArray>(). 
                        ContinueWith((rt) => 
                                         { 
                                             var cntry = rt.Result[1]; 
                                             resultCountry = new Country() 
                                                           { 
                                                               Name = cntry[0]["name"].ToString(), 
                                                               CapitalCity = cntry[0]["capitalCity"].ToString(), 
                                                               Latitude = cntry[0]["latitude"].ToString(), 
                                                               Longtitude = cntry[0]["longitude"].ToString() 
                                                           }; 
                                         }); 
                }).Wait(); 
            return resultCountry; 
        }

        /// <summary> 
        /// İlk 50 ülkenin Gelir düzeyi bilgilerini döndürür 
        /// </summary> 
        /// <returns>Gelir düzeyi bilgileri</returns> 
        public static List<CountryIncome> GetCountryIncomeLevels() 
        { 
            List<CountryIncome> countries = new List<CountryIncome>();

            _client.GetAsync(_incomeLevels) 
                .ContinueWith((r) => 
                { 
                    HttpResponseMessage responseMessage = r.Result; 
                    responseMessage.EnsureSuccessStatusCode();

                    responseMessage.Content.ReadAsAsync<JsonArray>(). 
                        ContinueWith((rt) => 
                        { 
                            foreach (var country in rt.Result[1]) 
                            { 
                                countries.Add( 
                                    new CountryIncome() 
                                    { 
                                        Name = country.Value["name"].ToString(), 
                                        IncomeLevel = country.Value["incomeLevel"]["value"].ToString(), 
                                        Region = country.Value["region"]["value"].ToString() 
                                    } 
                                    ); 
                            } 
                        }); 
                }).Wait(); 
            return countries; 
        }

        /// <summary> 
        /// Topic(Education) bazlı rapor bilgilerini döndürür 
        /// </summary> 
        /// <param name="topicCode">Topic kodu(Education=4 gibi)</param> 
        /// <returns>Topic içeriği</returns> 
        public static List<Topic> GetTopics(string topicCode) 
        { 
            List<Topic> topics = new List<Topic>();

            _client.GetAsync(string.Format(_Topic,topicCode)) 
                .ContinueWith((r) => 
                { 
                    HttpResponseMessage responseMessage = r.Result; 
                    responseMessage.EnsureSuccessStatusCode();

                    responseMessage.Content.ReadAsAsync<JsonArray>(). 
                        ContinueWith((rt) => 
                        { 
                            foreach (var topic in rt.Result[1]) 
                            {

                                topics.Add( 
                                    new Topic() 
                                    { 
                                        Name = topic.Value["name"].ToString(), 
                                        Source = topic.Value["source"]["value"].ToString(), 
                                        SourceNote = topic.Value["sourceNote"].ToString() 
                                    } 
                                    ); 
                            } 
                        }); 
                }).Wait(); 
            return topics; 
        } 
    } 
}
```

CommonQueries sınıfı içerisinde HttpClient tipinin kullanıldığı çeşitli metodlar söz konusudur. Her metod Worldbank OData servisine doğru bir sorgu göndermekte, gelen içeriği JSON formatında olacak şekilde ele almaktadır.

Çok doğal olarak veri çekme işlemi uzun sürebilir. Bu nedenle HttpClient tipinin GetAsync metodundan yararlanılmaktadır. Bu metoda olan çağrı asenkron olarak verinin çekilmesi noktasında devreye girmektedir. Yanlız dikkat edilmesi gereken hususlardan birisi, bu metodları kullanan uygulamaların söz konusu asenkron çalışmaların sonucunu beklemeden sonlanmasını engellemeye çalışmaktır.

Bir Windows Forms uygulaması söz konusu olacağından, client’ ı uyarmak adına Wait metodundan yararlanılmış ve buradaki metodlardan ancak sonuçlar alındığında çıkılması garanti edilmiştir. Tabi Windows Forms tarafında bu duraksatmaların ekranı dondurması da engellenmeli ve kullanıcının eş zamanlı başka işlemler yapabilmesine de izin verilmelidir. Bunun için Forms tarafında BackgroundWorker kontrolünden yararlanıyor olacağız. Örnek Windows Forms tasarımımızı ve kod içeriğini aşağıdaki gibi geliştirebiliriz.

[![worldb_2](/assets/images/2012/worldb_2_thumb.png)](/assets/images/2012/worldb_2.png)

ve kod içeriğimiz

```csharp
using System; 
using System.Collections.Generic; 
using System.ComponentModel; 
using System.Windows.Forms; 
using WorldBankLib;

namespace WinClientApp 
{ 
    public partial class Form1 : Form 
    { 
        private List<Country> countries = null; 
        private Country brazil = null;

        public Form1() 
        { 
            InitializeComponent(); 
        }

        private void btnGetFirst50Countries_Click(object sender, EventArgs e) 
        { 
            bgwGetFirst50Countries.RunWorkerAsync(); 
        }

        private void bgwGetFirst50Countries_DoWork(object sender, DoWorkEventArgs e) 
        { 
            e.Result = CommonQueries.GetFirst50Countries(); 
        }

        private void bgwGetFirst50Countries_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) 
        { 
            if(!e.Cancelled || e.Error==null) 
                dgvResult.DataSource = (List<Country>)e.Result; 
        }

        private void bgwGetBrazil_DoWork(object sender, DoWorkEventArgs e) 
        { 
            e.Result = CommonQueries.GetCountry("br"); 
        }

        private void bgwGetBrazil_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) 
        { 
            if (!e.Cancelled || e.Error == null) 
            { 
                var country=e.Result as Country; 
                if(country!=null) 
                    MessageBox.Show(country.ToString()); 
            } 
        }

        private void btnGetBrazil_Click(object sender, EventArgs e) 
        { 
            bgwGetBrazil.RunWorkerAsync(); 
        }

        private void btnGetIncomeLevels_Click(object sender, EventArgs e) 
        { 
            bgwGetIncomeLevels.RunWorkerAsync(); 
        }

        private void bgwGetIncomeLevels_DoWork(object sender, DoWorkEventArgs e) 
        { 
            e.Result = CommonQueries.GetCountryIncomeLevels(); 
        }

        private void bgwGetIncomeLevels_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) 
        { 
            if (!e.Cancelled || e.Error == null) 
            { 
                dgvResult.DataSource=(List<CountryIncome>)e.Result; 
            } 
        }

        private void btnGetEducationTopics_Click(object sender, EventArgs e) 
        { 
            bgwGetEducationTopics.RunWorkerAsync(); 
        }

        private void bgwGetEducationTopics_DoWork(object sender, DoWorkEventArgs e) 
        { 
            e.Result = CommonQueries.GetTopics("4"); 
        }

        private void bgwGetEducationTopics_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) 
        { 
            if (!e.Cancelled || e.Error == null) 
            { 
                dgvResult.DataSource = (List<Topic>)e.Result; 
            } 
        } 
    } 
}
```

Dikkat edileceği üzere test düğmelerine basıldıkça ilgli BackgroundWorker nesne örneği çalıştırılmaktadır. Bu BackgroundWorker tiplerine ait DoWork ve RunWorkerCompleted olay metodlarında ise, çalıştırma ve işlem sonuçlarının ele alınması işlemleri sağlanmaktadır. Örneğimizi test ettiğimizde aşağıdakilere benzer sonuçlar elde ederiz.

İlk 50 ülke bilgisinin çekilmesi;

[![worldb_3](/assets/images/2012/worldb_3_thumb.png)](/assets/images/2012/worldb_3.png)

Brezilyaya ait bilginin çekilmesi sonucu;

[![worldb_6](/assets/images/2012/worldb_6_thumb.png)](/assets/images/2012/worldb_6.png)

Gelir düzeyi bilgilerinin elde edilmesi;

[![worldb_4](/assets/images/2012/worldb_4_thumb.png)](/assets/images/2012/worldb_4.png)

Eğitim odaklı verilerin elde edilmesi;

[![worldb_5](/assets/images/2012/worldb_5_thumb.png)](/assets/images/2012/worldb_5.png)

Gördüğünüz gibi gayet kolay

![Wink](/assets/images/2012/smiley-wink.gif)

Tabi örneğimizin pek çok noktasında hata bulunmaktadır. (Biraz acele ile yazdığımdan dolayı) Örneğin Exception Handling mekanizması eksiktir ve hatta DataGridView kontrolü bazı noktalarda index hatası vermektedir. Diğer yandan daha fazla sorgu da işin içerisine katılabilir. Ben kapıyı gösteren kişi olarak yazımı burada sonlandırıyorum

![Smile](/assets/images/2012/smiley-smile.gif)

Artık Worldbank verilerinin ASP.NET Web API (ve tabi WCF Web API) ile gelen HttpClient tipi yardımıyla ve özellikle JSON formatında nasıl ele alabileceğinizi öğrendiniz. Bunun geliştirmek tamamen sizlerin elinde. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WorldBank.zip (88,93 kb)](/assets/files/2012/WorldBank.zip)