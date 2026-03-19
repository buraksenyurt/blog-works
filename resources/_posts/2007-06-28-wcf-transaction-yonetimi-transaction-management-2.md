---
layout: post
title: "WCF - Transaction Yonetimi (Transaction Management) - 2"
date: 2007-06-28 12:00:00 +0300
description: "Windows Communication Foundation için transaction yönetimi ile ilgili bir önceki makalemizde teorik bilgiler üzerinde durmaya çalışmıştık. Bu makalemizde ise, transaction yönetimi için gerekli materyalleri toplamaya devam edecek ve bir örnek üzerinde konuyu daha net bir şekilde anlamaya çalışacağız."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - sql-server
  - soap
  - http
  - transactions
---
# WCF - Transaction Yonetimi (Transaction Management) - 2
Değerli Okurlarım Merhabalar,

Windows Communication Foundation için transaction yönetimi ile ilgili bir önceki makalemizde teorik bilgiler üzerinde durmaya çalışmıştık. Bu makalemizde ise, transaction yönetimi için gerekli materyalleri toplamaya devam edecek ve bir örnek üzerinde konuyu daha net bir şekilde anlamaya çalışacağız. Örneği geliştirmeden önce özellikle servis ve metod seviyesinde bilinmesi gerekenler olduğunu belirtelim. Özellikle servis nesnesi için çalışma zamanı davranışlarını belirlemek adına ServiceBehavior niteliğinin bazı özelliklerini kullanmak gerekmektedir. Benzer şekilde operasyonların transaction ile ilişkili çalışma zamanı davranışlarını belirlemek içinde, OperationBehavior niteliğinin özelliklerinden faydalanılmaktadır.

ServiceBehavior ve OperationBehavior dışında transacation yönetimi için kullanılan bir diğer nitelik (attribute) ise, TransactionFlow'dur. Bu nitelik servis tarafındaki metodlara uygulanabilen türdedir. TransactionFlow niteliği temel olarak bir servisin dış servisler ile olan ilişkilerinde servisler arasında akan transaction'ların operasyon kademesinde kabul edilme seviyelerinin belirlenmesinde kullanılır. Söz konusu seviyeler TransactionFlowOption isimli enum sabiti tarafından belirlenebilir. TransactionFlowOption enum sabitinin değerleri ve nasıl bir akışa izin verdikleri aşağıdaki tabloda gösterilmektedir.

TransactionFlowOption
Değeri
Açıklaması

NotAllowed
WCF çalışma zamanı (runtime) servise akan transactionları geçersiz kılıp her zaman için yeni bir tane oluşturur.

Allowed
Transaction istemci tarafında (istemci güncel servisin konuştuğu başka bir serviste olabilir) açılır. Eğer açılmamışsa WCF çalışma zamanı (runtime) yeni bir tane oluşturur.

Mandatory
Söz konusu operasyonun yürütülebilmesi için istemci uygulma (yada diğer servis) transaction açmak zorundadır. Söz konusu transaction'a ait bilgi SOAP Header (SOAP zarfının başlık kısmında) içerisinde de taşınmaktadır.

Bir servisin başka servis veya istemcilerden gelecek transaction akışına izin verip vermemesi durumlarını kontrol etmek için ilgili bağlayıcı tiplerin (binding types) transactionFlow elementi kullanılır. Varsayılan olarak transaction flow seçeneği aktif değildir (disabled). Dolayısıyla transaction akışının kontrol altına alınmasının istendiği durumlarda bilinçli olarak açılması gerekmektedir. Bunun için transactionFlow niteliğine true değerinin atanması yeterlidir.

ServiceBehavior niteliğininde transaction yönetiminde kullanılan önemli özellikleri vardır. Bunlar TransactionAutoCompleteOnSessionClose, RelaseServiceInstanceOnTransactionComplete, TransactionTimeOut ve TransactionIsolationLevel isimli özelliklerdir. TransactionAutoCompleteOnSessionClose özelliği adındanda anlaşılacağı üzere oturum kapatıldığında transaction'a ne olacağının belirlenmesinde kullanılır. Bunu oturum kapatıldığında transaction'ın tamamlanıp tamamlanmayacağının belirtilmesi olarakda düşünebiliriz. Nitekim söz konusu özelliğin değeri true olarak belirlendiğinde hatasız olarak tamamlanan bir oturum sonrasında ilgili transaction otomatik olarak kapatılır. Normal şartlar altında herhangibir problem nedeni ile ortama bir istisna (exception) nesnesi fırlatıldığında otomatik olarak geri alma (rollback) işlemi gerçekleşir. Bu özelliğin true olarak belirlenmesi bu işlemin daha kontrollü bir şekilde yapılmasınıda sağlar. Ne varki varsayılan değeri false'dur.

RelaseServiceInstanceOnTransactionComplete özelliği, transaction tamamlandığında ilgili servis nesne örneğinin serbest (relase) bırakılıp bırakılmayacağının belirlenmesinde kullanılır. Varsayılan değeri true olarak belirlenmiştir.

> Eğer OperationBehavior niteliğinin TransactionScopeRequired değeri true ve ServiceBehavior niteliğinin ConcurrencyMode özelliğinin değeri Reentrant iseRelaseServiceInstanceOnTransactionComplete özelliğinin değeri false yapılmalıdır. Aksi takdirde çalışma zamanında bir istisna (exception) alınır. Bunun sebebi RelaseServiceInstanceOnTransactionComplete özelliğinin varsayılan değerinin true olmasıdır.

Burada RelaseServiceInstanceOnTransactionComplete özelliğine göre transaction'ları tamamlamanın öncelikli olan 4 farklı yolu vardır.

- OperationBehavior niteliğinde TransactionAutoComplete özelliğine true değeri verilir.
- Servis tarafında SetTransactionComplete metodu çağırılır. Bu özellikle TransactionAutoComplete özelliğinin false olduğu durumlarda kullanılır. Genellikle geliştirme safhasında operasyonun transaction'ı tamamlama ihtiyacının tam olarak kararlaştırılamadığı durumlarda tercih edilebilir.
- İstemcinin (yada diğer bir servisin) dahil olduğu aktif bir transaction içerisindeyken oturumu kapatması veya network hatası alınması halinde eğer TransactionAutoComplete özelliğinin değeri false ise otomatik olarak geri alma (rollback) işlemi gerçekleşir.
- Yukarıdaki durumların dışında oluşabilecek herhangibir neden transaction'ın iptal edilmesi (abort) ve o ana kadar yapılmış işlemler var ise bunların geri alınması (Rollback) anlamına gelmektedir.

ServiceBehavior niteliğinde transaction yönetimi için kullanılan bir diğer özellikte TransactionIsolationLevel'dır. Bu özelliğin değeri, servis içerisinde yeni bir transaction açıldığında veya servise başka bir istemciden (veya bir servisten) bir transaction aktığında (Flow), değişken veriler (volatile datas) için söz konusu olacak izolasyon seviyesinin (Isolation Level) ne olacağını belirler.

> Bir transaction tarafından etkilenen veri, değişken veri (volatile data) olarak adlandırılır.

Izolasyon seviyeleri sayesinde ortak veriler üzerinde diğer transaction'ların yaptığı değişikliklerin nasıl ele alınacağı yada transaction'ın değişikliğe uğrayan veriyi ne kadar süre kilitleyeceği gibi özellikler belirlenebilir. Söz konusu özelliğin değeri IsolationLevel enum sabiti türünden olabilir. Temel olarak ele alınabilecek 5 farklı izolasyon seviyesi vardır (Chaos ve Unspecified haricinde). Bunlar aşağıdaki tabloda kısaca özetlenmektedir.

Izolasyon Seviyesi
Değeri
Açıklaması

ReadUncommited
Transaction boyunca değişken veri (volatile data) üzerinde değişiklik ve okuma yapılabilir. Bu en düşük izolasyon seviyesi olarak nitelendirilir. Bu nedenle Phantoms, Non-Repeatable Read ve Dirty-Read durumları oluşabilir.

ReadCommited
Transaction boyunca değişken veri üzerinde değişiklik yapılabilir ama okuma yapılamaz. Non Repetable Read ve Phantoms durumları oluşabilir. Ancak Dirty-Read durumları oluşmaz.

Repeatable Read
Transaction boyunca değişken veri üzerinde değişiklik yapılamaz ama okuma yapılabilir. Bunun dışında Transaction boyunca yeni veri eklenebilir. Phantoms durumu oluşabilir ancak Non-Repeatable Read ve Dirty-Read halleri oluşmaz.

Serializable
En yüksek mertebeli izolasyon seviyesi olarak ele alınır. Buna göre Transaction boyunca değişken veri okunabilir ama değiştirme veya ekleme yapılamaz. Bu seviye Phantoms, Non-Repeatable Read ve Dirty-Read gibi durumların oluşmasına izin vemez.

Snapshot
Bu seviye aslında ReadCommited'ın farklı bir formasyonudur ve SQL Server 2005 tarafından desteklenmektedir. Transaction boyunca değişken veri okunabilir. Burada veri değiştirilmeden önce, son okumadan bu yana başka bir transaction'ın aynı veriyi değiştirip değiştirmediğine bakılır. Eğer böyle bir durum var ise hata fırlatılır. Bu bir anlamda transaction'ın son onaylanan veriyi okumasını sağlayan bir yoldur. Bu seviye Phantoms, Non-Repeatable Read ve Dirty-Read gibi durumların oluşmasına izin vermez. (Snapshot izolasyon seviyesini kullanabilmek için SQL Server üzerinde Snapshot desteğinin ilgili veritabanı için açılması gerekmetekdir.)

ServiceBehavior tarafında yer alan bir diğer özellik ise TransactionTimeOut'dur. Bu özellik tahmin edileceği üzere transaction için zaman aşımı süresini bildirir. Eğer belirtilen sürede transaction tamamlanmamışsa iptal (abort) süreci başlatılır.

Gelelim OperationBehavior niteliğinin transaction için kullanılan özelliklerine. Söz konusu özellikler, TransactionAutoComplete ve TransactionScopeRequired'dır. TransactionAutoComplete özelliğinin varsayılan değeri true'dur ve buna göre eğer ele alınamayacak (unhandled) bir hata oluşmamışsa söz konusu operasyonun yer aldığı Transaction otomatik olarak tamamlanır (Commit). Ele alınamayan istisnalar otomatik olarak Transaction'ın iptal (abort) edilmesini tetiklemektedir.

> Transaction'ın kod içerisinden tamamlanması veya iptal edilmesi istendiğindeTransactionAutoComplete özelliğinin varsayılan olarak true olan değerinin bilinçli bir şekilde false olarak belirtilmesi gerekmektedir.

TransactionScopeRequired özelliği, uygulandığı metodun çalışması sırasında bir transaction scope gerektirip gerektirmediğinin belirlenmesinde kullanılır. Söz konusu özelliğin varsayılan değeri false'dur. Eğer metodun çalışması sırasında bir Transaction Scope gerekiyorsa true olarak belirlenir. Eğer başka bir yerden metoda akan bir transaction var ise, metod içeriği gelen transaction'a dahil olacaktır. Tam aksine kullanılabilir bir transaction yoksa yeni bir tane oluşturulur ve yürütme yeni açılan bu Transaction içerisinde gerçekleştirilir.

Aslında TransactionFlow ve TransactionScopeRequired özellikleri göz önüne alındığında bunların sahip olabileceği değerlerin etkisini iyi bir şekilde kavramak gerekir. Bu amaçla örnek bir senaryo olarak aşağıdaki şekli göz önüne alalım.

![mk210_1.gif](/assets/images/2007/mk210_1.gif)

İlk olarak istemci uygulama Servis A üzerinden bir operasyon çağrısında bulunur. Şekle göre Servis A bağlayıcı tipini (Binding Type) istemcilerden gelecek transaction akışlarını destekleyecek biçimde tasarlamıştır. Üstelik çağırılan operasyon için Servis A, Transaction akışına izin vermektedir. Bununla birlikte istemcide açılan bir Transaction mevcut değildir. Lakin Servis A için TransactionScopeRequired özelliğinin değeri true olarak belirlenmiştir. Buna göre söz konusu operasyon için Servis A otomatik olarak bir Transaction alanı (Scope) açacaktır.

Gelelim Servis B'ye. Servis A, Servis B üzerinden de bir operasyon çağrısı yapmaktadır. Servis B'ye ait bağlayıcı tip (binding type) diğer istemcilerden transaction akışına izin vermektedir. Bununla birlikte Servis B'deki operasyonun TransactionFlow özelliği Mandatory olarak belirlenmiştir. Bir başka deyişle Servis A'nın Servis B'ye bir Transaction akışı sağlaması şarttır. Diğer taraftan Servis B'deki TransactionScopeRequired özelliğinin değeri true olduğundan Servis B'deki operasyon, otomatik olarak Servis A'dan gelen Transaction Scope'a dahil olacaktır. Buna göre Servis A ve Servis B aynı Transaction Scope içerisinde çalışacaktır.

Servis B, Servis C üzerinden bir operasyon çağırmaktadır. Servis C'deki duruma baktığımızda, bağlayıcı tipin başka servis veya istemcileriden gelen transaction akışına izin vermediği görülmektedir. Bununla birlikte operasyon içinde, transaction akışına izin verilmemektedir. Ancak çağırılan operasyonun bir transaction içerisinde çalıştırılması gereklidir. Çünkü TransactionScopeRequired özelliğinin değeri true olarak ayarlanmıştır. Bu sebepten Servis C kendine ait Transaction alanı içerisinde çalışmaktadır.

Servis C, Servis D üzerinden de bir operasyon çağırmaktadır. Servis C'dekine benzer olarak diğer servis veya istemcilerden gelecek transaction akışına izin vermeyecek şekilde bağlayıcı tipi ayarlanmıştır. Diğer yandan çağırılan operasyon içinde transaction akışına izin verilmemekte ve yeni bir transaction oluşturulması da engellenmektedir. Dolayısıyla buradaki operasyon tamamen transaction haricinde kendi haline çalışmaktadır.

Buradaki bilgiler ışığında pek çok senaryo düşünülebilir. Kullanılan özellikler göz önünde bulundurulduğunda çok doğal olarak olasılıkların sayısı artmakta ve ele alınmaları zorlaşmaktadır. Bu sebepten özelliklerin değerlerinin farklı kombinasyonlarının etkilerini daha kolay bir şekilde görebilmek için, aşağıdaki tablodan yararlanılabilir.

TransactionScopeRequired
Değeri
TransactionFlowOption
Değeri
transactionFlow
Elementi Değeri
Transaction'ın
Ele Alındığı Taraf

true
Allowed
false
Servis Taraflı

true
NotAllowed
false

true
Mandatory
true
İstemci Taraflı

true
Allowed
true
Çift Tarafllı

false
Allowed
false
Tarafsız

false
NotAllowed
false

false
Allowed
true

false
Mandatory
true

Tabloda TransactionScopeRequired, TransactionFlowOption özelliklerinin ve transactionFlow elementinin değerlerine göre transaction'ın hangi tarafta ele alınabileceği görülmektedir. Servis taraflı modele göre, servis uygulaması her zaman için kendine ait bir transaction'a sahiptir. Ayrıca servis tarafı bu transaction'ı her zaman için istemci transaction'ından ayrı olacak şekilde ele alır. Bir başka deyişle servis tarafının her zaman için kendi transaction'ında çalıştığı düşünülebilir. Senaryoda yer alan Servis C bu durumu tam olarak karşılamaktadır.

İstemci taraflı modele göre, servis kendisine istemci tarafından akan transaction'ı kullanır. Örnek senaryoda yer alan Servis C bu modele uygun bir şekilde çalışmaktadır. Bu modelde servisin istemciden akan bir transaction'ı alması zorunludur. Daha çok, deadlock'ların oluşmasının engellenmesi ve sistem tutarlılığının sağlanması istenilen durumlarda tercih edilebilir. Nitekim istemci tarafında açılan transaction, servis tarafına da dahil edildiğinden tüm işlemler aslında aynı transaction scope içerisinde yer alacaktır. Buda doğal olarak deadlock oluşma ihtimalini azaltmaktadır. Bunun dışında istemci ve servislerin sayısının arttığı bir modelde herkesin aynı transaction'a dahil olması, atomicity ilkesinin en iyi biçimde karşılanması anlamınada gelmektedir.

Çift taraflı modelde servis istemciden kendisine doğru akan transaction'ı kullanabilir yada akan bir transaction yoksa kendi oluşturduğu transaction scope'u kullanır. Bu yararlı bir modeldir. Eğer istemci tarafından akan bir transaction var ise tüm sürecin atomikliği daha tutarlı bir şekilde sağlanabilir. Yinede istemci tarafından gelen bir transaction yok ise bu durumda servisin bir kök transaction (root transaction) açarak tüm süreci üstüne almasıda söz konusu olabilir. Bu açılardan bakıldığında çok fazla tercih edilebilecek bir modeldir.

Son olarak tarafsız modele göre, servis hiç bir durumda transaction'a sahip olmaz. Örnek senaryoda yer alan Servis D buna örnek olarak verilebilir. Tarafsız model daha çok istemci tarafındaki transaction'ların, servis tarafından bozulması istenmeyen durumlarda ele alınabilir.

Şimdi basit bir örnek ile transaction yönetimini incelemeye çalışalım. Şu ana kadar anlatılanlar göz önüne alındığında transaction kullanımının değişiklik varyasyonları olabileceği düşünülebilir. Örnekte ilk olarak istemci tarafında oluşturulan bir transaction'ın servis tarafına aktarılması ve burada ele alınarak commit işleminin gerçekleştirilmesi incelenecektir. Her zaman olduğu gibi işe servisin sunacağı arayüz sözleşmesi ve uzak nesne sınıfını tasarlayarak başlamak gerekir.

![mk210_2.gif](/assets/images/2007/mk210_2.gif)

ISiparisYonetici arayüzü;

```csharp
using System;
using System.Transactions;
using System.ServiceModel;

namespace NorthwindYonetici
{
    // IsolationLevel enum sabitinin kullanılabilmesi için System.Transactions.dll' inin projeye referans edilmesi gerekmektedir.
    [ServiceContract(Name="Siparis_Yonetim_Servisi", Namespace="http://www.bsenyurt.com/2007/23/6/SiparisYonetimServisi", SessionMode=SessionMode.Required)]
    public interface ISiparisYonetici
    {
        [OperationContract(Name="SepeteEkle",IsInitiating=true)]
        bool SepeteUrunEkle(int urunNumarasi);
    
        [OperationContract(Name="SepettenCikar",IsInitiating=false)]
        bool SepettenUrunCikart(int urunNumarais);

        [OperationContract(Name="SepetiOnayla",IsInitiating=false,IsTerminating=true)]
        bool Onayla();
    }
}
```

SiparisYonetici sınıfı;

```csharp
using System;
using System.Data.SqlClient;
using System.Transactions;
using System.ServiceModel;
using System.Text;

namespace NorthwindYonetici
{
    [ServiceBehavior(TransactionIsolationLevel= IsolationLevel.Serializable, TransactionTimeout="00:02:00", InstanceContextMode=InstanceContextMode.PerSession)]
    public class SiparisYonetici:ISiparisYonetici
    {
        private string TransactionInfo(Transaction trx,string mesaj)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(mesaj);
            builder.AppendLine("Güncel Transaction Bilgileri");
            builder.Append("Oluşturulma zamanı (Creation Time): ");
            builder.AppendLine(trx.TransactionInformation.CreationTime.ToLongTimeString());
            builder.Append("Dağıtık GUID (Distributed GUID): ");
            builder.AppendLine(trx.TransactionInformation.DistributedIdentifier.ToString());
            builder.Append("Yered belirleyici (Local Identifier) : ");
            builder.AppendLine(trx.TransactionInformation.LocalIdentifier);
            builder.Append("Durum (Status) : ");
            builder.AppendLine(trx.TransactionInformation.Status.ToString());
            builder.AppendLine();
            return builder.ToString();
        }

        #region ISiparisYonetici Members
    
        [OperationBehavior(TransactionAutoComplete=false,TransactionScopeRequired=true)]
        [TransactionFlow( TransactionFlowOption.Mandatory)]
        public bool SepeteUrunEkle(int urunNumarasi)
        {
            // İstemci tarafında bu metoda yapılan her çağrı sonrasında o anki transaction için bir TransactionCompleted olayı yüklenir. Bu nedenle kaç metod çağrısı var ise hepsi için TransactionCompleted olayı birer kez çalışır.
            Transaction.Current.TransactionCompleted += new TransactionCompletedEventHandler(Current_TransactionCompleted);
            Console.WriteLine(TransactionInfo(Transaction.Current,"SepeteUrunEkle metodu çağırıldı"));
            return true;
        }

        void Current_TransactionCompleted(object sender, TransactionEventArgs e)
        {
            Console.WriteLine(TransactionInfo(e.Transaction,"TransactionCompleted Olayı tetiklendi"));
        } 

        [OperationBehavior(TransactionAutoComplete = false, TransactionScopeRequired = true)]
        [TransactionFlow(TransactionFlowOption.Mandatory)]
        public bool SepettenUrunCikart(int urunNumarais)
        {
            Console.WriteLine(TransactionInfo(Transaction.Current, "SepetenUrunCikart metodu çağırıldı"));
            return true;
        }

        [OperationBehavior(TransactionAutoComplete = true, TransactionScopeRequired = true)]
        [TransactionFlow(TransactionFlowOption.Mandatory)]
        public bool Onayla()
        {
            Console.WriteLine(TransactionInfo(Transaction.Current,"Onayla metodu çağırıldı"));
            /* Yorum satırı haline getirildiğinde TransactionAutoComplete false ise söz konusu transaction abort edilir. Ancak TransactionScopeAutoComplete true ise aşağıdaki metodu çağırmaya gerek kalmadan transaction onaylanır. Ayrıca TransactionAutoComplete özelliği true ise, SetTransactionComplete metodunu çağırmak çalışma zamanı hatasına neden olur. Bu aktif olan bir transaction var ise bununda iptal edilmesine neden olacaktır. */
            //OperationContext.Current.SetTransactionComplete(); // Bu çağrıdan sonra Transaction.Current null değer döndürecektir.
            return true;
        }

        #endregion
    }
}
```

Söz konusu servis sınıfı içerisindeki metodlarda herhangibir bağlantı ifadesi yer almamaktadır. Sadece bir alış veriş sepetinin hayali bir uyarlaması bulunmaktadır. Ancak önemli olan bazı noktalar vardır. İstemci operasyon çağrılarını gerçekleştirirken ilk olarak SepeteUrunEkle metodunu çağırmalıdır. Operasyonların tamamlanması ise Onayla metodunun çağırılması ile gerçekleşir. Bu metod sırasını sağlamak için arayüze (interface) ait OperationContract niteliği ve IsInitiating ile IsTerminating özellikleri kullanılmıştır.

SiparisYonetici sınıfındaki metodlarda istemci tarafında oluşturulması zorunlu olan bir transaction akışının gelmesini sağlamak için TransactionFlow niteliklerinde Mandatory değeri kullanılmıştır. Sınıf içerisinde oluşturulan güncel transaction'ları daha kolay takip edeilmek için TransactionInfo isimli bir metod kullanılmaktadır. Çalışma zamanında aktif olan bir transaction'ı elde etmek için Transaction sınıfının Current özelliği ele alınabilir.

> Servis veya istemci uygulamalarda, kod tarafında Transaction, TransactionScope gibi tipleri kullanabilmek için projeye System.Transactions.dll'inin referans edilmesi gerekmektedir.

Current özelliği ile çalışma zamanında elde edilen Transaction nesne örneği üzerinden TransactionInfo özelliğine geçilerek güncel transaction hakkında bazı bilgiler toplanabilir. Örneğin oluşturulma zamanı (Creation Time), dağıtık transaction açılmışsa bunun GUID numarası (Distributed Transaction Identifier) ve hatta transaction'ın o anki durumu (status) gibi. Bu tanımlamalara ek olarak servise ait transaction'ların izolasyon seviyesi Serializable, transaction için geçerli zaman aşımı (timeout) süresi ise 2 dakika olarak belirlenmiştir.

Servis uygulamasında kullanılacak olan konfigurasyon dosyasının içeriği aşağıdaki gibi tasarlanmalıdır.

Servis tarafındaki App.config dosyası;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <netTcpBinding>
                <binding name="SiparisServisiBindingConfiguration" transactionFlow="true" transactionProtocol="OleTransactions">
                </binding>
            </netTcpBinding>
        </bindings>
        <services>
            <service name="NorthwindYonetici.SiparisYonetici">
                <endpoint address="net.tcp://localhost:4500/SiparisServisi" binding="netTcpBinding" bindingConfiguration="SiparisServisiBindingConfiguration" name="SiparisServisiEndPoint" contract="NorthwindYonetici.ISiparisYonetici" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Dikkat edilmesi gereken noktalardan birisi, netTcpBinding bağlayıcısı için gerekli konfigurasyon ayarlarında transactionFlow niteliğinin true, transactionProtocol niteliğinin ise OleTransactions olarak set edilmiş olmasıdır. Servis tarafı, örneğin daha kolay bir şekilde incelenmesi için Console uygulaması olarak tasarlanmıştır ve kodları aşağıdaki gibidir.

```csharp
ServiceHost host = new ServiceHost(typeof(NorthwindYonetici.SiparisYonetici));
host.Open();
Console.WriteLine("Sunucu dinlemede");
Console.WriteLine("Servisi durdurmak için bir tuşa basın");
Console.ReadLine();
host.Close();
```

Bu adımlardan sonra istemci tarafı için gerekli proxy nesnesinin oluşturulması gerekir. Bunun için yine svcutil aracından yararlanılabilir. Dikkat edilmesi gereken noktalardan birisi şudur; servis tarafında TransactionFlow niteliklerinde bir değişiklik yapıldığında bunun proxy sınıfı içinde gerçekleştirilmesi bir başka deyişle istemci tarafında güncelleştirilmesi gerekir. Dolayısıyla proxy sınıfının yeniden üretilmesi gerekecektir. Yada manuel olarak istemci tarafındaki proxy sınıfına müdahale edilip bu nitelikler açık bir şekilde değiştirilmelidir.

Söz gelimi SiparisYonetici sınıfındaki TransactionFlow niteliklerini Allowed olarak belirleyip, istemciler için gerekli proxy sınıfını ürettiğimizi düşünelim. Daha sonradan TransactionFlow niteliğinde Mandatory seçeneğini kullanır ve proxy sınıfını güncellenmez ise, WCF çalışma zamanında ProtocolException tipinden bir istisna (exception) üretilecektir.

İstemci tarafında yer alacak konfigurasyon dosyasının içeriğinin aşağıdaki gibi olması gerekmektedir.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <system.serviceModel>
        <bindings>
            <netTcpBinding>
                <binding name="SiparisYonetimServisiBinding" transactionFlow="true" transactionProtocol="OleTransactions">
                </binding>
            </netTcpBinding>
        </bindings>
        <client>
            <endpoint address="net.tcp://localhost:4500/SiparisServisi" binding="netTcpBinding" bindingConfiguration="SiparisYonetimServisiBinding" contract="Siparis_Yonetim_Servisi" name="Siparis_Yonetim_Servisi" />
        </client>
    </system.serviceModel>
</configuration>
```

İstemci uygulamayıda bir Console olarak tasarladığımızı düşünecek olursak, Main metodu içerisinde aşağıdaki kodları yazmamız gerekecektir.

```csharp
Console.WriteLine("Devam etmek için bir tuşa basınız");
Console.ReadLine();
Siparis_Yonetim_ServisiClient client = new Siparis_Yonetim_ServisiClient("Siparis_Yonetim_Servisi");
using (TransactionScope tx =new TransactionScope(TransactionScopeOption.RequiresNew))
{
    client.SepeteEkle(11);
    client.SepeteEkle(42);

    client.SepettenCikar(42);

    if(client.SepetiOnayla())
        tx.Complete(); 
}
Console.ReadLine();
```

İstemci uygulamada TransactionScope tipine ait bir nesne örneği kullanılarak yeni bir transaction alanı açılması sağlanmaktadır. Bundan sonrasında ise istemci tarafından servis üzerindeki bir kaç metod arka arkaya çağırılır. Son olarak using bloğu terk edilmeden önce eğer SepetiOnayla metodunun dönüş değeri true ise tx isimli TransactionScope sınıfı nesne örneğine ait Complete metodu çağırılır. Önce sunucu ve sonrasında ise istemciyi çalıştırdığımızda servis tarafındaki console uygulamasında aşağıdaki ekran görüntüsü elde edilir.

![mk210_3.gif](/assets/images/2007/mk210_3.gif)

Burada dikkat edilmesi gereken ilk nokta, SepeteUrunEkle metodunun ilk çağırılışıyla beraber elde edilen dağıtık transaction GUID numarasının tüm metod çağrılarında aynı olmasıdır. Bu basit olarak, her operasyonun açılan transaction scope'a dahil edildiği anlamına da gelmektedir. SepeteUrunEkle metodu içerisinde Transction için TransactionCompleted olayı yüklenmiştir. İstemci tarafında SepeteUrunEkle metodu iki kez çağırılmıştır. Bu nedenle iki adet TransactionCompleted olay metodu yüklenmiş ve güncel transaction'a ait Commited durumu iki kez yakalanmıştır. Olayı daha iyi kavrayabilmek için servis tarafındaki SiparisYonetici sınıfı içerisindeki SepeteUrunEkle metodu içerisinde aşağıdaki ekran görüntüsünde olduğu gibi breakpoint konulması yerinde bir harekete olacaktır.

![mk210_4.gif](/assets/images/2007/mk210_4.gif)

İlk metod çağrısı ile birlikte Compenent Services içerisindeki Distributed Transaction Coordinator kısmına bakıldığında Transaction List içerisinde yeni bir OleTx transaction açıldığı ve Transaction Statistics kısmındada bu transaction'ın aktif olarak yer aldığı görülmektedir.

![mk210_5.gif](/assets/images/2007/mk210_5.gif)

Aynı zamanda Transaction Statistics kısmına bakılırsa aşağıdakine benzer bir ekran görüntüsü ile karşılaşılır. Dikkat edilecek olursa aktif olan 1 adet transaction bulunmaktadır.

![mk210_6.gif](/assets/images/2007/mk210_6.gif)

Eğer işlemler sonuna kadar devam ettirilirse, transaction'ın başarılı bir şekilde tamamlandığı yine istatistik kısmından görülebilir. Burada dikkat edilmesi gereken değer Commited'dır.

![mk210_7.gif](/assets/images/2007/mk210_7.gif)

Şimdi Onayla metodundan geriye false değer döndürdüğümüzü düşünelim. Bu durumda istemci tarafındaki TransactionScope sınıfına ait nesne örneği üzerinden Complete metodu tetiklenemiyecektir. Dolayısıyla söz konusu transaction'ın iptal edilmiş olması gerekir. Gerçektende uygulama çalıştırıldığında aşağıdaki ekran görüntüsünde olduğu gibi TransactionCompleted isimli olay metodu tetiklendiğinde, o anki transaction'ın durumunun Aborted olduğu gözlemlenebilir.

![mk210_8.gif](/assets/images/2007/mk210_8.gif)

Benzer şekilde DTC altındaki Transaction Statistics bölümüne bakıldığında bir transaction'ın iptal edildiği bilgisi yer alacaktır. (Bu aşamada olayın daha iyi analiz edilmesi için Services kısmından Distributed Transaction Coordinator servisi Restart edilmiştir. Bu sebepten aşağıdaki ekran görüntüsünde olduğu gibi önceki örnekle birlikte toplam 2 transaction olması gerekirken sadece 1 transaction yer almaktadır.)

![mk210_9.gif](/assets/images/2007/mk210_9.gif)

Dikkat etmemiz gereken noktalardan biriside transactionFlow elementinin değeridir. Eğer istemci tarafındaki konfigurasyon dosyasında bu özelliğin değeri false olarak bildirilirse, Mandatory seçeneği nedeni ile WCF çalışma zamanında InvalidOperationException istisnası alınır. Benzer durum servis tarafı içinde geçerlidir. Eğer istemcide özelliğin değeri true ama servis tarafında false ise yine WCF çalışma zamanı servis tarafına bir InvalidOperationException istisnası fırlatacaktır.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde WCF için transaction yönetimini biraz daha derinlemesine incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın.](/assets/files/2007/TransactionKullanimi.zip)