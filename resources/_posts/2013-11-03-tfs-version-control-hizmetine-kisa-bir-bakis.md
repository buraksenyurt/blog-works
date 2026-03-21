---
layout: post
title: "TFS Version Control Hizmetine Kısa Bir Bakış"
date: 2013-11-03 14:58:00 +0300
categories:
  - team-foundation-server
tags:
  - team-foundation-server
  - client-object-model
  - xml-web-service
  - version-control
---
TFS Web Services kullanımlarını incelediğimiz [bu](https://www.buraksenyurt.com/post/TFS-Web-Services-ve-Kullanimi) yazımızda, en popüler hizmetlerden birisi olan Work Item Tracking servisine odaklamıştık. Bu servisten yararlanarak özellikle template bazlı öğelerin (Task, Bug, Product Back Log Item gibi) nasıl okunabileceğini öğrenmiştik. Çok doğal olarak daha pek çok servis kullanımı söz konusu. Önemli olan nokta, ilgili servislerin TFS Client Object Model üzerinden kullanılabileceğidir.

[Yazı, Team Foundation Server 2012 üzerinde ele alınmıştır]

İşte bu yazımızda başka bir servis kullanımını incelemeye çalışıyor alacağız. Amacımız Version Control hizmetini ele alarak bir Team Project içerisinde yer alan ve Source Control’e dahil edilmiş çeşitli içerikleri elde edebilmek. Ağırlıklı olarak kod dosylarını görmeyi hedefliyoruz.

Örneğimizi bir Windows Forms uygulaması olarak geliştirebiliriz. Dilerseniz hiç vakit kaybetmeden ilgili TFS Assembly dosyalarını referans ederek işe başlayalım.(Tabi kaynak kodun versiyon kontrolü denilince aklımıza aşağıdakinden daha iyi bir çözüm geliyordur diye var sayıyorum)

[![versioncontrolfunny](/assets/images/2013/versioncontrolfunny_thumb.png)](/assets/images/2013/versioncontrolfunny.png)

Başlangıç

İlk olarak uygulamaya Microsoft.TeamFoundation.Client, Microsoft.TeamFoundation. Common ve Microsoft.TeamFoundation.VersionControl.Client assembly kütüphanelerini referans ederek işe başlayabiliriz. Örneğimize konu olan VersionControlServer tahmin edileceği üzere Microsoft.TeamFoundation.VersionControl.Client assembly’ ının bir parçasıdır.

[![tfsvc_1](/assets/images/2013/tfsvc_1_thumb.png)](/assets/images/2013/tfsvc_1.png)

Uygulamamıza ait Form ise aşağıdaki gibi tasarlanabilir.

[![tfsvc_2](/assets/images/2013/tfsvc_2_thumb.png)](/assets/images/2013/tfsvc_2.png)

Kullanıcılar app.config dosyasında belirtilen TFS sunucusuna bağlanabilecektir. Bu TFS sunucusu çok doğal olarak kendi içerisinde n sayıda Team Project Collection barındırabilir. İlgili Team Project Collection’ a ait bazı bilgilerin Collections bileşeni yanındaki ComboBox kontrolüne doldurulması sağlanmalıdır. Kullanıcı, bir Team Project Collection seçimi yaptığında ise, buna bağlı Team Project listesinin de ilgili ComboBox kontrolüne eklenmesi gerekmektedir. En azından bağlı olan Team Project adlarının listelenmesi gerekir. Kullanıcının yapacağı bir diğer seçim de, source control üzerinden çekilmek istenen dosyaların tipleridir. Örneğin C#, VB gibi kod dosyaları olabileceği gibi, XAML içerikli dosyalara da bakılmak istenebilir. Örnekte kısıtlı bir küme kullanılmıştır ancak bu genişletilebilir.

> Kendi çalışmalarınızı yaparken Team Foundation Server’ ın tfs.visualstudio.com adresinden sunulan hizmetini göz önüne almanızı ve o ortamda sunulan Code penceresini incelemenizi öneriririm. Code penceresine gelindiğinde aslında bu, bir Team Project Collection’ daki bir Team Project içerisindeyiz anlamına gelir. Dolayısıyla bu Team Project içerisindeyken source control üzerine atılan ne kadar içerik varsa görülebilir. Biz çok daha kısıtlı bir örneğini yapıyoruz. Aslında kapıyı azcık aralamak niyetindeyiz.
> [![tfsvc_5](/assets/images/2013/tfsvc_5_thumb.png)](/assets/images/2013/tfsvc_5.png)

Dosya tipi seçimi de belli olduğunda liste kontrolüne, söz konusu uzantıya sahip dosyaların bazı bilgileri gelecektir. Eğer herhangibir dosya seçilirse de, bu dosyanın içeriği gösterilecektir.

Yardımcı Sınıflar

Örnekte işleri biraz olsun kolaylaştırmak adına iki yardımcı POCO (PlainOldClrObject) tipi kullanılmıştır. Bu tipler içerisinde bir Team Project Collection’ ın ve Change Set’ in temel bilgileri tutulmaktadır.

[![tfsvc_3](/assets/images/2013/tfsvc_3_thumb.png)](/assets/images/2013/tfsvc_3.png)

TeamCollection sınıfı içeriği;

```csharp
using System;

namespace HowTo_TFSVersionControl 
{ 
    public class TeamCollection 
    { 
        public string DisplayName { get; set; } 
        public Guid Id { get; set; }

        public override string ToString() 
        { 
            return string.Format("[{0}]-{1}", Id.ToString(), DisplayName); 
        } 
    } 
}
```

ve ChangeSetItem sınıfı içeriği;

```csharp
using Microsoft.TeamFoundation.VersionControl.Client;

namespace HowTo_TFSVersionControl 
{ 
    public class ChangeSetItem 
    { 
        public ItemType ItemType { get; set; } 
        public string ServerItem { get; set; } 
        public int ItemId { get; set; } 
        public int ChangeSetId { get; set; }

        public override string ToString() 
        { 
            return string.Format("[{0}]-[{1}]-{2}" 
                , ChangeSetId.ToString() 
                , ItemId.ToString() 
                , ServerItem); 
        } 
    } 
}
```

Kod Tarafı

Dilerseniz öncelikle Form1’ e ait kod içeriklerini üretelim ve neler yaptığımızı açıklamaya çalışalım.

```csharp
using Microsoft.TeamFoundation.Client; 
using Microsoft.TeamFoundation.Framework.Client; 
using Microsoft.TeamFoundation.Framework.Common; 
using Microsoft.TeamFoundation.VersionControl.Client; 
using System; 
using System.Collections.Generic; 
using System.Configuration; 
using System.IO; 
using System.Windows.Forms;

namespace HowTo_TFSVersionControl 
{ 
    public partial class Form1 
        : Form 
    { 
        #region Common Fields

        TfsTeamProjectCollection tfs = null; 
        VersionControlServer vcServer = null; 
        ItemSet itemSet = null; 
        List<ChangeSetItem> csitems = null; 
        IReadOnlyCollection<CatalogNode> teamCollectionNodes = null; 
        TfsTeamProjectCollection selectedCollection = null;

        #endregion Common Fields     

        public Form1() 
        { 
            InitializeComponent();

            cmbFileTypes.Items.Add("cs|C# Files"); 
            cmbFileTypes.Items.Add("vb|Visual Basic Files"); 
            cmbFileTypes.Items.Add("xaml|XAML Files"); 
            
            txtTFSAddress.Text = ConfigurationManager.AppSettings["TfsAddress"]; 
            FillTeamCollections(); 
        }

        private void FillTeamCollections() 
        { 
            tfs = new TfsTeamProjectCollection(new Uri(txtTFSAddress.Text));

            teamCollectionNodes=tfs.ConfigurationServer.CatalogNode.QueryChildren( 
                new[] {CatalogResourceTypes.ProjectCollection }, 
                false, 
                CatalogQueryOptions.None);

            foreach (var teamCollectionNode in teamCollectionNodes) 
            { 
                TeamCollection teamCollection = new TeamCollection(); 
                teamCollection.DisplayName= teamCollectionNode.Resource.DisplayName; 
                teamCollection.Id = Guid.Parse(teamCollectionNode.Resource.Properties["InstanceId"]);

                cmbTeamCollections.Items.Add(teamCollection);                
            } 
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e) 
        { 
            ChangeSetItem selectedChangeSetItem=lstItems.SelectedItem as ChangeSetItem; 
            Item item=vcServer.GetItem(selectedChangeSetItem.ItemId, selectedChangeSetItem.ChangeSetId);

            using(Stream stream = item.DownloadFile()) 
            { 
                using(StreamReader reader = new StreamReader(stream)) 
                { 
                    txtItemContent.Text=reader.ReadToEnd(); 
                } 
            } 
        }

        private void cmbTeamCollections_SelectedIndexChanged(object sender, EventArgs e) 
        { 
            cmbTeamProjects.Items.Clear(); 
            lstItems.Items.Clear(); 
            TeamCollection selectedTeamCollection=cmbTeamCollections.SelectedItem as TeamCollection; 
            selectedCollection=tfs.ConfigurationServer. GetTeamProjectCollection(selectedTeamCollection.Id); 
            var teamProjectNodes=selectedCollection.CatalogNode.QueryChildren( 
                new[] { CatalogResourceTypes.TeamProject }, 
                false, CatalogQueryOptions.None); 
            
            foreach (var teamProjectNode in teamProjectNodes) 
            { 
                cmbTeamProjects.Items.Add(teamProjectNode.Resource.DisplayName); 
            }           
        }

        private void cmbTeamProjects_SelectedIndexChanged(object sender, EventArgs e) 
        { 
            lstItems.Items.Clear(); 
            txtItemContent.Clear();

            vcServer=selectedCollection.GetService<VersionControlServer>(); 
            string itemPath = string.Format("$/{0}/*.{1}" 
                , cmbTeamProjects.SelectedItem 
                ,cmbFileTypes.SelectedItem.ToString().Split('|')[0]); 
            itemSet = vcServer.GetItems(itemPath, RecursionType.Full); 
           csitems = new List<ChangeSetItem>();

            foreach (Item item in itemSet.Items) 
            { 
                ChangeSetItem csi = new ChangeSetItem();

                csi.ItemId = item.ItemId; 
                csi.ChangeSetId = item.ChangesetId; 
                csi.ItemType = item.ItemType; 
                csi.ServerItem = item.ServerItem;

                lstItems.Items.Add(csi); 
            }

            lblStatus.Text = itemSet.Items.Length.ToString(); 
        } 
    } 
}
```

TFS üzerindeki Collection nesnelerini elde etmek için TfsTeamProjectCollection örneği üzerinden önce ConfigurationServer, ardından da CatalogNode özelliğine gidilmekte ve Child Node içerikleri sorgulanmaktadır. Bu sorgu içerisinde CatalogResourceTypes enum sabitinin ProjectCollection değeri kullanıldığından, TFS sunucusu üzerindeki Team Project Collection’ ların içerikleri elde edilir.

Bir Team Project Collection sorgulanırken Guid tipinden olan ID değeri önem arz eder. Bu yüzden sorgu sonucu elde edilen Node’ lar arasında dolaşılırken Resorce özelliklerinden yararlanılarak iki değerin alınması sağlanmıştır. DisplayName ve Properties[“InstanceId”] yardımıyla Team Project Collection’ ı benzersiz olarak niteyen Guid değeri. İlgili özelliklerin toplandığı TeamCollection nesne örnekleri de cmbTeamCollections isimli ComboBox kontrolünün Items koleksiyonuna eklenmektedir.

Kullanıcı bu ComboBox bileşeninden bir öğe seçtiğinde ise, ilgili Team Project Collection altındaki Team Project örneklerinin, en azından adlarının çekilmesi gerekmektedir. Nitekim bu Team Project adı ilgili Version Control servisi tarafından, Change Set öğelerinin çekilmesi sırasında ele alınacaktır.

Seçilen öğenin karşılığı olan TfsTeamProjectCollection nesnesine ulaşmak için ConfigurationServer örneğine ait GetTeamProjectCollection metoduna başvurulur. Bu metod parametre olarak seçili öğenin Id değerini alır. Tahmin edeceğiniz üzere bu değeri, ComboBox içerisinde TeamCollection nesne örneklerinde birer özellik olarak kullanmıştık. Seçilen öğenin karşılığı olan TfsTeamProjectCollection yakalandıktan sonra ise CatalogNode üzerinden tekrar Child Node’ lara gidilecek şekilde bir çağrı yapılır.

Lakin bu kez CalatogResourceTypes enum sabitinin TeamProject değeri kullanılır. Buna göre seçili olan Team Project Collection’a bağlı olan Team Project listesi, birer CatalogNode olarak elde edilebilir. Sonrasında ise liste dönülür ve her bir Team Project’ in Resource özelliği üzerinden yakalanacak DisplayName değeri, cmbTeamProjects bileşeninin Items koleksiyonuna eklenir.

Version Control Servisinin Çalışması

Yazımıza konu olan Verison Control servisi ise bu noktadan sonra devreye girecektir. İlgili servis örneği kullanıcının yaptığı Team Project seçimi sonrasında önemlidir. Nitekim Change Set veri içeriklerinin elde edilmesi için ilgili operasyon desteğini sunmaktadır.

İlk olarak güncel Team Project Collection tespit edilerek generic GetService metoduna bir çağrıda bulunulur ve VersionControlServer tipinden bir referans alınır. Bu referansın GetItems metodu kilit noktamızdır. İlk parametre ile bir string verilmektedir ama yazım şekli özeldir.

$/{0}/.{1}

ifadesinde {0} yerine Team Project adı, {1} yerine ise talep edilen dosya formatının uzantısı gelir. GetItems metodu esas itibariyle bir ItemSet örneği döndürür ve aslında beklediğimiz dosyalar bu tipin Items koleksiyonunca alınır. Örnekte Items özelliği dolaşılmakta olup her biri için bir ChangeSetItem örneklenir. ChangeSetItem sınıfında bir kaç temel özellik bulunmaktadır. ItemId, ChangeSetId, ItemType ve ServerItem.

Bu aşamadan sonra kullanıcının bir ChangeSetItem’ ı listeden seçmesi halinde içeriğinin gösterilmesi adımına gelinir. Bu sefer de VersionControlServer örneğine ve GetItem isimli metoduna başvurulur. Örnekte GetItem metoduna iki parametre girmektedir. Item ve Change Set numaraları. Çok doğal olarak bir Change Set altında farklı numaralara sahip birden fazla öğe yer alabilir. Bu sebepten hangi Change Set’teki hangi öğeyi alacağımızı bildirmemiz gerekir.

Elde edilen Item öğesinin DownloadFile isimli bir fonksiyonu da bulunmaktadır. Bu metod sayesinde, sunucudaki içeriğin Stream olarak elde edilmesi mümkündür. Örnek kod parçasında StreamReader sınıfından yararlanılmış ve ilgili sunucu içeriğinin txtItemContent isimli TextBox kontrolüne basılması sağlanmıştır.

Çalışma Zamanı Sonuçları

Örneği çalıştırdığımızda ve sırasıyla Team Project Collection, File Type ve Team Project seçimlerine yaptığımızda, aşağıdakine benzer sonuçlar ile karşılaşabiliriz.

[![tfsvc_4](/assets/images/2013/tfsvc_4_thumb.png)](/assets/images/2013/tfsvc_4.png)

Dikkat edileceği üzere Default Collection altındaki bir Team Project’ in içerisinde yer alan C# kod dosyalarının temel bilgileri çekilebilmiştir. Bu bilgiler arasında, Item ve Change Set numarası ile Server üzerinde tutulan Full Path bilgisi yer almaktadır. Hatta istenirse son Check-In bilgisi bile alınabilir (Belki de alınamaz. Neden araştırmıyorsunuz ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_197.png)) İşin güzel yanı söz konusu öğelerden herhangibirisine tıklandığında, içeriğinin de görüldüğüdür.

Uygulamada Neleri Yapmadık?

Version Control hizmetini kullanarak, Source Control içeriğini çekelebilmek oldukça kolaydır. Bu felsefeden yola çıkarak kendi Source Control arabirimlerinizi yazabilirsiniz. Örneğin Windows Phone tabanlı çalışacak bir istemci söz konusu olabilir. Yani mobil bir cihazdan Source Control üzerindeki hareketlilikleri takip edebilirsiniz. Pek tabi yapabileceğimiz daha pek çok şey var. Örneğin,

- Source Control’ den çekilen bir içeriği güncelleyebilir ve sunucuya kaydedebiliriz.
- Yeni bir öğeyi Source Control’a ekleyebiliriz.
- Bir öğeyi silebiliriz.
- Klasör veya Branch gibi öğeler oluşturabiliriz.

ve benzerleri…

Bu konuları örneğimizde ele almadık ancak VersionControlServer tipinin işaret ettiği servis metodlarını inceleyerek nasıl yapabileceğinizi araştırmaya başlayabilirsiniz. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_VersionControl.zip (218,31 kb)](/assets/files/2013/HowTo_VersionControl.zip)