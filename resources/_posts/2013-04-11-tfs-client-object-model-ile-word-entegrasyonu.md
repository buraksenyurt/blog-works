---
layout: post
title: "TFS Client Object Model ile Word Entegrasyonu"
date: 2013-04-11 13:15:00 +0300
categories:
  - team-foundation-server
tags:
  - team-foundation-server
  - client-object-model
  - work-item
  - task
  - product-backlog-item
  - scrum
  - cmmi
  - msf
---
Geçtiğimiz gün National Geographic kanalında Mega Fabrikalar’ ı seyretme fırsatı buldum. Amerikalı Dodge firması efsane Challanger‘ ı yeniden üretmekteydi. Konu bu üretimin gerçekleştirildiği mega fabrikaydı.

[![Dodge-Challenger-production-1024x566](/assets/images/2013/Dodge-Challenger-production-1024x566_thumb.jpg)](/assets/images/2013/Dodge-Challenger-production-1024x566.jpg)


Robotların, gelişmiş endüstürinin ve insan gücünün bir araya geldiği fabrika, sadece 24 saat içerisinde üretim hattından mükemmel spor arabalar çıkmaktaydı. Üstelik motor bloğu da kıtanın bir diğer ucundan geliyordu.

Her ne kadar mükemmele yakın bir üretim bandı da olsa, akıllı bilgisayarlar üretim sürecindeki her adımı gözlemliyor ve bir istisna olması halinde bandı durduruyordu. O da yetmiyor pek çok noktada usta insanlar devreye giriyor ve gerekirse üretim bandını kendi insiyatifleri ile durduruyorlardı. Tabi burada yazarak anlatmak çok zor o yüzden mutlaka seyredin derim. Aslında belgeseli izlerken en çok da şunu düşündüm “Böyle muazzam yapılar nasıl oluyor da inşa ediliyor? İnsan aklı ne kadar muazzam ki her ayrıntıyı düşünüyor, düşünmeye çalışıyor”

Derken bilgisayarımın başına döndüm ve kendi kendime şöyle dedim “E illa ki bir Hello World uygulamaları vardır yahu”

![Nerd smile](/assets/images/2013/wlEmoticon-nerdsmile.png)

Bu yazımız ile birlikte Team Foundation Server maceralarımıza devam etmeye çalışıyor olacağız. Yeni bölümüzde Client Object Model’ i bir Word uygulaması içerisinde kullanmaya çalışacağız ve çok basit olarak Work Item öğelerini nasıl kayıt edebileceğimizi göreceğiz.

> Team Foundation Server dünyası ile ilişkili önceki yazılarıma aşağıdaki adreslerden ulaşabilirsiniz.
> - [TFS Client Object Model için Hello World](https://www.buraksenyurt.com/post/TFS-2012-Client-Object-Model-icin-Hello-World)
> - [TFS Web Services ve Kullanımları](https://www.buraksenyurt.com/post/TFS-Web-Services-ve-Kullanimi)
> - [Heryerden TFS Kullanabilmek](/2013/03/02/heryerden-tfs-kullanabilmek/)

Çalışmakta olduğumuz Team Project’ in süreç şablonu (Process Template) ne olursa olsun (Scrum, MSF, CMMI) giriş yapılan öğeler Work Item olarak düşünülmektedir. Örneğin Scrum felsefesi göz önüne alındığında Product Backlog Item, Task, Bug, Test Case ve Impediment birer Work Item’ dır. CMMI şablonuna bakıldığında ise Requirement, Task, Bug, Change Request, Issue, Review, Risk ve Test Case birer Work Item olarak düşünülmektedir.

Pek tabi bu Work Item öğeleri Client Object Model tarafında da birer tip olarak ele alınabilirler. Client Object Model bilindiği üzere, Team Foundation Server ın dış dünya ile olan iletişiminde ve özellikle çevre araçlar ile olan entegrasyonunda önemli bir yere sahiptir. Dilerseniz örneğimize geçelim ve adım adım ilerleyerek konuyu anlamaya çalışalım.

Senaryo

Elimizde Scrum 2.0 formatında oluşturulmuş bir Team Project var. Amacımız Word belgesi içerisinden, bir Product Backlog Item ve buna bağlı iki Task öğesinin kayıt edilmesini sağlamak. Olayı son derece basit bir biçimde ele alacağımızdan Product Backlog Item ve Task öğeleri için sadece Title ve Description içeriklerin yer veriyor olacağız. Kritik noktalardan birisi de, Task öğeleri ile Product Backlog Item arasında Parent-Child ilişkinin kurulmasıdır. Yani Task öğeleri ilgili Product Backlog’ a bağlı olacaklardır.

Ön Hazırlıklar ve Doküman Tasarımı

İşe ilk olarak Visual Studio 2012 ortamında bir Word 2010 Document projesi oluşturarak başlamalıyız.

[![tfsword_1](/assets/images/2013/tfsword_1_thumb.png)](/assets/images/2013/tfsword_1.png)

Bunun için yukarıdaki ekran görüntüsünde olduğu gibi Office/Sharepoint sekmesinde yer alan Word 2010 Document şablonunun seçilmesi yeterlidir. Word uygulaması içerisinde Team Foundation Server Client Object Model kullanılacağından ilgili Assembly referanslarının projeye eklenmesi de gerekmektedir. (Bu örnek için Microsoft.TeamFoundation.Client ve Microsoft.TeamFoundation.WorkItemTracking.Client dll’ lerini eklemeliyiz)

[![tfsword_3](/assets/images/2013/tfsword_3_thumb.png)](/assets/images/2013/tfsword_3.png)

Word dokümanının tasarımını ise aşağıdaki ekran görüntüsünde yer aldığı gibi yapabiliriz.

[![tfsword_4](/assets/images/2013/tfsword_4_thumb.png)](/assets/images/2013/tfsword_4.png)

Oldukça sade bir tasarımımız var. Örneği mümkün olduğunca basit seviyede tutmamız önemli. Product Backlog Item ile Task öğelerine ait Title ve Description girişleri için PlainTextContentControl bileşeninden yararlanılmaktadır. Ve,

Kod

Kullanıcı Save işlemini icra ettiğinde (Ribbon kontrolüne basabilir, Ctrl+Save yapabilir vb), girdiği veri içeriğinin TFS tarafındaki ilgili projenin backlog’ una yazılması gerekmektedir. Bu nedenle odak noktası dokümanın BeforeSave olay metodudur. Lakin Client Object Model nesne referanslarının nasıl kullanıldığına da dikkat edilmelidir.

```csharp
using Microsoft.Office.Tools.Word; 
using Microsoft.TeamFoundation.Client; 
using Microsoft.TeamFoundation.WorkItemTracking.Client; 
using System;

namespace HowTo_TFSandWord 
{ 
    public partial class ThisDocument 
    { 
        #region Global değişkenler

        Uri tfsAddress = new Uri("http://tfsserver:8080/tfs/defaultcollection"); 
        TfsTeamProjectCollection collection = null; 
        WorkItemStore store = null; 
        Project argeProject = null; 
        WorkItemType witBacklogItem = null; 
        WorkItemType witTask = null; 
        WorkItemLinkTypeEnd linkTypeEnd = null;

        #endregion Global değişkenler

        private void InitializeTFSComponents() 
        { 
            collection = new TfsTeamProjectCollection(tfsAddress); 
            store = collection.GetService<WorkItemStore>(); 
            argeProject = store.Projects["ARGE"]; 
            witBacklogItem = argeProject.WorkItemTypes["Product Backlog Item"]; 
            witTask = argeProject.WorkItemTypes["Task"]; 
            linkTypeEnd = store.WorkItemLinkTypes.LinkTypeEnds["Parent"]; 
        }

        void ThisDocument_BeforeSave(object sender, SaveEventArgs e) 
        { 
            #region Yeni Bir Product Backlog Item Oluşturmak

            int createdBacklogItemId=CreateBacklogItem(textBacklogTitle.Text, textBacklogDescription.Text); 
            CreateTask(createdBacklogItemId, textTask1Title.Text, textTask1Description.Text); 
            CreateTask(createdBacklogItemId, textTask2Title.Text, textTask2Description.Text);

            #endregion Yeni Bir Product Backlog Item Oluşturmak 
        }

        private int CreateBacklogItem(string title,string description) 
        { 
            WorkItem newBacklogItem = new WorkItem(witBacklogItem); 
            newBacklogItem.Title = title; 
            newBacklogItem.Description = description; 
            newBacklogItem.Save(); 
            return newBacklogItem.Id; 
        }

        private void CreateTask(int createdBacklogItemId,string title,string description) 
        {   
            WorkItem newTask = new WorkItem(witTask); 
            newTask.Title = title; 
            newTask.Description = description;            
            newTask.WorkItemLinks.Add(new WorkItemLink(linkTypeEnd,  createdBacklogItemId)); 
            newTask.Save(); 
        }

        #region VSTO Designer generated code 
        
        private void InternalStartup() 
        { 
            this.Startup += new System.EventHandler(ThisDocument_Startup); 
            this.Shutdown += new System.EventHandler(ThisDocument_Shutdown); 
            this.BeforeSave += ThisDocument_BeforeSave; 
            InitializeTFSComponents(); 
        }

        #endregion

        private void ThisDocument_Startup(object sender, System.EventArgs e) 
        {

        }

        private void ThisDocument_Shutdown(object sender, System.EventArgs e) 
        { 
        } 
    } 
}
```

Öncelikli olarak TFS sunucusuna ve ilgili koleksiyona bağlanılması gerekir. Bu amaçla TfsTeamProjectCollection sınıfından yararlanılmaktadır. Dikkat edileceği üzere yapıcı metoda (Constructor) parametre olarak Default Collection için kullanılabilecek adres verilmiştir. Work Item’ lar üzerinde CRUD işlemlerini icra edebilmek için WorkItemStore servisine ulaşılması gerekmektedir. Bu sebepten collection değişkeni üzerinden GetService fonksiyonuna başvurulmuştur. Çok doğal olarak ARGE isimli örnek Team Project içerisine Work Item eklenmesi istenmektedir. Bu nedenle ilgili proje referansı, store değişkeni üzerinden Projects koleksiyonuna erişilerek elde edilmektedir. Gelelim Work Item oluşturma kısımlarına.

Product Backlog Item için CreateBacklogItem, Task içinse CreateTask isimli metodlar kullanılmaktadır. Aslında her ikisi de içerisinde bir WorkItem nesnesini örneklemekte ve ilgili özelliklerini set etmektedir. Ancak önemli bir fark vardır. Bir WorkItem nesne örneği üretilirken, WorkItemType tipinden bir parametre verilir. Bu parametre, ilgili Work Item öğesinin ne olacağını belirtir. Bir Task mı, bir Product Backlog Item mı, bir Bug mı vs. Bundan sonraki kısımlarda yer alan özellikler temel anlamda ortak sayılabilir. Title ve Description özelliklerine ilgili değerler set edildikten sonra, WorkItem nesne örneği üzerinden Save işleminin icra edilmesi yeterlidir.

WorkItem nesneleri kayıt edildiğinde Id özellikleri de, sunucu tarafından verilen değer ile otomatik olarak doldurulur. Nitekim Parent-Child ilişkinin oluşturulması noktasında, Parent Work Item’ ın Id değerinin bilinmesi önemlidir. Bu sebepten CreateBacklogItem metodu geriyer üretilen Work Item Id değerini döndürmektedir. Bu değer CreateTask fonksiyonu için bir girdidir. CreateTask metodunun en kritik noktası ise WorkItemLinks koleksiyonuna Parent tipte bir bağlantı tanımının eklenmesidir. Bunun için WorkItemLinkTypeEnd ve WorkItemLink tiplerinden yararlanılmıştır. Parametre olarak gelen integer değer bu senaryo da Parent olacak Product Backlog Item’ ı işaret etmektedir.

Testler

Artık uygulamayı test edebiliriz. Örnek olarak ben aşağıdaki ekran görüntüsünde yer alan içeriği oluşturdum.

[![tfsword_5](/assets/images/2013/tfsword_5_thumb.png)](/assets/images/2013/tfsword_5.png)

Şu aşamda Save işlemini icra ettiğimizde TFS tarafında aşağıdaki içeriklerin oluştuğuna şahit olabiliriz. Dikkat edileceği üzere “Müşteri karakteristiği oluşturma” isimli bir Product Backlog Item oluşturulmuş ve varsayılan olarak o anki güncel Sprint’ e ilave edilmiştir.

[![tfsword_6](/assets/images/2013/tfsword_6_thumb.png)](/assets/images/2013/tfsword_6.png)

Product Backlog Item açıldığında bir Id değeri aldığını ve Word dosyasında belirttiğimiz Description içeriğine sahip olduğunu da görebiliriz.

[![tfsword_7](/assets/images/2013/tfsword_7_thumb.png)](/assets/images/2013/tfsword_7.png)

Hatta Tasks kısmına geçtiğimizde, Child olarak bağladığımız Work Item öğelerini de görebiliriz. Dikkat edileceği üzere 6539 ve 6540 numaralı Task öğeleri Child olarak 6538 numaralı Product Backlog’ a eklenmiştir.

[![tfsword_8](/assets/images/2013/tfsword_8_thumb.png)](/assets/images/2013/tfsword_8.png)

Pek tabi ki bu Task örneklerine çift tıklandığında, Word dosyasında belirttiğimiz Title ve Description bilgilerine sahip olduklarını görebiliriz.

[![tfsword_9](/assets/images/2013/tfsword_9_thumb.png)](/assets/images/2013/tfsword_9.png)

[![tfsword_10](/assets/images/2013/tfsword_10_thumb.png)](/assets/images/2013/tfsword_10.png)

Her şey çok kolay görünüyor değil mi? Ama pek çok eksik ve tamamlanması gereken iş var. Bu işlerin tamamlanması da önemli bir development eforunu gerektirmekte.

Eksikler

Bu nokta da örneğimizin aslında sadece bir Hello World olduğunu ifade etmem gerekiyor. Sizin de fark edeceğiniz gibi bazı eksik noktalar var. Örneğin,

- Kullanıcı doküman içeriğini doldurmadan da göndermeyi deneyebilir. Bu durumda bir tedbir almak faydalı olacaktır. Hatta TFS’ in böyle bir durumda vereceği olası Exception tepkisine karşı bir geliştirme yapılmalıdır.
- Akıllı bir Save mekanizması gerekebilir. Aynı Title’ a sahip bir Work Item içeriğinin oluşturulmasının önüne kolayca geçilebilir (WorkItem listesini çek, Title’ larını karşılaştır) Ancak bire bir eşleşmeyen fakat aynı anlama gelebilen Title’ lar var ise, belki kullanıcıya bir pencere ile bildirimde bulunulup aralarında seçim yapması istenebilir.
- Örnekte sadece Title ve Description alanları doldurulmuştur. Oysaki bir Product Backlog Item veya Task için set edilmesi gereken daha pek çok özellik vardır. Söz gelimi kapasite planlaması ve Velocity’ nin çıkmasında önem arz eden Product Backlog Item’ ın Effort ve Business Value değerleri doldurulmamıştır. Benzer şekilde Task’ lar için de bir atama işlemi yapılmamıştır. Yani ilgili Task kime atanmıştır bilgisi eksiktir. (Bilindiği üzere Sprint planlama toplantılarında bu tip öğeler son derece dikkatli bir şekilde tespit edilmekte ve doldurulmaktadır)
- Öğeler varsayılan olarak kök Area altına atanmıştır. Ancak farklı bir Area içerisinde tutulması da istenebilir. Dolayısıyla bu bilginin dosya daha açılmadan çekilmesi ve hatta dokümanda belki de bir ComboBox ile gösterilerek seçtirilmesi düşünülmelidir (Area’ ya benzer şekilde Sprint ve hatta Iteration seçimler de yaptırılabilir)
- Örnekte sadece bir Product Backlog Item ve buna bağlı iki Task girilmesi senaryosuna yer verilmiştir. Oysa n adet giriş yapılabilir. Dolayısıyla daha dinamik bir içerik giriş alt yapısı hazırlanmalıdır (Bu noktada Excel’ in TFS ile olan varsayılan entegrasyonuna bakmanızı öneririm)
- Senaryomuzda ARGE isimli bir Team Project kullanılmaktadır. Pek ala Word dosyası açılırken kullanıcıdan bir proje seçmesi istenebilir. Bu doğal olarak bir arabirimi gerektirmektedir (Hatta bu arabirimde hangi Team Project’ in hangi Sprint’ ine içerik girileceği bilgisi dahi sorulabilir)

ve benzeri pek çok eksik bulunabilir. Ancak amacımıza ulaştığımızı ve her zaman ki gibi kapıyı sadece araladığımızı, ardına kadar açmak için sizin çaba sarf etmeniz gerektiğini hatırlatmak isterim. Böylece geldik bir yazımızın daha sonuna. Team Foundation Server ile ilişkili araştırmalarıma fırsat buldukça devam ediyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_TFSandWord.zip (208,21 kb)](/assets/files/2013/HowTo_TFSandWord.zip)