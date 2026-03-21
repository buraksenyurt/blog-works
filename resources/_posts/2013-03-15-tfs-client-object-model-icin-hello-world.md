---
layout: post
title: "TFS–Client Object Model için Hello World"
date: 2013-03-15 02:30:00 +0300
categories:
  - team-foundation-server
tags:
  - team-foundation-server
  - team-explorer-everywhere
  - client-object-model
  - source-control
  - work-item
  - task
  - scrum
  - cmmi
  - msf
  - workitemstore
  - server-object-model
  - build-process-object-model
---
Çok eskidendi diyemeyeceğimiz kadar yakın bir zamanda, bilgisayar programcılarının ilah olduğu devirlerde, evimizin 37 ekran TV’ lerine girmiş Commodore 64K, Amiga oyunlarına sabaha kadar vakit ayırdığımız yıllarda; ne Source Code Control denen bir kavram vardı, ne de 9 kişilik askeri manga misali çalışan Scrum ekipleri. Ancak teknoloji ve yazılım dünyası öylesine hızla ilerledi ki…Koşar adımlarla geldiğimiz günümüzde, özellikle Enterprise çapta yürütülen projelerde, ekip olmadan hareket etmek neredeyse imkansız hale geldi.

[![commodore](/assets/images/2013/commodore_thumb.jpg)](/assets/images/2013/commodore.jpg)


Yazılıma başladığım yıllarda Microsoft Visual SourceSafe kullanan birisi olarak olayın uzun bir süre önce kod kontrolü ve saklanmasının ötesine geçtiğini söylesem sanırım hepimiz bu noktada hem fikir oluruz

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_179.png)

Artık ALM (Application Lifecycle Management) olarak ifade edilen ve ürün geliştirmenin sadece koddan oluşmadığını ifade eden önemli bir kavram var hayatımızda. Hatta bu kavramın içerisine giren MSF (Microsoft Solution Frameworks), SCRUM, CMMI (Capability Maturity Model Integration) gibi pek çok süreçte mevcut. Müşteri ihtiyaçlarının daha hızlı daha çevik bir şekilde karşılanması esasına dayalı bu süreçler çok ama çok popüler.

Tabii işin önemli bir parçasını da ALM için kullanılan etkili araçlar üstlenmekte. Microsoft bu alanda 2000li yılların ortalarından itibaryen Visual SourceSafe’ i terk edip Team Foundation Server’ a geçiş yaptı. Team Foundation Server şu an geldiği 2012 sürümü ile, yazılım alanındaki önemli bir ihtiyacı da karşılamakta: ALM’ in dijital ortamda yönetimi, yürütümü ve kontrolü.

Team Foundation Server’ ın çok geniş bir kavram olduğunu ve bir makaleye sığdırılamayacak kadar çok özelliği bulunduğunu gönül rahatlığı ile ifade edebilirim

![Laughing out loud](/assets/images/2013/wlEmoticon-laughingoutloud_7.png)

Diğer yandan bugünkü yazımıza konu olan onun küçük görünen ama çok önemli işlerin altına imza eden bir parçası…

Client Object Model

Peki, [Microsoft adresinden download edebileceğiniz](http://visualstudiogallery.msdn.microsoft.com/f30e5cc7-036e-449c-a541-d522299445aa) bu nesne modeli bize ne sunuyor? Olaya aşağıdaki grafik ile başlayalım.

[![comhello_1](/assets/images/2013/comhello_1_thumb.png)](/assets/images/2013/comhello_1.png)

Yukarıdaki grafik anlatımında TFS’ in uygulama modeline ait örnek bir dağılıma yer verilmektedir. Hepimiz esas itibariyle Team Foundation Server’ ın, n sayıda makine üzerine fiziki olarak dağıtılabilen bir uygulama sunucusu ve çevre programlar bütünü olduğunu biliyoruz. Bu anlamda TFS uygulama sunucusuna bağlanan pek çok istemci çeşidi de mevcut. Örneğin Continous Integration gibi modelleri destekleyen Build planlarının yönetildiği Build Server veya bir geliştirici makinesi üzerinde koşan Visual Studio gibi.

> TFS’ i bir dünyadan ziyade bir evren olarak nitelendirmek sanıyorum ki yanlış olmaz. Sadece kurulum sonrasında elimizin altında Application Server, Sharepoint ve SSRS (Sql Server Reporting Services), SSAS (Sql Server Analysis Services), Build Server gibi parçalar oluşmakta. Hatta sanallaştırma da işin içerisinde girdiğinde Lab Management için gerekli ek bir çok ortam türemekte. Tüm bunlara bir de Tool setlerini eklediğinizde Güneş Sisteminin üretmiş olabilirsiniz.
> Evren demiştik…Çünkü Güneş Sistemi dışına çıkarak farklı sistemleri de bu çembere dahil edebilirsiniz (LINUX’ dan UNIX’e, MacOS X’ den Eclipse’ e, Oracle’ dan TIBCO’ ya…)

Aslında TFS sistemine dahil olabilecek istemcileri düşündüğümüzde çoğumuzun aklına standart bir geliştirici makinesi ve üzerinde yüklü olan Visual Studio sürümü gelmektedir. Oysaki TFS’ i kullanabilen istemcilerin böyle bir zorunluluğu yoktur. TFS’ in çevre birimler ile olan entegrasyonu adına aşağıdakileri ifade edebiliriz.

- Visual Studio IDE’ sinin bir parçası olan Team Explorer ücretsizdir. Geliştirici olmayan birisi tarafından rahatlıkla kullanılabilir.
- MS Office uygulamaları TFS’ e entegre olabilir, dolayısıyla Work Item’ lar (Seçtiğiniz süreç şablonuna göre değişiklik gösterebilir) örneğin Excel’ e indirebilir, güncellenebilir (Hatta Ms Project ile proje planınızı aktarabilirsiniz vs)
- Takım üyeleri Web arayüzünü kullanıp TFS ortamına bağlanabilir, ALM’ e dahil olabilir, Source Code’ ları görebilir, hatta raporlama hizmetlerine (SSRS-Sql Server Reporting Services) ulaşıp güncel duruma bakabilir (Elbette yetkilendirmelere bağlı olarak neleri görüp neleri göremeyeceklerini nerelere erişip nerelere erişemeyeceklerini belirleyebilirsiniz)
- Sharepoint portalı ile doküman bazlı proje akışları Team Project’ ler ile ilişkilendirebilir.
- [Team Explorer Everywhere](http://www.microsoft.com/en-us/download/details.aspx?id=30661) sayesinde herhangi bir Eclipse IDE ile de Team Explorer’ ın avantajlarından yararlanılabilir. Dolayısıyla örneğin Java ekipleri TFS’ e entegre olabilir.
- Son olarak [MSSCCI Provider](http://visualstudiogallery.msdn.microsoft.com/bce06506-be38-47a1-9f29-d3937d3d88d6) hizmetinden yararlanıp, yabancı araçlarında (SQL Navigator, PowerBuilder, Solaris UNIX, LINUX, TIBCO vb) TFS’ e entegre olması mümkündür.

Son iki madde saha da tecrübe edilmiştir ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_179.png)

Esas itibariyle istemciler, uygulama sunucusu üzerinde yer alan TFS Web servislerini kullanırlar. Bu açıdan bakıldığında söz konusu servisleri tüketen istemci uygulamalar bu işi Client Object Model üzerinden icra etmektedirler. Bu şu anlama da geliyor; TFS’ i kullanacak kendi istemci uygulamalarımızı geliştirebiliriz.

> [Online olarak da Cloud tabanlı hizmet veren TFS](https://tfs.visualstudio.com/)’ in yakında zaman da [OData protokolünü baz alan servisleri](https://tfsodata.visualstudio.com/) yayınlandı. Bu [konuda Brian Keller’ ın şu adresteki yazısını](http://blogs.msdn.com/b/briankel/archive/2013/01/24/bringing-odata-to-team-foundation-service.aspx) incelemenizi şiddetle tavsiye ederim.

TFS açısından olaya bakıldığında 3 farklı nesne modeli olduğunu ifade edebiliriz. Sunucu tarafı için Server Object Model, istemci tarafı için bu yazımızda ele alacağımız Client Object Model ve son olarak da Build Service’ lerin, COM ile olan iletişiminde devreye giren Build Process Object Model. Biz bu yazımızda sadece Client Object Model’ in nasıl kullanılabileceğini basit bir Hello World uygulaması ile irdelemeye çalışıyor olacağız.

Hello Client Object Model

Client Object Model’ i yüklendikten sonra.Net projesine aşağıdaki ekran görüntüsünde yer alan Microsoft.TeamFoundation.Client ve Microsoft.TeamFoundation.Common assembly referanslarının eklenmesi yeterli olacaktır.

[![comhello_2](/assets/images/2013/comhello_2_thumb.png)](/assets/images/2013/comhello_2.png)

> Olur da Reference penceresinde Extensions kısmında çıkmazlar, bu durumda C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\ReferenceAssemblies\v2.0\ adresine bir uğrayın derim
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_179.png)

Şimdi dilerseniz ilk örnek kodlarımızı yazalım.

```csharp
using Microsoft.TeamFoundation.Client; 
using Microsoft.TeamFoundation.Framework.Client; 
using Microsoft.TeamFoundation.Framework.Common; 
using System; 
using System.Configuration; 
using System.Net;

namespace HelloTFSCOM 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Uri tfsAddress = new Uri(ConfigurationManager.AppSettings["TfsAddress"]);

            TfsConfigurationServer tfsServer = 
                TfsConfigurationServerFactory.GetConfigurationServer(tfsAddress); 
            tfsServer.Credentials = new NetworkCredential( 
                ConfigurationManager.AppSettings["Username"], 
                ConfigurationManager.AppSettings["Password"], 
                ConfigurationManager.AppSettings["Domain"] 
                ); 
            tfsServer.Connect(ConnectOptions.IncludeServices); 
            Console.WriteLine("TFS Server : {0}\n" 
                ,tfsServer.Name                
                );

            var teamCollections = tfsServer.CatalogNode.QueryChildren( 
                new[] { CatalogResourceTypes.ProjectCollection }, 
                false, CatalogQueryOptions.None);

            foreach (var teamCollection in teamCollections) 
            { 
                Guid teamCollectionId = new Guid(teamCollection.Resource.Properties["InstanceId"]); 
                TfsTeamProjectCollection teamProjectCollection = tfsServer.GetTeamProjectCollection(teamCollectionId);

                Console.WriteLine("Team Project Collection : {0}\n" 
                    ,teamProjectCollection.Name                    
                    );

                var teamProjects = teamCollection.QueryChildren( 
                    new[] { CatalogResourceTypes.TeamProject }, 
                    false, CatalogQueryOptions.None);

                foreach (CatalogNode teamProject in teamProjects) 
                { 
                    Console.WriteLine("Team Project {0}\n\tDescription {1}" 
                        ,teamProject.Resource.DisplayName 
                        ,teamProject.Resource.Description 
                        ); 
                } 
            } 
        } 
    } 
}
```

İlk olarak TfsConfigurationServer tipine ait bir nesne örneği oluşturuyoruz. Bunun için TfsConfigurationServerFactory fabrika sınıfından ve GetConfigurationServer metodundan yararlanılmaktadır. Operasyona gelen parametre TFS sunucu adresidir. Bu adresi app.config/web.config gibi bir dosyadan alabiliriz. Çok doğal olarak TFS sunucuları genellikle domain kontrolü altında kurulurlar. Bu sebepten domain içerisinde yetkisi olan kullanıcıların sunucuya erişebilmesi mümkündür. Bu sebepten şirket ortamlarında kullanıcı adı ve şifre haricinde bir de domain bilgisine ihtiyaç vardır. Kod parçasında bu 3 bilgi, NetworkCredential sınıfına ait bir nesne örneğinde toplanarak, Credentials özelliğine atanmıştır.

Sonraki adımlarda sırasıyla bir bağlantı işlemi ve sonrasında da TFS sunucusu üzerinde yer alan Team Project Collection içeriklerinin sorgulanması işlemi gerçekleştirilmektedir. Aslında sorgulamaların ortak noktası, dikkat edileceği üzere QueryChildren isimli bir metottan yararlanılması ve neyin sorgulanacağını belirtmek için CatalogResourceTypes enum sabitinden yararlanılmasıdır. Bu sabitin alacağı değerleri gördüğünüzde aslında bu seviyede neleri sorgulayabileceğinizi de anlamış oluyorsunuz

![Smile](/assets/images/2013/wlEmoticon-smile_84.png)

[![comhello_4](/assets/images/2013/comhello_4_thumb.png)](/assets/images/2013/comhello_4.png)

Dolayısıyla TFS üzerinde oldukça fazla nesneyi sorgulayabiliriz. Örneğimizde sadece Team Project Collection ve içerisinde yer alan Team Project örnekleri değerlendirilmiştir. Oysaki test ortamından raporlara, Sharepoint Proje portallerinden SQL veri tabanı örneklerine kadar pek çok içerik sorgulanabilmektedir.

Örnek uygulamayı çalıştırdığımızda ben aşağıdaki ekran görüntüsünde yer alan sonuçları aldım.

[![comhello_7](/assets/images/2013/comhello_7_thumb_1.png)](/assets/images/2013/comhello_7_1.png)

Bu örnek tfs.visualstudio.com üzerinde oluşturduğum bir koleksiyona aittir. seddulbahir.visualstudio.com adresinden ulaşabildiğim koleksiyon içerisinde bir kaç deneme projesi oluşturdum. Tahmin edeceğiniz üzere Windows Live ID ile burada yer almanız ve eğer bir değişiklik olmadıysa 5 kişiye kadar ücretsiz olarak yararlanmanız mümkün. Yani 5 kişilik bir ekibiniz var ise hemen bir TFS hesabı açıp çalışmaya başlayabilirsiniz

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_179.png)

Bu arada Windows Live ID ile bağlandığımız bu sistemde Credential için bir Domain bilgisi vermenize gerek yoktur.

TFS Proje İskeleti

Aslında bu tip bir örneği işletmeden önce TFS’ in genel olarak proje iskelet yapısını bilmekte de yarar vardır. Aşağıdaki şekilde bu durum kısaca özetlenmeye çalışılmaktadır.

[![comhello_3](/assets/images/2013/comhello_3_thumb.png)](/assets/images/2013/comhello_3.png)

Normal şartlarda TFS i kurduğumuzda (ki install işlemi eğer farm üzerine kurulum yapıyorsanız biraz sıkıntılı olabilir ![Confused smile](/assets/images/2013/wlEmoticon-confusedsmile_29.png)) hep Default Collection üzerinden çalışırız. Default Collection aslında SQL tarafında da bir veri tabanına karşılık gelmektedir. Oysaki 100lerce projeye sahip olup, bunların çoğunu Enterprise seviyede inşa eden firmalarda birden fazla Team Project Collection kullanıldığı da görülmektedir. Her Team Project Collection aslında bir Team Project ailesini işaret etmektedir. Bir başka deyişle bir Team Project Collection içerisinde n sayıda Team Project barındırabilirsiniz. Çok doğal olarak her bir Team Project de kendi içerisinde birden fazla proje barındırabilir.

Ne yazık ki Team Project ile Project kavramları zaman zaman birbirlerine karışabilmektedir. Aslında bu ayırım uygulanmak istenen süreç noktasında kendisini daha belirgin gösterir. Nitekim bir Team Project oluştururken Scrum, MSF, CMMI veya özelleştirilmiş bir Process Template seçilmelidir.

> Var olan bir Process Template’ i indirip, XML içerikleri ile oynayabilir ve şirket kültürünüze uygun farklı bir süreç şablonu oluşturabilirsiniz. Bu anlamda [Microsoft’ un Power Tools](http://visualstudiogallery.msdn.microsoft.com/b1ef7eb2-e084-4cb8-9bc7-06c3bad9148f) ürününü kullanmanızı öneririm. Visual Studio 2012’ ye bir eklenti şeklinde gelip şablonları görsel olarak yönetebilmenize olanak tanımaktadır.[![comhello_6](/assets/images/2013/comhello_6_thumb.png)](/assets/images/2013/comhello_6.png)

Yani ALM yoğurt yiyiş şekli belirlenmelidir. Sonrasında ise bu Team Project içerisine dahil olan ve aynı şekilde yoğurt yiyecek olan ekip elemanları, n sayıda ve n çeşitte proje üzerinde çalışabilir. Bu projeler.Net uygulamaları olabileceği gibi.Net dışı ortamlar da olabilir. Önemli olan tüm bu projelerin aynı Team Project içerisinde dahil olmalarıdır.

WorkItemStore ile Work Item Öğelerini Sorgulamak

Şimdi örneği biraz daha ilerletelim ve diğer TFS Web Service’ lerinden nasıl yararlanabileceğimize bakalım. Söz gelimi bir Team Project için söz konusu Work Item’ ları çekmeye çalışalım. Bu amaçla örnek kodlarımızı aşağıdaki gibi geliştirebiliriz.

> WorkItemStore tipi Microsoft.TeamFoundation.WorkItemTracking.Client.dll assembly’ ı içerisinde yer aldığından söz konusu kütüphanenin projeye referans edilmesi gerekmektedir.

Work Item’ ların sorgulanabilmesi için WorkItemStore servisinden yararlanılabilir. Bu servise ait bir nesne örneğini elde etmek içinse var olan bir TFS Team Project Collection’ dan faydalanılabilir. Bu sebepten örnek kod parçasında TfsTeamProjectCollection tipinden bir örnek üretilmiştir. Adres kısmındaki /DefaultCollection ilavesine lütfen dikkat edelim. Bu şekilde DefaultCollection’ ı işaret eden bir örnek oluşturmuş bulunuyoruz. Sonrasında tfsCollection değişkeni üzerinden GetService fonksiyonunu çağırmaktayız. Dikkat edileceği üzere fonksiyon parametre olarak WorkItemStore tipini almaktadır. Bu durumda TFS Sunucusuna şunu söylemiş oluyoruz.

Ey o adresteki DefaultCollection. Bana, tuttuğun Work Item örneklerini sorgulayabilmem için bir servis referansı verrrr!!!

Bu işlemin ardından artık Work Item’ ların sorgulanmasına başlanıyor. Aslında tipik bir SQL sorgusu diyebileceğimiz ama literatürde WIQL (WorkItem Query Language) olarak geçen ifademizi Query metoduna parametre olarak veriyoruz. Sorgumuz son derece basit. ARGE isimli Team Project içerisinde yer alan WorkItem’ lardan Task tipinden olanları, önce State’ e göre A…Z sırasında, sonrasında da son değişiklik zamanına (Changed Date) göre Z…A sırasında talep ediyoruz. Sonuç olarak benim deneme amaçlı olarak kullandığım ARGE isimli Team Project için aşağıdaki ekran görüntüsünde yer alan sonuçları elde ettiğimi ifade edebilirim.

[![comhello_5](/assets/images/2013/comhello_5_thumb.png)](/assets/images/2013/comhello_5.png)

Görüldüğü üzere ARGE projesine ait Task tipinden Work Item öğelerinin başlıkları (Title), güncel durumları (State) ve son değişiklik zamanları (Changed Date) elde edilebilmiştir. Bu noktada neler yapabileceğinizi ifade etmek istediğimizde sadece Team Explorer ile veya TFS’ in Web arayüzü ile yapabildiklerinizi düşünmeniz kafi olacaktır.(WIQL’ in örnek kullanımları ve 5 parçadan oluşan iskelet yapısının teknik detayı için [MSDN adresini ziyaret](http://msdn.microsoft.com/en-us/library/vstudio/bb130306.aspx) edebilirsiniz)

```csharp
using Microsoft.TeamFoundation.Client; 
using Microsoft.TeamFoundation.Framework.Client; 
using Microsoft.TeamFoundation.Framework.Common; 
using Microsoft.TeamFoundation.WorkItemTracking.Client; 
using System; 
using System.Configuration; 
using System.Net;

namespace HelloTFSCOM 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Bir Team Project Collection' daki Task' ların Çekilmesi

            TfsTeamProjectCollection tfsCollection = new TfsTeamProjectCollection( 
                new Uri(ConfigurationManager.AppSettings["TfsAddress"] + "/DefaultCollection")); 
            tfsCollection.Credentials = new NetworkCredential( 
                ConfigurationManager.AppSettings["Username"], 
                ConfigurationManager.AppSettings["Password"] 
                , ConfigurationManager.AppSettings["Domain"] 
                );

            WorkItemStore store = (WorkItemStore)tfsCollection.GetService(typeof(WorkItemStore));

           WorkItemCollection queryResults = store.Query(@" 
                           Select [State], [Title] 
                           From WorkItems 
                           Where [Work Item Type] = 'Task' and [Team Project]='ARGE' 
                           Order By [State] Asc, [Changed Date] Desc");

            for (int i = 0; i < queryResults.Count; i++) 
            { 
                Console.WriteLine("{0}\n{1}\n{2}\n",queryResults[i].Title, queryResults[i].State, queryResults[i].ChangedDate.ToString());            
            }

            #endregion Bir Team Project Collection' daki Task' ların Çekilmesi 
        } 
    } 
}
```

> WorkItemStore dışında kullanılabilen pek çok servis vardır. Örneğin ISecurityService, ILocationService, IBuildServer, IProcessTemplates, VersionControlServer vb…Söz konusu servislerden bazıları Team Project Collection, bazıları da TFS Server seyiyesinde kullanılabilmektedir. [Detaylı bilgiye bu adresten ulaşabilirsiniz](http://msdn.microsoft.com/en-us/library/vstudio/bb286958.aspx#services).

Bu yazımızda TFS Client Object Model ile basit de olsa el sıkışmaya çalıştık. Elbetteki örneği devam ettirip ilerletmek sizin elinizde. Söz gelimi iyi bir antrenman olarak Visual Studio Team Explorer penceresinin bir benzerini ve hatta Windows Phone 8 veya Windows 8 üzerinde çalışacak şekilde zengin kullanıcı deneyimi sunacak olan bir türevini geliştirebilirsiniz. Dokunmatikliği işin içerisine katın

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_179.png)

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloTFSCOM.zip (181,68 kb)](/assets/files/2013/HelloTFSCOM.zip)