---
layout: post
title: "Microsoft Distributed Cache(Velocity Project) - Hello World"
date: 2009-12-11 00:25:00 +0300
categories:
  - velocity-project
  - windows-server-appfabric
tags:
  - velocity
  - microsoft-distributed-cache
  - windows-server-appfabric
---
Bazen bir otomobilin çok hızlı gitmesi, yüksek süratlere çıktığında direksiyonunun titrememesi, yolda sağa sola savrulmaması, keskin virajlarda rahatlıkla tutunabilmesi, vitesler arasındaki geçişlerde torkunun mümkün olduğunca korunabilmesi, frenajlarda kontrolü sağlaması vb... istenir. Yarış otomobillerinde bu ve benzeri ihtiyaçlar için çok sık duyduğumuz tunning olarak isimlendirilen geliştirmeler yapılır. Bazen yarışın kategorisine göre neredeyse teknolojinin sınırlarını zorlayacak yeniliklere şahit olunur. Ama özetle aracın performansını arttıracak her yol, kuralları çerçevesinde mübahtır.

![blg95_Giris2.jpg](/assets/images/2009/blg95_Giris2.jpg)

Performansı arttırmak önemli bir kriterdir ve her zaman kolay bir şekilde sağlanamamaktadır. Bazen motor yağına konulan katkı maddeleri ile basit şekilde, bazen yarış pistindeki ıslak zemine göre takılan lastikler zor şekilde vb... Yine enteresan bir giriş olduğunun farkındayım ama bu günkü konumuz yazılımda performansı arttırmak adına kullanılan önemli tekniklerden birisi ile ilgilidir. Caching. Web uygulamalarında sıklıkla karşılaştığımız, Enterprise Library Caching Block sayesinde Web uygulaması sınırlarını aşarak diğer uygulamalarda daha kolay kullanılabilir hale gelmiş bu kavramın servis bazlı hale getirilebildiğini duysanız acaba ne düşünürdünüz

![Surprised](/assets/images/2009/smiley-surprised.gif)

İşte Microsoft'un uzun bir süre önce duyurduğu ve şu anda CTP 3 sürümü bulunan kod adı Velocity projesi...

Microsoft'un Velocity kod adlı projesi aslında Distributed Caching Service olarakta bilinmektedir. Projenin en büyük amacı, her çeşit veri içeriğinin ön belleklenerek saklanabilmesini sağlamaktır. Bu açıdan düşünüldüğünde Enterprise Library içerisindeki Caching bloğu veya Asp.Net içerisinde kullanılan Cache tekniklerinden bir farkı yokmuş gibi görünebilir. Hatta tüm bu modellerin ortak amacının, uygulamaların fiziksel veri bölgelerine tekrar tekrar gitmelerini engelleyip performansı arttırmak olduğu da aşikardır. Aslında aradaki tek fark Velocity projesinde bir servis anlayışının olması değildir. Farkı görmek için projenin detaylarına belikde daha yakından ve derinden bakmak gerekmektedir. Öncelikli olarak projenin ne gibi yetenekleri sunduğuna maddeler halined bir bakalım;

- Veritabanı veya Hard Disc üzerindeki herhangibir veri içeriğinin Cache'lenebilmesi
- Yüksek Ölçeklenebilirlik (High Scalability)
- Cache yönetiminin Windows Service tarafından sağlanması (Varsayılan olarak Network Service hesabı ile yüklenen bir Windows Service'inden bahsediyoruz)
- Clustering ile Cache üzerinde yük dağılımının dengelenmesinin (Load Balancing) otomatik yönetimi
- Cache içerisindeki veriye anahtar alanlar, farklı tanımlayıcılar yada name tag'leri ile erişilebilmesi
- Optimistic ve Pesimistic Concurrency modellerini desteklemesi
- Asp.Net Session nesnelerinin Distributed Cache üzerinden veritabanına yazılmadan tutulabilmesinin sağlanması
- TCP/IP bazlı servis alt yapısının kullanılmas (Dikkat; Firewall istisnaları oluşabilir)

vb...

Şimdi bu konu ile ilişkili bir örnek geliştirmeye çalışacağız. Ama öncesinde Cache yönetimini üstlenecek makineye Velocity projesini kurmamız gerekiyor. Ne varki bu kurulum her ne kadar Next-Next şeklinde görünse de dikkat edilmesi gereken bazı noktalar var. Kurulum ile ilişkili en önemli noktalardan birisi depolama yeri (Storage Location) ve tipi (Storage Type). Ben kendi kurulumumda ağ paylaşımı (Network Shared) yapılmış ve gerekli izinler (Permissions) tamamlanmış olan VelocityZone isimli bir klasörü kullanmayı planlıyorum. Bu belirleme işlemi sırasında sizlere sorun çıkartabilecek noktalar \\makineAdı\KlasorAdı isimli bir lokasyonun olmayışı (ki bunu kendimiz belirliyoruz) ve bu alanla ilişkili yeterli izinlerin (Permission) bulunmayışı olacaktır. Bu sorunlar aslında Test Connection düğmesine tıklandığında çıkan hata mesajları yardımıyla görülebilir. Eğer herhangibir sorun yoksa Cluster oluşturulması adımına geçilmektedir. Örnekte ben ClusterZoneA isimli ve Small tipinden bir Cluster oluşturdum.

![blg95_Setup.gif](/assets/images/2009/blg95_Setup.gif)

Small seçimi aslında arka planda 1 ile 4 arası Cache Server'ın kullanılabileceğini ifade etmektedir. Tahmin edileceği üzere örneği yerel makine üzerinden çalıştırdığımızdan burada tek bir Cache Server mevcuttur. Tam bu noktada aslında mimari modelden biraz bahsetmekte yarar olacağı kanısındayım. Aslında istemci için mantıksal olarak tek bir Cache birimi ile çalışmaktadır. Fakat birden fazla Cache Server olabilir ve bunlar arka planda farklı makinelerdeki Cache Service'leri (ki Windows Service'leridir) refere edebilir. Buda zaten Load Balancing kurulumu açısından önemlidir. Aynen aşağıdaki şekilde olduğu gibi.

![blg95_Architecture.gif](/assets/images/2009/blg95_Architecture.gif)

Birde mantıksal modele bakalım mı?

![Wink](/assets/images/2009/smiley-wink.gif)

Haydi bakalım.

![blg95_Logical.gif](/assets/images/2009/blg95_Logical.gif)

İlk kurulumnda varsayılan Cache mantığı kullanılır. Ancak istenirse farklı isimlendirmelere sahip birden fazla Cache alanı kullanılabilir. Özellikle uygulamalar kaç adet olursa olsun her biri için ayrı ayrı Cache isimlendirilmeleri kullanılabileceğini düşünebiliriz. Buna göre her farklı Cache birbirlerinden tamamen bağımsız olarak değerlendirilebilir. Örneğin Policy'leri ayrı ayrı belirlenebilir. Diğer yandan Regions bazlı mantıkta çalışma zamanında ayarlamalar yapılması gerekmektedir.

Bir başka deyişle Region'ların oluşturulması çalışma zamanında gerçekleştirilir. Region mantığına göre Cache içeriklerine Key'den farklı olarak Tag'ler yardımıyla (ki birden fazla Tag değerinin bir Cache nesnesi ile ilişkilendirebiliriz) ulaşılabilir. Burada Region bazlı bir ayrıştırma yapıldığından arama gibi işlemlerde Tag'lerden yararlanılabilir. Region mantığında Cache içerikleri tek bir host içerisinde tutulur. Yani diğer modellerdeki gibi servisler tarafından dağıtık olarak kullanılmazlar. Bu önemli bir farktır. Nitekim arama fonksiyonelliğinin avantajı ortaya çıkarken, Named mantığında olduğu gibi dağıtık kullanım söz konusu olmadığından ölçeklenebilirlik (Scalability) ortadan kalkmaktadır.

Kurulum işleminin sonrasında klasör içeriğinin aşağıdaki gibi şekillendiğini görebiliriz.

![blg95_FolderContent.gif](/assets/images/2009/blg95_FolderContent.gif)

Kurulum işlemi bu şekilde tamamlandıktan sonra programlardan (Microsoft Distributed Cache | Administration Tool - Microsoft Distributed Cache) ilgili Administration Tool aracınının kullanılarak Cluster'ın başlatılması gerekmektedir. Bu amaçla Administration Tool komut satırından Start-CacheCluster yazmamız yeterli olacaktır. Böylece ilgili Distributed Cache Service örneğinin vermiş olduğumuz kriterlere göre ilgili Port üzerinden çalıştırılması sağlanmış olur.

![blg95_StartCache.gif](/assets/images/2009/blg95_StartCache.gif)

Yukarıdaki ekran görüntüsünde Get-CacheHelp kullanımı sonucu yönetsel işlemler için hangi komutların kullanılması gerektiğide görülmektedir. Örneğin başlattığımız Cache Cluster hizmetini durdurmak için Stop-CacheCluster komutunu kullanmamız yeterlidir.

![blg95_StopCache.gif](/assets/images/2009/blg95_StopCache.gif)

Servis başarılı bir şekilde çalıştırıldığında bunu Services aracından da görebiliriz.

![blg95_Services.gif](/assets/images/2009/blg95_Services.gif)

Artık kurulumu yaptığımıza ve servisi çalıştırdığımızda göre örnek bir uygulama geliştirerek Velocity üzerinden Cache'lemenin nasıl yapılabileceğine bakabiliriz. Tabiki söz konusu uygulamanın Velocity servisini kullanabilmesi için gerekli bazı Assembly'ları referans etmesi gerektiği de ortadadır. Bu referanslar aslında kurulum sonrası Program Files\Microsoft Distributed Cache\V1.0 klasörüne atılacaktır.

![blg95_References.gif](/assets/images/2009/blg95_References.gif)

Şimdi Visual Studio 2010 Ultimate Beta 2 ortamında örnek bir Windows Forms uygulaması açalım ve ilgili Assembly'ları projemize referans edelim.

![blg95_ProjectReferences.gif](/assets/images/2009/blg95_ProjectReferences.gif)

Uygulamamızda Velocity projesini kullanabilmemiz için çalışma zamanına bazı konfigurasyon bilgilerinin bildiriminin yapılması gerekmektedir. Bu nedenle App.config dosyasının içeriğini aşağıdaki gibi değiştirerek devam edebiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="dataCacheClient" 
             type="Microsoft.Data.Caching.DataCacheClientSection, CacheBaseLibrary" 
             allowLocation="true" 
             allowDefinition="Everywhere"/>
  </configSections>
  <dataCacheClient deployment="simple">
    <localCache isEnabled="true" sync="TTLBased" ttlValue="300" />
    <hosts>
      <host name="bsenyurt" cachePort="22233" cacheHostName="DistributedCacheService"/>
    </hosts>
  </dataCacheClient>
</configuration>
```

Tahmin edileceği üzere uygulamanın Distributed Cache sistemini nasıl kullanacağına dair tüm konfigurasyon bilgileri buradaki ilgili elementler içerisinde bildirilmektedir. Söz gelimi host kısmında az önce başlatılan servise ait bilgiler yer almaktadır. Port bilgisi, host makinenin adı gibi. Yine localCache elementi içerisinde Cache üzerinde verinin tutulma süresinin belirlenmesi için ttlValue niteliğine saniye cinsinden bir değer atanmaktadır. Aynı element içerisinde yer alan sync niteliğine atanan TTLBased değeri ile zaman aşımlı bir Cache'leme kullanılacağı bildirilmektedir.

> Kişisel Not: Buradaki ayarları manuel olarak yapmamız bir dezavantaj olarak düşünülebilir. Malum Enterprise Libary içerisinde dahi bu tip ayarlar görsel olarak kolayca yapılabilmektedir. Üstelik App.config dosyası içerisinde intelli-sense özelliğide olmadığından şimdilik Copy-Paste kuralları ile ilerlemek zorunda kalmış durumdayız. Bu durumun Relase sürümde özellikle Visual Studio 2010 ile olan entegrasyonunda düzeleceğini düşünmek istiyorum.

Neyse biz kaldığımız yerden devam edelim. Windows Form'umuzu aşağıdaki gibi tasarladığımızı düşünelim.

![blg95_Form.gif](/assets/images/2009/blg95_Form.gif)

Aslında Hello World uygulamamızın amacı çok basit. Cache'e veri ekleyebilmek, var olanı çekebilmek ve güncelleyebilmek. String tipinden bir veri içeriğini saklıyor olacağız. Ancak tabiki kullanıcı tanımlı tiplerin de saklanabileceğini bir kere daha hatırlatalım. Uygulama kodlarmızı aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Windows.Forms;
using Microsoft.Data.Caching;

namespace HelloVelocity
{
    public partial class Form1 : Form
    {
        // DataCache nesnesi tanımlanır
        DataCache dCache = null;

        public Form1()
        {
            InitializeComponent();

            DataCacheFactory factory = new DataCacheFactory();
            //Fabrika nesne örneğinin GetDefaultCache metodundan yararlanılarak varsayılan DataCache nesne referansı elde edilir.
            dCache = factory.GetDefaultCache();        
        }

        // Cache' den veri çekme işlemi
        private void btnGetFromCache_Click(object sender, EventArgs e)
        {
            // Get metodu parametre olarak Cache' de duran nesnenin Key özelliğini alır. Örneğimizde Key özelliğinin değeri City' dir
            string cacheContent = dCache.Get("City") as string;

            if (!String.IsNullOrEmpty(cacheContent))
            {
                // Eğer cacheContext içeriği bol veya null değilse TextBox kontrolüne aktarılır
                txtCities.Text = cacheContent;
            }
        }

        // Cache' e veri ekleme işlemi için kullanılır
        private void btnSaveToCache_Click(object sender, EventArgs e)
        {
            try
            {
                // Add metodunun ilk parametresi Cache' de tutulacak veriyi işaret eden Key değeridir. İkinci parametre ile Cache' de duracak veri içeriği belirtilir.
                dCache.Add("City", txtCities.Text);
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message);
            }
        }

        // Cache' de zaten var olan bir veri içeriğinin güncellemek için kullanılır.
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // İlk parametre Cache' de duran nesnenin Key değeridir. İkinci parametre ise yeni halidir.
                dCache.Put("City", txtCities.Text);
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message);
            }
        }
    }
}
```

Aslında DataCache nesnesi Cache ile ilişkili tüm yönetsel işlemleri üstlenmektedir. Add, Put ve Get metodları sırasıyla Cache'e veri ekleme, Cache'deki veriyi güncelleme ve Cache'den veri çekme işlemleri için kullanılmaktadır. Tüm bu metodlar mutlaka Key ve Value değerlerine ihtiyaç duymaktadır. Yani Cache'de duran veriyi işaret eden bir anahtar ile içerik bilgisi. Örnekte Cache üzerinde String bir içerik tutulmaktadır. Ancak karmaşık tiplerin tutulmasıda mümkündür. Söz gelimi geliştirici tarafından tanımlanmış bir nesne içeriğide Cache içerisinde tutulabilir. Nitekim özellikle Get metodunun dönüş tipine bakıldığında object olduğu görülmektedir. Buda zaten herşeyi açıklamaktadır.

![Wink](/assets/images/2009/smiley-wink.gif)

Peki örneğimizi nasıl test edeceğiz? İşte örnek senaryo adımları;

TextBox içerisine bir şehir adı (Örneğin İstanbul) girip Kaydet tuşuna basınız.
Programı kapatınız ve tekrardan çalıştırıp Getir tuşuna basınız.
Eğer Getir tuşuna bastıktan sonra 1nci adımda girdiğiniz Şehir adını TextBox içerisinde görüyorsanız sonraki adımdan devam ediniz.
TextBox içeriğinde değişiklik yapınız. Örneğin farklı bir şehir adı giriniz ve bu kez Güncelle tuşuna basınız.
Programı yine kapatıp açınız ve Getir tuşuna basarak 4ncü adımda yaptığınız güncellemenin getirildiğinden emin olunuz.
Şehir adını değiştirip veya aynı bırakıp tekrardan Ekle tuşuna basınız.
6ncı adımdan sonra "ErrorCode:Cache::Add: An attempt is being made to create a object with a Key that already exists in the cache.Cache will only accept unique key value for objects." içerikli bir hata mesajı aldığınızdan emin olunuz (Nitekim tekrar kaydetmek istediğiniz Key değeri zaten mevcuttur)
Uygulamayı kapatınız ve bir kaç dakika sonra (tam olarak 300 saniye=5 dakika) tekrar başlatıp Getir tuşuna basınız.
8nci adımdan sonra Cache içeriğinin TextBox'a gelmediğinden emin olunuz.

Böylece geldik bir Hello World uygulamamızın daha sonuna. Bu konuda yeni bilgiler edindikçe sizlere paylaşmaya devam ediyor olacağım. Nitekim mimarisine baktığımızda son derece derin bir konu olduğunu farketmiş olmalısınız. Özellikle şu sıralar bir kaç Cluster Server'a kurulum yapıp Load Balancing ile ilgili testleri nasıl yapabileceğimi düşünmekteyim. Tabi bunun için öncesinden Environment'in tesis edilmesi gerekiyor.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Detaylı bilgi için [tıklayın](http://msdn.microsoft.com/en-us/data/cc655792.aspx).
CTP3 sürümünü [Download](http://www.microsoft.com/downloads/details.aspx?FamilyId=B24C3708-EEFF-4055-A867-19B5851E7CD2&displaylang=en)etmek için tıklayın.

[HelloVelocity.rar (285,93 kb)](/assets/files/2009/HelloVelocity.rar)
