---
layout: post
title: "Ado.Net Senkronizasyon Servisleri(Sync Services for Ado.Net)"
date: 2009-01-03 10:00:00 +0300
categories:
  - ado-net-sync-services
tags:
  - ado-net-sync-services
  - csharp
  - dotnet
  - ado-net
  - linq
  - sql-server
  - wcf
  - xml
  - http
  - concurrency
  - caching
  - transactions
  - generics
  - visual-studio
  - dataset
  - datatable
  - shared-state
---
Birbirleri ile sürekli bağlantı halinde olamayan istemci/sunucu (Client/Server) mimarilerinde en büyük problemlerden biriside verilerin karşılıklı veya tek taraflı olaraktan senkronize edilmeleridir. Çoğu büyük çaplı saha uygulamasında, sunucu tarafındaki veri kaynaklarının istemcide kullanıldığı durumlar söz konusudur. Bu noktada istemcilerin sürekli bağlı kalamadıkları bir ortamın var olması olasıdır (Occasionally Connected Enivronments). Nitekim istemci ve sunucu arasında kablosuz bağlantı olma ihtimali oldukça yüksektir. Nitekim günümüz teknolojileri düşünüldüğünde istemci uygulamaların bir çoğu mobil cihazlar ile, diz üstü bilgisayarlar üzerinde koşmaktadır. Bu tabiki daha çok saha elemanlarının işin içerisine girdiği senaryolardır.

Söz gelimi günlük ürün fiyatlandırmalarını kullanarak satış yapan personelin mobil cihazlara sahip olduğu bir durumda, istemcilerin sunucuya sürekli olarak bağlı kalmaları saha üzerinde son derece zor olabilir. (Mobil cihazlar, modern cep telefonları ve PDA'ler olabileceği gibi dizüstü bilgisayarlarda olabilir.) Buna rağmen istemcinin söz konusu veriyi kullanarak çalışabilmesi de istenebilir. Diğer taraftan istemcinin sunucuya bağlandığı hallerde ortak veri üzerindeki değişikliklerini göndermesi ve hatta var olan farklılıkları kendi sistemine çekmeside istenebilir.

Bu iki taraflı bir senkronizasyon anlamına gelmektedir ki, iki tarafında birbirleri üzerindeki verileri senkronize etmeleri sırasında pek çok güçlükle karşılaşılacaktır. Nitekim bu vakada eş zamanlı bağlı olan kullanıcıların ortak verilerde yapacağı değişikliklerin ele alınması gerekir ki bunlar çakışmalara (Conflicts) neden olmaktadır. Bazı durumlarda ise sadece sunucudaki farklılıkların istemciye aktarılması veya tam tersi söz konusudur. Bu durumlarda senkronizasyonu yönetmek nispeten biraz daha kolaydır. Ancak hangi vaka olursa olsun her iki uçta yer alan verinin kolay kodlanabilir, yönetilebilir bir biçimde senkronize edilmeleri istenir. En azından geliştiriciler için bu önemlidir.

> Ado.Net senkronizasyon servisleri esas itibariyle Microsoft Sync Framework (MSF) altyapısının bir parçasıdır. MSF altyapısı içerisinde Sync Services for File Systems ve Sync Services for FeedSync isimli iki alt açılım daha vardır. Sync Services For Ado.Net özel olarak istemci ve sunucu arasındaki veri senkronizasyonun sağlanmasında Ado.Net'in üstüne gelmiş olan bir tamamlayıcı olarak düşünülebilir. (Microsoft Sync Framework için [MSDN](http://msdn.microsoft.com/en-us/sync/default.aspx)'den detaylı bilgi alınabilir.)

Senkronizasyon işlemlerinde bilinen ve kullanılan farklı teknikler de söz konusudur. Örneğin Remote Data Access (RDA) veya Merge Replication. RDA, SQL Server Compact 3.5 ile diğer bir SQL veritabanı arasındaki senkronizasyon işlemlerinde ele alınır. Merge Replication ise SQL veritabanlarının herhangi versiyonları arasındaki senkronizasyon süreçlerinde kullanılır. Özellikle Merge Replication veritabanı yöneticilerine hitap eder ve SQL kaynaklarını hedefler.

Ancak ADO.Net Senkronizasyon Servisleri ile WCF (Windows Communication Foundation) hizmetlerini kullanarak, sunucu tarafında farklı veri kaynaklarına erişebilmek mümkündür. Diğer taraftan Ado.Net Senkronizasyon Servisleri daha çok uygulama geliştiricileri hedef alır. Eğer istemci tarafının senkronize edeceği veri kümesi SQL dışında bir kaynak ise mutlaka Ado.Net Senkronizasyon Servisi göz önüne alınmalıdır. RDA, Merge Replication ve Ado.Net Senkronizasypn Servisleri arasındaki karşılaştırmaları aşağıdaki tablodan da inceleyebilirsiniz. Özellikle karar verme aşamasında bu tablodaki bilgilerden de yararlanılabilir.

Anahtar Özellik
Kullanılabilen Teknikler

RDA
(Remote Data Access)
Merge Replication
Ado.Net Sync Services

Servisler üzerinden senkronizasyon sağlanması
Yok
Yok
Var

Farklı veri kaynakları için destek
Yok
Yok
Var

Değişimsel (Incremental) farklılıkların takibi
Yok
Var
Var

Çakışma (Conflict) kontrolü ve çözümleri
Yok
Var
Var

İstemci tarafında View'ların kolayca oluşturulması (Görsel derslerde ele alınacaktır)
Yok
Yok
Var

Otomaik şema (Schema) ve veri oluşturma
Var
Var
Var

Büyük boyutlu DataSet desteği
Var
Var
Var

Otomaik olarak şema değişikliklerini üretmek
Yok
Var
Yok

Veriyi tekradan birleştirmek (Repartition)
Yok
Var
Yok

RDA değişimsel upload'ları destekler. Verinin istemci tarafına alınmasında Snapshot modelini kullanılır. Yani istemci tarafına tüm veriyi indirir.

Teknik açıdan bakldığında Ado.Net Senkronizasyon Servisleri temel olarak aşağıdaki 3 assembly'dan oluşmaktadır. Ado.Net Senkronizasyon Servisleri tarafların sahip olduğu veri sağlayıcılarına göre 2 katlı (Two Tier), N-katlı (N-Tier) ve Servis Bazlı Mimariye (Service Oriented Architecture) uygun olacak şekilde kullanılabilmektedir. Eğer istemci ve sunucu Ado.Net veri sağlayıcıları üzerinden konuşuyorlarsa iki katlı veya n katlı modeller tercih edilebilir. Ancak sunucu tarafından SQL dışı bir veri kaynağı var ise (Söz gelimi bir XML deposu, Active Directory vb...) bu durumda servis yönemlimli olacak şekilde bir geliştirme yapılmalıdır. Senkronizasyon her zaman için istemci tarafında başlatılan bir olaydır ve temel olarak 4 farklı tipte senkronizasyon tekniği kullanılmaktadır.

Kullanılan Teknik
Açıklama

Snapshot
Bu teknikte senkronizasyon işlemi başlatıldığında sunucu tarafındaki verinin tamamı istemci tarafına indirilir. Bir başka deyişle değişimsel (incremental) farklılıklar göz önüne alınmaz, sürekli olarak son hal indirilir.

Download-Only
Bir önceki senkronizasyona göre farklı olan verilerin download edilmesi söz konusudur. Bir başka deyişle sadece değişimsel verilerin indirilmesi söz konusudur.

Upload-Only
Senkronizasyon işleminde istemci tarafındaki veriler üzerinde yapılan değişiklikler ve yeni eklemelerin sunucu tarafına taşınması söz konusudur. Söz gelimi satış ekibinin herhangibir ürün satışı için yaptığı girişler veya güncellemeler buna örnek bir vaka olarak düşünülebilir.

Bidirectional
Çift yönlü senkronizasyon söz konusudur. Söz gelimi bir kargo dağıtım firmasının saha elemanlarının dağıtılacak kargo bilgilerini alması ve dağıtıma ait bilgileri sunucu üzerine güncellemesi gibi bir vaka örnek olarak düşünülebilir. Bu noktada özellikle senkronizasyon işlemleri sırasında oluşabilecek eş zamanlı veri çakışmalarının (Conflicts) kontrol altına alınması gerekir.

Bu noktada belkide senkronizasyon servislerinin katlı mimarideki konumlarını ele almak yararlı olabilir. Bu amaçla aşağıdaki çizelgelerden yararlanabiliriz.

2 Katlı Mimari;

![mk265_1.gif](/assets/images/2009/mk265_1.gif)

İki katlı modelde sunucu ve istemci tarafında senkronizasyon işlemine tabi olan nesneler bazen aynı uygulama üzerinde bulunmaktadır. Her ne kadar çok fazla tavsiye etmesemde iki katlı modelin uygulanması özellikle Visual Studio 2008 ortamında son derece kolaydır. Ancak gerçek hayat uygulamalarında çoğunlukla sunucu senkronizasyonunu bir servis veya başka bir uygulama tek başına üstlenir.

N-Katlı Mimari;

![mk265_2.gif](/assets/images/2009/mk265_2.gif)

N-katlı modelde istemci ve sunucu üzerindeki veritabanları arasındaki iletişim sırasında devreye giren ve ayrı bir katta duran Servis-Proxy tipleri mevcuttur. Genellikle istemci ve sunucu veribanları arasında doğrudan bağlantıyı gerektirmediği için 2 katlı mimariye göre daha fazla tercih edilmektedir.

Servis Bazlı Mimari;

![mk265_3.gif](/assets/images/2009/mk265_3.gif)

SOA modeli sunucu tarafındaki veri kaynağının SQL olmadığı durumlarda ele alınabilir. Bu sebepten dolayı sunucu tarafında sunucu senkronizasyon sağlayıcısı veya senkronizasyon adaptörleri bulunmamaktadır. Bu mimaride istemcinin sunucu tarafı ile mutlak suretle bir servis üzerinden konuşuyor olması gerekmektedir. Bir başka deyişle sunucu senkronizasyon sağlayıcısı ile senkronizasyon adaptörlerinin görevini servis tarafı üstlenmektedir. Bu noktada servis tarafında WCF gibi gelişmiş modellerin kullanılmasıda mümkündür.(İlerleyen görsel derslerimizde bu konuyuda incelemeye çalışıyor olacağız.)

> .Net Framework tarafından bakıldığında Ado.Net Sync Service'ler aşağıdaki 3 temel assembly ve tiplerinden yaralanmaktadır.
> - Microsoft.Synchronization.Data.dll assembly (Synchronization Agent, Synchronization Tables ve Synchronization Groups)
> - Microsoft. Synchronization.Data.SqlServerCe.dll (Client Synchronization Provider)
> - Microsoft. Synchronization.Data.Server.dll (Server Synchronization Provider ve Synchronization Adapters)
> ![mk265_4.gif](/assets/images/2009/mk265_4.gif)
> (Tabi Sync Service For Ado.Net'i kullanabilmek için ilgili [sürümünü](http://www.microsoft.com/downloads/details.aspx?familyid=75FEF59F-1B5E-49BC-A21A-9EF4F34DE6FC&displaylang=en) indirip kurmanız gerekmektedir. Makalenin yazıldığı tarihten sonra farklı versiyonların çıkması ve muhtemel değişimlerin olmasınında söz konusu olduğunu belirtmek isterim. Makalemizde Microsoft Sync Framework 2.0 CTP sürümü içerisindeki Sync Services For Ado.Net alt yapısı kullanılmaktadır.)

Makalemizin bundan sonraki bölümünde teknik detayları bir kenara bırakıp çok basit bir örnek üzerinden konuyu daha net bir şekilde kavramaya çalışacağız. Örnekte kullanılmakta olan Windows uygulaması üzerinde, Azon isimli örnek veritabanında yer alan Kitap isimli tablo için çift yönlü (Bidirectional) senkronizasyon işlemleri yapılmaktadır. Ancak elbette istediğiniz tipte bir veritabanı ve tablolarını kullanabilirsiniz. Tablomuzun ilk hali aşağıdaki şekilde görüldüğü gibidir ve bu noktadaki hali oldukça önemlidir.

![mk265_5.gif](/assets/images/2009/mk265_5.gif)

Biraz sonra tablonun şu anki alan yapısının senkronizasyon desteği için değiştiğini göreceğiz:) Şimdi örnek Windows uygulamamıza Local Database Cache isimli yeni bir şabloun öğe (Template Item) ekliyoruz.

![mk265_6.gif](/assets/images/2009/mk265_6.gif)

Sync uzantılı LocalAzon isimli öğe temel olarak senkronizasyon işlemleri ile ilişkili ayarları tutacaktır. Örneğin senkronizasyon tablolarına ait şema (Schema) bilgileri, bağlantı (Connection) ayarları, kodlama kısımları vb... Bu nedenle ilk olarak karşımıza aşağıdaki pencere çıkacaktır.

![mk265_7.gif](/assets/images/2009/mk265_7.gif)

Burada ilk etapta sunucu veri kaynağı ve istemci veri kaynağına ait bağlantılar belirtilir. Dikkat edileceği üzere sunucu bağlantısı Sql Server (ki örnekte SQL Server 2008 üzerinde durmaktadır) üzerindeki veritabanını işaret etmektedir. İstemci bağlantısında ise sdf uzantısı ile dikkat çeken ve biraz sonra oluşturulacak olan Sql Server Compact 3.5 sürümünde bir veritabanı dosyası adı bulunmaktadır. Advanced kısmına baktığımızda ise senkronizasyon için transaction tanımlaması yapılabildiği de dikkati çekmektedir. Yani tüm tabloların senkronizasyon işlemlerinin ortak bir transaction içerisinde gerçekleştirilmesi isteği belirtilebilmektedir. Diğer taraftan önemli noktalardan biriside sunucu ve istemci proje lokasyonlarıdır.

Şu an itibariyle her iki değerde geliştirmekte olduğumuz projeyi işaret etmektedir. Elbetteki gerçek hayat vakalarında özellikle sunucu proje lokasyonu başka bir uygulamayı işaret etmektedir.(N-Tier veya SOA uyarlaması). Bu adımı tamamlamadan önce sol taraftaki Application kısmına yeni bir offline table eklenmesi gerekmektedir. Bu amaçla, Add düğmesine basıldıktan sonra aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![mk265_8.gif](/assets/images/2009/mk265_8.gif)

Bu kısımda senkronizasyon sürecine dahil olacak tablo (tablolar) veya veritabanı nesneleri seçilir. Dikkat edileceği üzere verinin istemci tarafına indirilme şekli belirlenebilmektedir. Şu andaki seçime göre ilk senkronizasyon işleminden sonra değişimsel (Incremental) ve yeni eklemelerin indirilmesi seçeneği etkindir.

Senkronizasyon işlemleri sırasındaki önemli noktalardan biriside, istemci ve sunucu arasındaki bağlantının tekrardan sağlanması sonrasında karşılıklı olarak verilerde yapılan işlemlerin nasıl ayırt edilebileceğidir. Söz gelimi yeni güncelleştirmeler, silmeler veya eklemeler nasıl ayırt edilebililr. Bu sebepten update işlemleri için varsayılan olarak LastEditDate, insert işlemleri için CreationDate isimli iki yeni alanın Kitap isimli tabloya eklenmesi söz konusudur. Diğer taraftan silinen satırlar Tombstone uzantılı bir tabloda saklanacaktır ve bu şekilde takip edilebilecektir. New veya Edit düğmelerinden yararlanarak söz konusu parametrelerin farklı isimlerde oluşturulmalarıda sağlanabilir. Burada geliştiricilerin hayatını kolaylaştıran bir linkte bulunmaktadır. Tablo eklenmesi tamamlandıktan sonra Configure Data Synchronization penceresinde yer alan Show Code Example linkine tıklandığında örnek bir kod parçası görülür.

![mk265_9.gif](/assets/images/2009/mk265_9.gif)

Bu kod parçası senkronize işleminin istemci tarafında başlatılacağı yerde kolayca kullanılabilmektedir ki biz de öyle yapıyor olacağız:) Configure Data Synchronization pencersinden çıkılırken istenirse senkronizasyon işlemleri için kullanılacak SQL Script'lerinin istemci uygulamaya eklenmesi sağlanabilir. Bunun için aşağıdaki penceredeki seçenekleri varsayılan halleri ile bırakmak yeterli olacaktır.

![mk265_10.gif](/assets/images/2009/mk265_10.gif)

Artık istemci tarafı için gerekli olan türlendirilmiş (Typed) DataSet, DataTable ve DataAdapter üretimleri yapılacaktır. Bunun için var olan Kitap tablosunun seçilmesi yeterlidir.

![mk265_11.gif](/assets/images/2009/mk265_11.gif)

Senkronizasyon kodlarını eklemeden önce, istemci uygulamada ve sunucu veritabanında olan düzenleme ve ilaveleri analiz etmeye başlayabiliriz. İlk dikkat çekici nokta sunucu üzerindeki Azon veritabanında olan ilavelerdir.

![mk265_12.gif](/assets/images/2009/mk265_12.gif)

Görüldüğü üzere güncelleme ve ekleme işlemlerinin takibi için Kitap tablosuna LastEditDate ve CreationDate isimli datetime tipinden iki alan eklenmiştir. Silinen satırların bilgisi için KitapTombstone isimli bir tablo oluşturulmuştur. Bu tablo KitapId ve DeletionDate isimli alanları içermektedir. Böylece hangi satırın ne zaman silindiği bilgisi tutulabilmektedir. Diğer taraftan Insert, Update ve Delete işlemlerinden sonra devreye giren tetikleyicilerinde (triggers) eklendiği görülebilir. Triggerların içerikleri ve ne iş yaptıkları kısaca aşağıdaki tabloda açıklanmaktadır.

Trigger
Query
Görevi

KitapDeletionTrigger
ALTER TRIGGER [dbo].[Kitap_DeletionTrigger]
ON [dbo].[Kitap]
AFTER DELETE
AS
SET NOCOUNT ON
UPDATE [dbo].[Kitap_Tombstone]
SET [DeletionDate] = GETUTCDATE ()
FROM deleted
WHERE
deleted.[KitapId] = [dbo].[Kitap_Tombstone].[KitapId]
IF @@ROWCOUNT = 0
BEGIN
INSERT INTO [dbo].[Kitap_Tombstone]
([KitapId], DeletionDate)
SELECT [KitapId], GETUTCDATE () FROM deleted
END
Kitap tablosunda bir satır silindiğinde KitapTombstone tablosunda silinen kayıdın var olup olmaması durumuna göre (@@ROWCOUNT değeri) ya DeletionDate alanın güncellemesi yapılır yada KitapTombstone tablosuna silinen kayıt eklenir.

KitapUpdateTrigger
ALTER TRIGGER [dbo].[Kitap_UpdateTrigger]
ON [dbo].[Kitap]
AFTER UPDATE
AS
BEGIN
SET NOCOUNT ON
UPDATE [dbo].[Kitap]
SET [LastEditDate] = GETUTCDATE () FROM inserted
WHERE inserted.[KitapId] = [dbo].[Kitap].[KitapId]
END;
Kitap tablosundan bir satır güncellendiğinde, o satırın LastEditDate alanına anlık zaman değeri atanır.

KitapInsertTrigger
ALTER TRIGGER [dbo].[Kitap_InsertTrigger]
ON [dbo].[Kitap]
AFTER INSERT
AS
BEGIN
SET NOCOUNT ON
UPDATE [dbo].[Kitap]
SET [CreationDate] = GETUTCDATE () FROM inserted
WHERE inserted.[KitapId] = [dbo].[Kitap].[KitapId]
END;
Kitap tablosuna yeni bir satır eklendikten sonra bu satırının CreationDate alanına o anki zaman değeri atanır.

Peki ya uygulama tarafındaki değişiklikler nelerdir?

![mk265_13.gif](/assets/images/2009/mk265_13.gif)

Görüldüğü üzere Sync Services for Ado.Net için gerekli olan Microsoft.Synchronization.Data, Microsoft.Synchronization.Data.Server ve Microsoft.Synchronization.Data.SqlServerCe assembly'ları projeye referans olarak gelmektedir. Tüm senkronizasyon alt yapısına ait tipler bu assembly'lar içerisinde gelmektedir. Diğer taraftan istemci uygulamada yerel bir sdf dosyasıda oluşturulmuştur. Bu dosya local olarak çalışabilen Sql Server Compact 3.5 versiyonunda bir veritabanıdır. İstemci uygulamadaki görsel veri bağlı bileşenler için türlendirilmiş DataSet içeriği AzonDataSet.xsd adıyla oluşturulmuştur. Senkronizasyon ayarlarını ve işlemlerini üstlenen tipleri LocalAzon.sync öğesi barındırmaktadır. Bunlara ek olarak istemci tarafı için üretilen tiplere baktığımızda aşağıdaki sınıf diagramında yer alan temel öğeler dikkat çekmektedir.

![mk265_15.gif](/assets/images/2009/mk265_15.gif)

Buradaki tiplerin temel işlevleri aşağıdaki tabloda belirtilmektedir.

Kullanılan Sınıf
Açıklama

DbServerSyncProvider
Microsoft.Synchronization.Data.Server.dll assembly'ı içerisinde yer alan bu sınıf ServerSyncProvider tipinden türemektedir.
Sunucu üzerindeki senkronizasyon tablolarının bilgilerinin tutulması, sunucu üzerindeki verilerde son senkronizasyondan sonra olan değişikliklerin elde edilmesi, sunucu veritabanına değişimsel (incremental) farklılıkların aktarılması, çakışmaların (Conflicts) kontrol edilmesi gibi işlemleri üstlenir.

SqlCeSyncProvider
Microsoft.Synchronization.Data.SqlServerCe.dll assembly'ı içerisinde yer almaktadır.
İstemci tarafında senkronizasyona dahil edilmiş tabloların bilgilerinin saklanması, istemci veritabanındaki son senkronizasyondan sonra olan değişikliklerin elde edilmesi, istemci veritabanına değişimsel farklılıkların aktarılması, çakışmaların tespit edilmesi gibi kritik ve önemli işlemleri üstlenir.

SyncAdapter
Microsoft.Synchronization.Data.Server.dll assembly'ı içerisinde yer alan bu sınıf DbServerSyncProvider ile sunucu veritabanı arasında köprü vazifesi görmektedir.
Senkronize işleminde ele alınan her tablo için bu tipten türeyen bir sınıf üretilir. Bu adaptör nesneleri, senkronizasyonun tipine göre gerekli olan DbCommand örneklerini bir başka deyişle SQL sorgularını içerir.

SyncAgent
Microsoft.Synchronization.Data.dll assembly'ı içerisinde yer almaktadır. Tüm senkronizasyon sürecinin orkestrasyonunu üstlenmektedir.

SyncTable
Microsoft.Synchronization.Data.dll assembly'ı içerisinde bulunmaktadır.
Senkronizasyon işlemine tabi olan tüm tablolar için istemci tarafında birer adet oluşturulur. SyncAgent tipi içerisinde Nested Type (Dahili Tip) şeklinde oluşturulmaktadır. Senkronizasyona dahil olan istemci tablolarına ait ayarları taşımak gibi görevleri vardır.

Makalede geliştirdiğimiz örnekte sunucu tarafı senkronizasyon tiplerininde istemci uygulama üzerinde oluşması normaldir. Nitekim, Configura Data Synchronization kısmındaki Application ayarlarına bakıldığında istemci ve sunucu uygulamaların aynı olduğu görülmektedir. Ancak gerçek vakalarda sunucu veri kaynağı ile iletişimi sağlayan ayrık bir uygulamanın servis bazlı olması söz konusudur ki bu durumda N-Tier veya SOA mimarisine geçilmiş olmaktadır.

Testlere başlamadan önce istmeci uygulamanın tasarımını basit olarak aşağıdaki gibi değiştirelim.

![mk265_14.gif](/assets/images/2009/mk265_14.gif)

Burada GridView kontrolü otomatik olarak AzonDataSet içerisindeki Kitap tablosuna bağlıdır ve üzerinde bağlantısız olarak veri ekleme, çıkarma, güncelleştirme işlemleri yapılabilmektedir. Diğer taraftan Senkronize Et başlıklı düğmenin içeriği aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void kitapBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.kitapBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.azonDataSet);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.kitapTableAdapter.Fill(this.azonDataSet.Kitap);
        }

        private void btnSenkronizeEt_Click(object sender, EventArgs e)
        {
            LocalAzonSyncAgent syncAgent = new LocalAzonSyncAgent();
            Microsoft.Synchronization.Data.SyncStatistics syncStats = syncAgent.Synchronize();
            Form1_Load(null, null);
        }
    }
}
```

Tahmin ettiğiniz gibi biz sadece button içeriğini ekliyoruz. Burada ilk olarak bir Agent nesnesi örnekleniyor. Sonrasında ise Synchronize metodu ile senkronizasyon işlemi başlatılıyor. Bu işlemin sonuçlarını istersek üretilen SyncStatistics tipi üzerinden elde edebiliriz ki bunu loglama amacıyla kullanabiliriz. Yaptığımız bu değişiklikler sonrasında uygulamayı test ettiğimizde özellikle çift yönlü olarak bir senkronizasyon işlemi yapılamadığını göreceğiz. Bu sorunu çözmek için aşağıdaki kod parçasında da görüldüğü gibi LocalKitap.sync kod dosyasının içeriğini değiştirmemiz ve senkronizasyon tipini söz konusu Kitap tablosu için belirtmemiz gerekmektedir.

```csharp
namespace ClientApp { 

public partial class LocalAzonSyncAgent {
    partial void OnInitialized(){
           Kitap.SyncDirection = Microsoft.Synchronization.Data.SyncDirection.Bidirectional;
        }
    }
}
```

Artık Kitap tablosu için çift yönlü olaraktan senkronizasyon kontrolü yapılacaktır. Bir başka deyişle hem istemci hemde sunucu tarafındaki değişiklikler senkronizasyon işlemleri sonrasında karşı tarafa iletilecek ve verilerin eşlenmesi sağlanacaktır. Elbette birden fazla tablo kullanılması halinde bu tabloların her biri için ayrı ayrı senkronizasyon yönleri seçilebilir.

Artık kısa bir kaç test yapılabilir. İlk olarak istemci uygulama çalıştırıldığında Kitap tablosunun tüm içeriğinin çekildiği gözlemlenecektir. Burada tüm verinin indirilmesi son derece normaldir. Program çalıştıktan sonra örneğin, istemci tarafındaki veri içeriğinde çeşitli değişiklikler yaptığımızı varsayalım. Örneğin KitapId alanının değeri 4 olan satırı sildiğimizi, yeni bir kitap eklediğimizi ve 83 numaralı kitabın adı için bir güncelleştirme yaptığımızı düşünelim.

![mk265_16.gif](/assets/images/2009/mk265_16.gif)

Bu işlemlerin arkasından değişiklikleri DataSet üzerinde kaydedip Senkronize Et düğmesine basarsak, farklılıkların sunucu tarafınada aktarıldığını görebiliriz. Bu noktada özellikle LastEditDate ve CreationDate alanlarının istemci ve sunucudaki değerleri farklılıkların tespitini kolaylaştırmaktadır. Bunu daha rahat görebilmek amacıyla Azon.sdf dosyası içerisindeki Kitap tablosu ile sunucudaki Azon veritabanında yer alan Kitap tablosunun içeriklerini karşılaştırmanızı öneririm.

![mk265_17.gif](/assets/images/2009/mk265_17.gif)

Yeni kayıt ekleme ve güncelleme işlemleri dışında istemci tarafında birde satır silmiştik. Bu nedenle sunucu veritabanındaki KitapTombstone tablosunda aşağıdaki şekildende anlaşılacağı üzere 4 numaralı satır (Silinen kitabın KitapId değeri) için bir ekleme yapıldığı gözlemlenebilir.

![mk265_18.gif](/assets/images/2009/mk265_18.gif)

İkinci test olarak sunucu üzerinde değişikliker yapıp bunları istemci tarafına, uygulamadaki Senkronize Et düğmesini kullanarak alabiliriz. Şu nokta unutulmamalıdır; Senkronize etme işlemi program her açıldığında yapılmamaktadır. Nitekim veritabanında değişiklik yapılsa ve istemci ile sunucu arasında bir bağlantı olmasa bile, istemci uygulama yerel veritabanı (Local Database-sdf) üzerindeki veriler ile çalışarak, değişiklikler, eklemeler ve silmeler yapabilir ama, senkronize etme işlemi başlamadığı sürece hem değişiklikleri alamaz hem de bunları sunucu veritabanına iletemez.

Bir açıdan bakıldığında, yapılan işlemler verilerin normal yollarla karşılıklı olarak uç kaynaklara gönderilmesinden pek farklı değildir. Özellikle bağlantısız katman modelinde geliştirme yapıldığında benzer işlevsellikleri sağlayabiliriz. Ne varki Sync Services for Ado.Net alt yapısı sunucu ve istemci tarafındaki senkronizasyonun sağlanması adına geliştiriciye daha güçlü ve yönetilebilir tipler sunmaktadır. Ayrıca senkronize işlemlerinin yapılması sırasında sunucu veri kaynağının sabit bir SQL veritabanı olması şartı bulunmamakla birlikte, N-Tier yada SOA tabanlı geliştirme yapmakta mümkündür. Bu açılardan bakıldığında avantajlar ortadadır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örneği İndirmek İçin Tıklayın](/assets/files/2009/AzonSync.rar)