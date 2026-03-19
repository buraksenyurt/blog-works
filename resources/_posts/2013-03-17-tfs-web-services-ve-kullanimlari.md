---
layout: post
title: "TFS Web Services ve Kullanımları"
date: 2013-03-17 12:30:00 +0300
description: "TFS mimari alt yapısı ve çevre etkileşimini incelediğimiz şu yazımızda Client Object Model’ i kısaca anlamaya çalışmıştık. O makalede yer alan mimari çizime dikkatlice bakarsak eğer, Client Object Model’ in aslında TFS Web Service’ ler ile haberleştiğini görebiliriz. Aslına bakarsanız Team Foundation Server tarafında epeyce fazla sayıda XML Web Service yer almaktadır. Bu servisleri ana hatları ile değerlendirdiğimizde ise sunucu ve koleksiyon seviyesinde olmak üzere iki ana dala bölündüklerini görürüz."
categories:
  - team-foundation-server
tags:
  - team-foundation-server
  - csharp
  - dotnet
  - oracle
  - wcf
  - xml
  - web-service
  - http
  - iis
  - generics
  - asmx
---
# TFS Web Services ve Kullanımları
[![cartoon-soldier-010](/assets/images/2013/cartoon-soldier-010_thumb.jpg)](/assets/images/2013/cartoon-soldier-010.jpg) Merhaba Arkadaşlar,

Yedek Subay olarak askerlik hizmetimi yerine getirdiğim yıllarda (O zamanlar 16 ay idi) Jandarma Genel Komutanlığı Personel Şube’ de görev almıştım. Aslında temel işim Powerpoint ile sunum hazırlamaktı ama verilen emir her ne ise onu da yerine getirmek mesuliyetini taşımaktaydım.

Bir gün komutanım ile birlikte yine sivil hayat için anlamsız olan ama Askeri disiplin kuralları çerçevesinde gayet de makul görünen bir işe adanmıştık. Neredeyse tüm komutanlık personelinin iğneli Printer’ dan çıkartılmış karınca yazısı ebatlarındaki bilgilerini, bir diğer koca liste ile karşılaştıracak ve bir filtreleme işlemi gerçekleştirecektik. (İşin yaklaşık olarak kesintisiz çalışma ile 48 saate varabileceğini biliyorduk)

Takdir edersiniz ki o yıllarda bilgisayar kullanılıyor olmasına rağmen, komutanlığın ihtiyaç duyduğu ve hayatı kolaylaştıracak işlevsellikler için genellikle Microsoft Office ürünleri ele alınmaktaydı ama elimizde şöyle zırt diye filtreleme yapabileceğimiz bir SQL/Oracle ortamı da yoktu. (Yıl 2001-2003 arası diyeyim)

Hal böyle olunca elimize aldık çıktıları başladık tek tek karşılaştırmaya. Bir ara ben durup bunu daha kolay nasıl yapabiliriz diye düşünmeye başladığımı hatırlıyorum. Hatta o anlarda arkamda beliren Yarbay’ ımın da sert bir ses tonu ile beni rüyamdan uyandırdığını…”Asteğmenimmmm!!!…”

Doğruyu söylemek gerekirse ne ben ne de komutanım tüm listeyi dolaşmak istemiyorduk. Kafa kafaya vererek güzel bir yol bulduk elbette

![Smile](/assets/images/2013/wlEmoticon-smile_88.png)

Yol derken elimizde cetvel, kalem vesaire vardı. Ne bilgisayar ne de başka bir akıllı cihaz. O zaman anladım ki bazı işlerde kişiye büyük sabır gerekebiliyor. Örneğin TFS (Team Foundation Server) üzerinde kullanılan XML Web Service örneklerinin teker teker bulunup çıkartılması gibi

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_185.png)

TFS mimari alt yapısı ve çevre etkileşimini incelediğimiz [şu yazımızda](https://www.buraksenyurt.com/post/TFS-2012-Client-Object-Model-icin-Hello-World) Client Object Model’ i kısaca anlamaya çalışmıştık. O makalede yer alan mimari çizime dikkatlice bakarsak eğer, Client Object Model’ in aslında TFS Web Service’ ler ile haberleştiğini görebiliriz. Aslına bakarsanız Team Foundation Server tarafında epeyce fazla sayıda XML Web Service yer almaktadır. Bu servisleri ana hatları ile değerlendirdiğimizde ise sunucu ve koleksiyon seviyesinde olmak üzere iki ana dala bölündüklerini görürüz.

Aşağıdaki şekilde bu servisler genel isimlendirmeler halinde ifade edilmektedir.

[![tfsservices_1](/assets/images/2013/tfsservices_1_thumb.png)](/assets/images/2013/tfsservices_1.png)

Dikkat edilmesi gereken notkalardan birisi de bazı servislerin her iki seviyede de yer alıyor olmasıdır. Ne varki bu servislerin alanları farklıdır. Sadece bir koleksiyon ve içeriği için kullanılabilecek olan hizmetler Collection Level grubunda yer almaktadır. Diğer taraftan tüm TFS sunucusunu ilgilendiren servisler de Server Level grubuna dahildir.

Yine dikkat edileceği üzere koleksiyon seviyesinde farklılaşan (Team Project Collection örneklerine özel olan) hizmetler bulunmaktadır. Örneğin Version Control veya Lab Management gibi. Normal şartlarda bu servislere, kurulu olan TFS ortamına erişim yetkisi olan istemcilerden ulaşılabilinmektedir. Örneğin koleksiyon seviyesinde kullanılabilen ve proje listesinin çekilmesi, proje oluşturması, silinmesi, branch silinmesi vb operasyonları ele alabildiğimiz Common Structure Service hizmetinin 4ncü versiyonuna aşağıdaki şekilde görüldüğü gibi ulaşabiliriz.

[![tfsservices_3](/assets/images/2013/tfsservices_3_thumb.png)](/assets/images/2013/tfsservices_3.png)

Çok doğal olarak diğer servislere de ulaşmamız, hatta WSDL (Web Service Description Language) çıktılarına bakmamız mümkündür. Ben çalışmakta olduğum Team Foundation Server 2012 için, uygulama sunucusunun yüklü olduğu IIS (Internet Information Services) altında yaptığım araştırmalarda, aşağıdaki uzun listeye ulaştığımı rahatlıkla ifade edebilirim.

Administration

http://tfsserver:8080/tfs/TeamFoundation/Administration/v4.0/AccessControlService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v4.0/FileHandlerService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v4.0/IdentityManagementService2.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/AdministrationService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/CatalogService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/EventService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/IdentityManagementService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/JobService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/LocationService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/PropertyService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/RegistryService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/SecurityServices.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/TeamProjectCollectionService.asmx
http://tfsserver:8080/tfs/TeamFoundation/Administration/v3.0/WarehouseControlService.asmx

Lab

http://tfsserver:8080/tfs/TeamFoundation/Lab/v3.0/LabFrameworkService.asmx

TFS Resources – Services

http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/AuthorizationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/CommonStructureService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/ConnectedServicesService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/EventService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/GroupSecurityService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/ProcessConfigurationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/ProcessTemplate.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/ProjectMaintenance.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/registration.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/ServerStatus.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/StrongBoxService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v1.0/TeamConfigurationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v2.0/GroupSecurityService2.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v2.0/ProcessConfigurationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v2.0/ProcessConfigurationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/AuthorizationService3.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/CommonStructureService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/IdentityManagementService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/JobService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/LocationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/PropertyService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/RegistryService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/SecurityService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v3.0/SyncService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v4.0/AccessControlService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v4.0/AuthorizationService4.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v4.0/CommonStructureService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v4.0/FileHandlerService.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v4.0/IdentityManagementService2.asmx
http://tfsserver:8080/tfs/_tfs_resources/services/v4.0/SyncService4.asmx

TFS Resources - Work Item Tracking

http://tfsserver:8080/tfs/_tfs_resources/workitemtracking/v5.0/clientservice.asmx
http://tfsserver:8080/tfs/_tfs_resources/workitemtracking/v4.0/ClientService.asmx
http://tfsserver:8080/tfs/_tfs_resources/workitemtracking/v3.0/ClientService.asmx
http://tfsserver:8080/tfs/_tfs_resources/workitemtracking/v1.0/ClientService.asmx
http://tfsserver:8080/tfs/_tfs_resources/workitemtracking/v1.0/ConfigurationSettingsService.asmx
http://tfsserver:8080/tfs/_tfs_resources/workitemtracking/v1.0/ExternalServices.asmx
http://tfsserver:8080/tfs/_tfs_resources/workitemtracking/v1.0/Integration.asmx

TFS Resources - Verison Control

http://tfsserver:8080/tfs/_tfs_resources/VersionControl/v4.0/Repository.asmx
http://tfsserver:8080/tfs/_tfs_resources/VersionControl/v3.0/Repository.asmx
http://tfsserver:8080/tfs/_tfs_resources/VersionControl/v1.0/Repository.asmx
http://tfsserver:8080/tfs/_tfs_resources/VersionControl/v1.0/Administration.asmx
http://tfsserver:8080/tfs/_tfs_resources/VersionControl/v1.0/Integration.asmx
http://tfsserver:8080/tfs/_tfs_resources/VersionControl/v1.0/ProxyStatistics.asmx

TFS Resources - Test Management

http://tfsserver:8080/tfs/_tfs_resources/TestManagement/v2.0/TestManagementWebService.asmx
http://tfsserver:8080/tfs/_tfs_resources/TestManagement/v1.0/TestImpactService.asmx
http://tfsserver:8080/tfs/_tfs_resources/TestManagement/v1.0/TestResults.asmx
http://tfsserver:8080/tfs/_tfs_resources/TestManagement/v1.0/TestResultsEx.asmx

TFS Resources - Sync

http://tfsserver:8080/tfs/_tfs_resources/sync/v4.0/AdministrationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/sync/v3.0/AdministrationService.asmx

TFS Resources - Lab

http://tfsserver:8080/tfs/_tfs_resources/lab/v4.0/LabService.asmx
http://tfsserver:8080/tfs/_tfs_resources/lab/v3.0/Integration.asmx
http://tfsserver:8080/tfs/_tfs_resources/lab/v3.0/LabAdminService.asmx
http://tfsserver:8080/tfs/_tfs_resources/lab/v3.0/LabService.asmx
http://tfsserver:8080/tfs/_tfs_resources/lab/v3.0/TestIntegrationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/lab/v3.0/WorkflowIntegrationService.asmx

TFS Resources - Discussion

http://tfsserver:8080/tfs/_tfs_resources/discussion/v1.0/discussionwebservice.asmx

TFS Resources - Build

http://tfsserver:8080/tfs/_tfs_resources/build/v4.0/AdministrationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v4.0/AgentReservationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v4.0/BuildDeploymentService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v4.0/BuildQueueService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v4.0/BuildService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v4.0/SharedResourceService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v3.0/AdministrationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v3.0/AgentReservationService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v3.0/BuildQueueService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v3.0/BuildService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v3.0/Integration.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v3.0/SharedResourceService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v2.0/BuildService.asmx
http://tfsserver:8080/tfs/_tfs_resources/build/v2.0/Integration.asmx

Metadata Publishing'i kapalı olan WCF servisleri

http://tfsserver:8080/tfs/queue/_tfs_resources/services/v4.0/MessageQueueService.svc
http://tfsserver:8080/tfs/queue/_tfs_resources/services/v4.0/MessageQueueService2.svc

Görüldüğü üzere oldukça uzun bir servis listesi söz konusu.

> Aslına bakarsanız bu servislerin detaylı olarak ne iş yaptıklarına dair MSDN üzerinde çok fazla bilgi bulunmamaktadır. Standart bir teknik Help dokümanından ötesi değildir. Bu nedenle TFS’ in çalışma yapısını bilip, biraz tahminler yürüterek ilerlemeye çalışmak, işinizi epeyce kolaylaştıracaktır.

#Region Off Topic

Bu arada dilerseniz bu servislerin tamamı izlemek için basit bir Web uygulaması geliştirebilir ve örneğin listedeki adresleri bir Web User Control içerisine gömerek, tıklama usulüyle ilgili XML Web Service adreslerine gidebilirsiniz. Örneğin aşağıdaki gibi bir Web User Control ve yardımcı sınıf işinizi görecektir.

Text dosyasında durmakta olan servis adreslerini okuyan yardımcı tip ve metod;

```csharp
using System.Collections.Generic; 
using System.IO; 
using System.Web.UI.WebControls;

namespace AllServices 
{ 
    public static class Utility 
    { 
        public static List<HyperLink> GetServiceLinks(string textFilePath) 
        { 
            List<HyperLink> links = new List<HyperLink>();

            string[] lines = File.ReadAllLines(textFilePath); 
            foreach (string line in lines) 
            { 
                if (line.StartsWith("http://")) 
                { 
                    HyperLink link = new HyperLink 
                    { 
                        Text = line.Substring(line.LastIndexOf("tfs")), 
                        NavigateUrl = line 
                    }; 
                    links.Add(link); 
                }                
            }

            return links; 
        } 
    } 
}
```

Web User Control kodu;

```csharp
using System; 
using System.Web.UI.WebControls;

namespace AllServices 
{ 
    public partial class TfsServicesControl : System.Web.UI.UserControl 
    { 
        protected void Page_Load(object sender, EventArgs e) 
        { 
            var serviceLinks=Utility.GetServiceLinks(Server.MapPath("~/Services.txt")); 
            foreach (var serviceLink in serviceLinks) 
            { 
                divLinks.Controls.Add(serviceLink); 
                divLinks.Controls.Add(new Literal { Text = "<br/><br/>" }); 
            } 
        } 
    } 
}
```

ve işte çalışma zamanına ait örnek ekran çıktısı

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_185.png)

[![tfsservices_4](/assets/images/2013/tfsservices_4_thumb.png)](/assets/images/2013/tfsservices_4.png)

Bir servis adına tıklayın ve içeriğine ulaşın

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_185.png)

#endregion Off Topic

Referans Etmek

Team Foundation Server üzerinden sunulan XML Web Service örneklerini herhangibir.Net istemcisi tarafından tüketmek istediğimizde, Client Object Model üzerinden erişimde bulunmamız gerekmektedir. Normal şartlarda ilgili Web Service’ leri, Add Service Reference sekmesinden hareket edilerek projeye ilave edilebilir ve üretilen Proxy tipinin metodlarına ulaşılabilir.

[![tfsservices_6](/assets/images/2013/tfsservices_6_thumb.png)](/assets/images/2013/tfsservices_6.png)

Ancak çalışma zamanında servis tarafında kuvvetle muhtemel aşağıdaki şekilde görülen 504 Unknown Host hatası alınacaktır.

[![TfsError2](/assets/images/2013/TfsError2_thumb.png)](/assets/images/2013/TfsError2.png)

Bunun nedeni aslında ilgili servislerin, Client Object Model (veya Server Object Model) tafaından ele alınış ve üretiliş şekilleridir. Ancak unutulmaması gereken bir nokta da şudur. Bu servislerin çoğu, TFS'in kurulu makine üzerinden erişilmeye çalışıldığında (en azından bir tarayıcı ile) bildiğimiz XML Web Service'leri gibi tüketilebilirler. Örneğin TFS'in kurulu olduğu makinede http://tfsserver:8080/tfs/TeamFoundation/Administration/ v3.0/WarehouseControlService.asmx'e ulaşmayı deneyin. (Yani ilgili servislere kurulu olduğu makineden yerel olarak ulaşmayı) Bu durumda aşağıdaki gibi bir operasyonu deneyebileceğinizi görebilirsiniz.

![gpstatus.png](/assets/images/2013/gpstatus.png)

Dilerseniz TFS'e uzaktan bağlanacak bir istemci açısından olaya bakmaya devam edelim ve basit bir kullanım şeklini değerlendirerek ilerlemeye çalışalım.

Hello World

Aşağıdaki kod parçasında Collection Level grubundan iki örnek servisin kullanımına yer verilmiştir. Bu servislerden birisi ICommonStructureSerice4, diğeri ise IProcessTemplates arayüzleri (Interface) tarafından taşınmaktadır. Dikkat edileceği üzere anahtar nokta TfsTeamProjectCollection tipinin örnekleniş şeklidir. Burada TfsTeamProjectCollectionFactory sınıfının static GetTeamProjectCollection metodundan yararlanılmakta olup, fonksiyona parametre olarak TFS sunucusundaki Team Project Collection’ ın HTTP tabanlı adresi geçilmektedir.

> Bildiğiniz üzere TFS kurulumunda aksi belirtilmedikçe mutlaka Default Collection isimli bir Team Project Collection oluşturulmaktadır.

Tabi ki Server Level servis gruplarını kullanmak da isteyebiliriz. Bu durumda TfsConfigurationServer ve TfsConfigurationServerFactory tiplerinden yararlanmamız gerekmetedir.

Gelelim örnek uygulama kodlarımıza. Bunun için herzaman olduğu gibi basit bir Console uygulamasından yararlanıyor olacağız. Eğer makinemizde TFS Client Object Model veya ilgili SDK yüklüyse en azından Microsoft.TeamFoundation.Client ve Microsoft.TeamFoundation.Common assembly’ larının referans edilmesi gerekmektedir. Ancak bazı servisler farklı Assembly’ ların referans edilmesini gerektirebilir. Söz gelimi WorkItemStore için, Microsoft.TeamFoundation.WorkItemTracking.Client.dll isimli assembly referans edilir.

```csharp
using Microsoft.TeamFoundation.Client; 
using Microsoft.TeamFoundation.Server; 
using System;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
           TfsTeamProjectCollection collection = TfsTeamProjectCollectionFactory 
               .GetTeamProjectCollection(new Uri("http://tfsserver:8080/tfs/defaultcollection"));

            #region Bazı servislerin çekilmesi

           ICommonStructureService4 commonStructureService = (ICommonStructureService4)collection 
                .GetService(typeof(ICommonStructureService4));

            IProcessTemplates processTemplateService = (IProcessTemplates)collection.GetService<IProcessTemplates>();

            #endregion Bazı servislerin çekilmesi

            #region Bir Team Project adı üzerinden temel proje bilgisinin çekilmesi

            ProjectInfo argeInfo = commonStructureService.GetProjectFromName("ARGE"); 
            Console.WriteLine("Proje\t\t\t{0}\nDurumu\t\t\t{1}\nTemplate Id\t\t{2}" 
                , argeInfo.Name 
                , argeInfo.Status 
                , argeInfo.Uri.ToString() 
                );

            string name; 
            string state; 
            int templateId; 
            ProjectProperty[] projectProperties;

            // Tüm Proje özelliklerinin çekilmesi 
           commonStructureService.GetProjectProperties( 
                argeInfo.Uri.ToString() 
                , out name 
                , out state 
                , out templateId 
                , out projectProperties); 
            Console.WriteLine("\nProje Özellikleri\n"); 
            foreach (var projectProperty in projectProperties) 
                Console.WriteLine("Proje Özelliği\t\t{0}\nDeğeri\t\t\t{1}", projectProperty.Name, projectProperty.Value);

            #endregion Bir Team Project adı üzerinden temel proje bilgisinin çekilmesi

            #region Sunucuda yüklü Process Template' lerin bilgilerinin elde edilmesi

            Console.WriteLine("\nProcess Templates\n"); 
            TemplateHeader[] templateHeaders = processTemplateService.TemplateHeaders(); 
            foreach (TemplateHeader templateHeader in templateHeaders) 
            { 
                Console.WriteLine("Template Id\t{0}\nAdı\t\t{1}\nRank\t\t{2}\nDurumu\t\t{3}\nMetadata\t{4}\nAçıklama\t{5}\n", 
                    templateHeader.TemplateId, 
                    templateHeader.Name, 
                    templateHeader.Rank, 
                    templateHeader.State, 
                    templateHeader.Metadata, 
                    templateHeader.Description 
                    ); 
            }

            #endregion Sunucuda yüklü Process Template' lerin bilgilerinin elde edilmesi           
        } 
    } 
}
```

TfsTeamProjectCollection referansı elde edildikten sonra, alınmak istenen hizmete ait nesne örneğinin üretilmesi için GetService metodundan yararlanılmaktadır. Bu metodun Type parametresi ile çalışan versiyonu dışında generic olan bir versiyonu daha bulunmaktadır. Örneğin,

```csharp
ICommonStructureService4 commonStructureService = (ICommonStructureService4)collection 
                .GetService(typeof(ICommonStructureService4));
```

kod satırı ile Common Structure Service örneği üretilmektedir. Bu adımdan sonra söz konusu servise ait referansın fonksiyonları kullanılabilir. Söz gelimi bir proje adı verilerek özelliklerinin elde edilmesi sağlanabilir. Bu özellikler arasında bir Team Project’ in uyguladığı Process Template bilgisi ve hatta XML tabanlı şablon içeriği de yer almaktadır. Aşağıdaki örnek ekran çıktısında ARGE isimli projenin ulaşılan bilgileri gösterilmektedir.

[![tfsservices_7](/assets/images/2013/tfsservices_7_thumb.png)](/assets/images/2013/tfsservices_7.png)

Diğer yandan IProcessTemplate arayüzüne atanan servis referansının elde edilmesi için, GetService metodunun generic sürümünden yararlanılmıştır. Sonrasında ise TFS sunucusunda yüklü olan Process Template listesine gidilerek Name, State, Metadata, Id, Description gibi bilgileri elde edilmiştir. Aşağıdaki ekran çıktısına bakıldığında Scrum 2.0, CMMI ve MSF şablonlarının yüklenmiş olduğu bilgisine ulaşılabilinir.

[![tfsservices_8](/assets/images/2013/tfsservices_8_thumb.png)](/assets/images/2013/tfsservices_8.png)

Tabi burada akla takılan en önemli sorunlardan birisi kullanabileceğimiz TFS Web Service’ lerinin kod tarafındaki GetService metodu tarafından kullanılabilecek karşılıklarının neler olduğudur?

![Who me?](/assets/images/2013/wlEmoticon-whome_9.png)

Bu konuda aşağıdaki listenin yardımcı olabileceğini düşünüyorum.

Servisin Adı
Collection Level
Server Level
Assembly
Namespace

ITeamFoundationRegistry
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

IIdentityManagementService
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

ITeamFoundationJobService
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

IPropertyService
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

IEventService
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

ISecurityService
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

ILocationService
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

TswaClientHyperlinkService
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

ITeamProjectCollectionService
Yok
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

IAdministrationService
Var
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

ICatalogService
Yok
Var
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Framework.Client

VersionControlServer
Var
Yok
Microsoft.TeamFoundation.VersionControl.Client
Microsoft.TeamFoundation.VersionControl.Client

WorkItemStore
Var
Yok
Microsoft.TeamFoundation.WorkItemTracking.Client
Microsoft.TeamFoundation.WorkItemTracking.Client

IBuildServer
Var
Yok
Microsoft.TeamFoundation.Build.Client
Microsoft.TeamFoundation.Build.Client

ITestManagementService
Var
Yok
Microsoft.TeamFoundation.TestManagement.Client
Microsoft.TeamFoundation.TestManagement.Client

ILinking
Var
Yok
Microsoft.TeamFoundation.Common
Microsoft.TeamFoundation

ICommonStructureService3
Var
Yok
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Server

IServerStatusService
Var
Yok
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Server

IProcessTemplates
Var
Yok
Microsoft.TeamFoundation.Client
Microsoft.TeamFoundation.Server

Yukarıdaki listede eksikliker olabilir. Gelen Update’ ler, Service Pack’ ler ve yeni sürümler sonrasın güncellenebilir. Lütfen MSDN üzerinden kontrol ediniz.

Demek ki, istemci tarafında çalışarak hayatımızı kolaylaştırmakta olan pek çok.Net tabanlı uygulama (Örneğin Power Tools veya Team Explorer gibi) söz konusu nesne modellerini ve ilgili XML Web Service’ lerini kullanmaktadır. Elbette Team Foundation Server tarafında çok fazla sayıda XML Web Service metodu ve hatta versiyonu bulunmaktadır. TFS üzerindeki tecrübelerinizden yararlanarak bu servislerin efektif olarak kullanımlarını irdeleyebilirsiniz. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_TFSServices.zip (213,36 kb)](/assets/files/2013/HowTo_TFSServices.zip)