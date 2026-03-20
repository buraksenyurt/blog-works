---
layout: post
title: "MemoryCache"
date: 2010-11-08 03:34:00 +0300
categories:
  - aspnet-4-0
tags:
  - aspnet-4-0
  - csharp
  - dotnet
  - aspnet
  - wpf
  - windows-forms
  - http
  - performance
  - caching
  - generics
  - visual-studio
  - datatable
  - dependency-management
---
Deadline…Benim gibi yazılım geliştirici olan pek coğumuzun sevmediği kelimelerin başında geldiğinden eminim. Ancak kaçınılmaz bir gerçek olduğunu da biliyoruz. Her şeye rağmen onunla yaşamak veya yaşamasını öğrenmek zorundayız.

[![blg238_Giris](/assets/images/2010/blg238_Giris_thumb.jpg)](/assets/images/2010/blg238_Giris.jpg)

Tabi Deadline’ lar her zaman için bir proje için söz konusu olmayabiliyorlar. Söz gelimi şu sıralar hazırlanmakta olduğum Microsoft Teknoloji Günler Akşam Sınıfı Asp.Net 4.0 eğitiminin 3 gün öncesi de benim için bir Deadline (Aslında ilke olarak her seminerin 3 gün öncesinden tam olarak hazır olmayı benimsemişimdir) Bu deadline zamanına hızla yaklaştığım şu günlerde uykusuz geceler ile hazırlanmaya devam ediyorum. Ne varki oldukça yoğun ve zorlu bir projenin de içerisinde yer almaktayım. Ama ne demişler “No Sacrifice No Victory” Bakalım bu gece ki konumuz neymiş.

Asp.Net tarafında performans tarafında göz önüne alınan kriterlerden birisi de ön bellekleme mekanizmalarının kullanılmasıdır. Özellikle nesne tabanlı ön bellekleme işlemlerinde Cache tipinden sıklıkla yararlanıldığını görürüz. Bu tip yardımıyla herhangibir nesne örneğinin, içeriği ile birlikte bellek üzerinde tamponlanması mümkündür. Bu kullanıma göre ön bellekleme seçeneklerini zaman bazlı olarak değerlendirebiliriz.(Absolute Expiration, Sliding Expiration). Hatta özellikle SQL tarafında, tablo bazlı bağımlılıklar oluşturabilir ve buna göre ön bellekte tutulma sürelerini tablodaki değişikliklere bağımlı hale getirebiliriz (SqlCacheDependency). Buna ilaveten dosya bazlı bağımlılıklar da oluşturmamız mümkündür (File Based Dependency).

Tabi günümüzde ve öncesinde ön bellekleme için kullanılabilecek farklı alt yapılar da bulunmaktadır. Söz gelimi Enterprise Library ile birlikte gelen Caching Application Block ve hatta dağıtık mimari stratejilerini barındıran ve Clustered ön bellekleme modelini sunan Velocity kod adlı Windows Server AppFabric Distributed Caching. Ancak ön bellekleme işlemlerinde Asp.Net tarafından sunulan built-in ön bellekleme motoru en popüler olanlarından birisidir diyebiliriz.

Lakin özellikle Windows Forms, WPF, Console, Class Library gibi web dışında kalan projeler için enteresan bir durum da söz konusudur. Bu proje çeşitlerinde de istenirse Asp.Net Cache tipi kullanılabilir. Tek yapılması gereken söz konusu projeye System.Web.dll assembly’ ının referans edilmesidir

![Confused smile](/assets/images/2010/wlEmoticon-confusedsmile_4.png)

Cümlenin sonundaki Confused Simle Emoticon’ u mutlaka dikkatinizi çekmiştir. Evet gerçekten de Asp.Net tarafının System.Web.Caching isim alanında (Namespace) yer alan Cache tipini örneğin bir Windows Forms uygulamasında kullanabiliriz. Ancak burada ters olan durum aslında Web tarafı için tasarlanmış ve o mimari alana ait olan bir assembly’ ının (System.Web.dll), konu ile pek alakası olmayan desktop tipinden bir uygulamaya referans edilmiş olmasıdır.

İşte bu genel sıkıntı nedeni ile.Net Framework 4.0 sürümünde gelen yeni bir in-memory Caching alt yapısı mevcuttur (In-Memory olsa da aslında özelleştirilebilir ve farklı provider’ lar ile farklı kaynaklara doğru ön bellekleme işlemleri yapılabilir). Bu yapı aynı zamanda Asp.Net 4.0 yenilikleri arasında değerlendirilmektedir. Buna göre Asp.Net’ in Caching stratejisi, web bağımlılığından çıkartılmakta ve tüm.Net uygulamalarının kullanabileceği bir model haline getirilmektedir. Bu sayede, içerisinde barındırdığı temel tipler yardımıyla web programcılarının aşina olduğu ön bellekleme teknikleri, web ortamı dışındaki uygulama çeşitleri tarafından da kullanılabilir.

Yeni alt yapı (Infrastructure) System.Runtime.Caching.dll assembly’ ı içerisinde yer almaktadır. Dilerseniz bu Cache tipinin kullanımını örnek bir uygulama üzerinden değerlendirmeye çalışalım. Bu amaçla basit bir Windows Forms uygulaması geliştiriyor olacağız. İlk olarak söz konusu uygulamaya aşağıdaki ekran görüntüsünde yer aldığı üzere System.Runtime.Caching assembly’ ını referans ederek işe başlayabiliriz.

[![blg238_Reference](/assets/images/2010/blg238_Reference_thumb.gif)](/assets/images/2010/blg238_Reference.gif)

Örnek uygulamamıza ait Windows Form tasarımı ise aşağıdaki gibi olabilir. Burada Add Cache Items isimli düğmeye basıldığında bazı nesne örneklerinin ön belleğe atılması işlemleri gerçekleştiriliyor olacaktır. Diğer taraftan Get Cache Items başlıklı düğmeye basıldığında ise, ön bellek üzerinde o anda durmakta olan nesne örneklerinin elde edilmesi işlemi gerçekleştirilecektir.

[![blg238_FormDesign](/assets/images/2010/blg238_FormDesign_thumb.gif)](/assets/images/2010/blg238_FormDesign.gif)

Windows Forms uygulamasına ait kod içeriğini ise aşağıdaki gibi geliştirebiliriz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Data; 
using System.Data.SqlClient; 
using System.IO; 
using System.Runtime.Caching; 
using System.Windows.Forms;

namespace NewCacheConcept 
{ 
    public partial class Form1 : Form 
    { 
        string filePath = Path.Combine(Environment.CurrentDirectory, "ASP_NET_4_and_Visual_Studio_2010_Web_Development_Overview.pdf"); 
        // In-Memory Caching için kullanılacak olan nesne örneklenir 
        MemoryCache mCache = MemoryCache.Default;

        public Form1() 
        { 
            InitializeComponent(); 
        }

        private void btnAddToCache_Click(object sender, EventArgs e) 
        { 
            // Dosya bazlı bir bağımlılık politikası üretimi 
            CacheItemPolicy policy = new CacheItemPolicy(); // Önce Policy tipi oluşturulur

            // Policy tipi için yeni bir dosya değişikliğini takip eden monitor nesnesi eklenir 
            HostFileChangeMonitor cMonitor = new HostFileChangeMonitor( 
                new List<string> 
                { 
                    filePath 
                } 
                ); 
            // Monitor örneğin ilkelere eklenir 
            policy.ChangeMonitors.Add(cMonitor); 
            // Ön belleğe PDF tipinden olan dosya bir byte[] dizisi şeklinde eklenir. İkinci parametre de dependency belirtilir 
            mCache.Add("Aspnet40", File.ReadAllBytes(filePath), policy);

            // Custom Type türevli bir örneğin ön belleğe eklenmesi 
            Person burak = new Person(); 
            burak.Name = "Burak Selim"; 
            burak.Surname = "Şenyurt"; 
            burak.Salary = 1000.25; 
            burak.BirthDate = new DateTime(1976, 12, 4); 
            mCache.Add("Person", burak, DateTimeOffset.Now.AddSeconds(30)); // Eklenme anından itibaren 30 saniye süreyle ön bellekte tutulacağı DateTimeOffset yardımıyla belirtilir

            // Ön belleğe DataTable türünden bir nesne örneğinin eklenmesi 
            DataTable table = new DataTable(); 
            using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI")) 
            { 
                SqlDataAdapter adapter = new SqlDataAdapter("Select * From Production.Product", conn); 
                adapter.Fill(table); 
            } 
            mCache.Add("Products", table, DateTimeOffset.Now.AddMinutes(2)); // DataTable içeriği eklenme anından itibaren iki dakika süreyle ön bellekte tutulacaktır 
        }

        private void btnGetCacheInfo_Click(object sender, EventArgs e) 
        { 
            GetDashboard(); 
        }

        private void GetDashboard() 
        { 
            // Fiziksel olarak ön bellek ile ilişkili bazı bilgilerin çekilmesi 
            lstHistories.Items.Add(String.Format("Cache için kullanılabilecek bellek oranı %{0} ", mCache.PhysicalMemoryLimit)); 
            lstHistories.Items.Add(String.Format("Cache için kullanılabilecek bellek miktarı {0}", mCache.CacheMemoryLimit)); 
            lstHistories.Items.Add(String.Format("Cache içerisindeki nesne sayısı {0}", mCache.GetCount())); 
        }

        private void btnGetCacheItem_Click(object sender, EventArgs e) 
        { 
            GetDashboard(); 
            // byte[] tipinden ön belleklenmiş olan PDF dosyasının elde edilişi 
            CacheItem cItem=mCache.GetCacheItem("Aspnet40"); 
            if (cItem != null) 
            { 
                byte[] cValue = cItem.Value as byte[]; 
                lstHistories.Items.Add(String.Format("Cache deki {0} key değerli nesnenin boyutu {1}", cItem.Key, cValue.Length)); 
            }

            // Person tipinden ön belleklenen nesnenin elde edilişi 
            CacheItem cItemPerson = mCache.GetCacheItem("Person"); 
            if (cItemPerson != null) 
            { 
                Person cachedPerson = cItemPerson.Value as Person; 
                lstHistories.Items.Add(String.Format("Cache deki {0} key değerli nesnenin Name değeri {1}", cItemPerson.Key, cachedPerson.Name)); 
            } 
            
            // DataTable tipinden ön belleklenen nesnenin elde edilişi 
            CacheItem cItemTable = mCache.GetCacheItem("Products"); 
            if (cItemTable != null) 
            { 
                DataTable table = cItemTable.Value as DataTable; 
                lstHistories.Items.Add(String.Format("Cache deki {0} key değerli nesnenin toplam satır sayısı {1}", cItemTable.Key, table.Rows.Count)); 
            }

            // Belirli key bilgilerine ait değerlerin çekilmesi 
            var values = mCache.GetValues(keys: new string[] { "Person", "Products" }); 
        } 
    } 
}
```

Olayın kalbi MemoryCache isimli nesne örneğidir. Bu nesne örneğinden yararlanılarak byte[], Person ve DataTable tipinden nesne örneklerinin ön belleğe atılması işlemleri gerçekleştirilmektedir. Özellikle PDF formatındaki dosya içeriğinin (ki içerisinde Asp.Net 4.0 ile gelen yenilikler anlatılmaktadır ![Winking smile](/assets/images/2010/wlEmoticon-winkingsmile_8.png)) byte[] dizisi olarak belleğe atılışı sırasında bir de ilke (Policy) uygulandığına dikkat edilmelidir. Öyleki bu ilkeye göre bir dosya bağımlılığı oluşturulmuştur. Gerçi PDF dosyasının değiştirilmesi pek söz konusu olmasa da önemli olan bağımlılığın nasıl yaratıldığıdır. Bu, tipik olarak dosya da değişiklik olması halinde ön bellekte duran nesnenin düşürülmesi anlamına gelmektedir. Yani web tarafından aşina olduğumuz File Dependency olayı

![Winking smile](/assets/images/2010/wlEmoticon-winkingsmile_8.png)

Person ve DataTable tipinden olan nesne örneklerinin ön bellekte tutulması ise kesin süre sonlu olarak bildirilmiştir (Absolute Time Expiration). Yani ön belleğe atılan nesne örnekleri belirlediğimiz süre kadar tutulacaklardır. Dikkat edilmesi gereken noktalardan bir diğeri de, uygulamanın çalıştığı makinenin ön bellekleme için kullanılabilecek alan değerlerinin yüzdesel ve miktarsal olarak elde edilebiliyor olmasıdır.

Şimdi örneğimizi çalıştırarak ilk testimizi yapalım.

[![blg238_Runtime1](/assets/images/2010/blg238_Runtime1_thumb.gif)](/assets/images/2010/blg238_Runtime1.gif)

Dikkat edileceği üzere ön belleğe atılan 3 nesne örneği söz konusudur ve bunlara ait bir takım bilgilerde elde edilebilmektedir. Söz gelimi PDF dosyasının boyutu, Person nesne örneğinin Name değeri ve DataTable içerisinde duran toplam satır sayısı gibi. Diğer yandan önem arz eden konulardan biriside Absolute Expiration sürelerinin dolması halinde ne olacağıdır. Söz gelimi Person nesne örneği ön belleğe atıldıktan sonra 30 saniye boyunca yaşayabilir. Eğer süre dolduktan sonra bellek üzerinde kalan nesnelere bakılırsa Person nesne örneğinin artık olmadığı rahatlıkla görülebilir.(Diğer yandan uygulama kapatıldığı takdirde ön belleğe atılan tüm nesnelerin geçerliliği ortadan kalkacaktır. Bir başka deyişle süreleri veya bağımlılıkları bozulmasa dahi, uygulama açık iken eklenen ön bellek nesnelerine, uygulama tekrar açıldığında erişilemeyecektir)

[![blg238_Runtime2](/assets/images/2010/blg238_Runtime2_thumb.gif)](/assets/images/2010/blg238_Runtime2.gif)

Şu an itibariyle ön bellekte 2 nesne örneği yer almaktadır ve Person tipine ait örnek bunların arasında değildir. Nitekim kendisi için belirlenen ön bellekte yaşama süresi aşılmıştır.

Aslında ön bellekte duran n sayıda nesne olduğunda bunlardan sadece belirli Key adlarına sahip olanlarının elde edilmesi de bazı senaryolarda işimize yarayabilir. Bu noktada MemoryCache nesne örneği üzerinden çağırılabilen GetValues metodu oldukça işe yaramaktadır. Aşağıdaki Debug zamanı ekran görüntüsünde bu nesne örneği içerisinde Person ve Products adları ile işaret edilen nesne örneklerine erişilebildiği işaret edilmektedir.

[![blg238_DebugView](/assets/images/2010/blg238_DebugView_thumb.gif)](/assets/images/2010/blg238_DebugView.gif)

Görüldüğü üzere MemoryCache nesne örneğinden yararlanarak Asp.Net tarafında sık kullanılan in-memory Caching tekniğini, System.Web.dll assembly’ ına bağımlı olmadan web harici herhangibir uygulamada ele alabilmekteyiz. MemoryCache nesnesinin kullanımı ile ilişkili olarak detaylı referans bilgisine [MSDN](http://msdn.microsoft.com/en-us/library/system.runtime.caching.memorycache.aspx) üzerinden ulaşabilirsiniz. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ASPNET40Ogreniyorum.rar (2,31 mb)](/assets/files/2010/ASPNET40Ogreniyorum.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde geliştirilmiş ve test edilmiştir]