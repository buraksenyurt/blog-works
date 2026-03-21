---
layout: post
title: "WCF - MSMQ(MicroSoft Message Queue) ile Entegrasyon"
date: 2007-11-13 12:00:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
  - msmq
---
TCP veya HTTP bazlı iletişimlerde, tarafların aynı zaman dilimi içerisinde çalışıyor olmaları gerekmektedir. Böyle bir mesajlaşma sürecinde taraflardan herhangibirin çalışmaması, aradaki bağlantının kopması gibi nedenlerden dolayı tüm iletişimin aksamasıda muhtemeldir. Bazı gerçek hayat senaryolarında, sistemin tarafları olan istemci (Client), sunucu (Server), ağ (Network) bileşenlerinin çökmesi durumlarında dahi işlevselliğin devam edebilmesi istenebilir. Bunun dışında, çalışan sistemin içerisindeki bileşenlerin sürekli bir bağlantıda olmadığı durumlarda bu tip iletişimleri zorlaştırmaktadır. Bir başka deyişle ağa sürekli olarak bağlanamayan ama offline olarak çalışabilen istemcilerin bu tip bir mesajlaşma sisteminin parçası olması istendiğinde senkronizasyon güçlükleri ortaya çıkmaktadır.

Ne varki mesajlaşmaların kuyruk (Queue) modeline göre taşındığı bir sistemde yukarıda bahsedilen iletişim sorunlarının yaşanmaması sağlanabilir. Nitekim mesaj kuyruğu (Message Queue) sisteminin geliştirilme amacıda aslında budur. Bu sistem temelde Windows tabanlı bilgisayarlar arasında kuyruk sistemine dayalı bir mesaj alışverişinin tesis edilmesini sağlamaktadır. Bu iletişimde istemci (Client), servis (Service) ve ağ (Network) arasında bir izolasyon sağlanmaktadır. Bu izolasyon sayesinde istemci, servis yada ağ çökse, hata üretse dahi fonksiyonelliklerini devam ettirebileceklerdir. Kuyruk temelli iletişimde güvenilir (Reliable) bir iletişim ortamı tesis edilmekte olup transaction kullanılabilmekte, kuyruğa atılan mesajlar fiziki yada değişken (Volatile) olarak saklanabilmektedir. Kuyruk temelli mesajlaşmanın en bilinen uyarlaması MicroSoft Message Queue bileşenidir (Component).

MicroSoft Message Queue (MSMQ), Windows NT sürümünden beri gelen bir bileşendir. NT işletim sistemi için 1.0 versiyonu duyrulan MSMQ bileşeninin çok kısa tarihçesi aşağıdaki tabloda olduğu gibidir.

MSMQ
Versiyonu
Desteklenen
İşletim Sistemi (Sistemleri)
Özellikler

1.0
Windows NT
Varsayılan özellikler...

2.0
Windows 2000

Public mesaj kuyruklarının Active Directory destekli olarak saklanması
128 bitlik şifreleme desteği
Dijital imza desteği
Tam COM desteği

3.0
Windows XP
Windows Server 2003

HTTP, SOAP desteği ile internet üzerinden mesajlaşabilme
IIS için MSMQ desteği
Mesajların Çoklu Dönüştürülebilmesi (MultiCasting)

4.0
Vista
Longhorn Server

Alt kuyruk desteği (Subqueue)
Zehirli mesajların ele alınması (Posion Message Handling)
Uzak kuyruklardan transaction bazlı mesaj alma desteği (Transactional Remote Receive)

* [http://www.microsoft.com/windowsserver2003/technologies/msmq/whatsnew.mspx](http://www.microsoft.com/windowsserver2003/technologies/msmq/whatsnew.mspx)
* * [http://windowssdk.msdn.microsoft.com/en-us/library/ms701784.aspx](http://windowssdk.msdn.microsoft.com/en-us/library/ms701784.aspx)

Ayrıca 1999 yılında MSMQ'nun mobil cihazlar için olan desteğide Windows CE 3.0 işletim sistemi ile birlikte başlamıştır.

MSMQ Windows tabanlı bir bileşendir. Ancak farklı bilgisayarlar ilede haberleşme olanağı mevcuttur. MSMQ'nun görevi aynen diğer iletişim sistemlerinde olduğu gibi farklı bilgisayarlar arasında mesajlaşmayı sağlayabilmektir. Bu mesajlaşmadaki tek fark, tarafların aynı anda çalışıyor olma zorunluluklarının bulunmayışıdır. Hatta söz konusu taraflarda çalışan uygulamalar bir ağa (Network) bağlı olmak zorundada değildirler. Böyle bir durumda ortaya çıkan soru iletilmek istenen mesajların karşı taraf kapalı iken nasıl ulaştırılacağıdır? MSMQ bileşenleri, mesajları, karşı taraf hazır oluncaya kadar bir depoda saklamaktadır. Bu deponun tutuluş yeri varsayılan olarak fiziki bir ortamdır. Ancak istenirse performası arttırmak adına geçici bir ortamda da saklanabilirler. Ne varki Volatile olarak adlandırılan bu mesajlar sistemlerin çökmesi (MSMQ bileşeninin yüklü olduğu bilgisayarın örneğin yeniden başlatılması-restart) halinde kaybolacaktır. Oysaki fiziki saklanan mesajlar korunmaya devam edecektir. Buda MSMQ'nun mesajlar için uzun süreli bir saklama ortamı kullanabildiği anlamına gelmektedir (Durable).

MSMQ bileşenlerine.NET ortamı içerisinde System.Messaging isim alanında (Namespace) bulunan tipler yardımıyla erişilebilmektedir. Bu sebepten programatik olarak C#, VB.Net ve diğer.NET destekli diller yardımıyla kuyruklara bakılması, mesaj eklenmesi ve daha gelişmiş yönetsel işlemlerin yapılması mümkündür. Diğer taraftan MSMQ'nun C/C++ ile yazılmış olan ve COM tabanlı ortamlarda kullanılabilen bir API'side mevcuttur. Elbette WCF içerisinde MSMQ için hazır olan bazı bağlayıcı tipler (Binding Types) kullanılmaktadır.

MSMQ sonuç itibariyle taraflar arasında mesaj taşıdığından mesajların boyutlarıda performans açısından önemlidir. Varsayılan olarak minimum mesaj büyüklüğü 150 byte'tır. Bu mesaj içerisinde, imza (Signature), kaynak ve hedef bilgisayar ID'leri, hedef kuyruğun adı (Target Queue Name), mesaj özellikleri yer alır. Ancak transaction kullanılıyorsa yada doğrulama (authentication) ve şifreleme (encryption) söz konusuysa bu boyutun artması muhtemeldir. Örneğin dahili bir sertifika kullanımında mesaj boyutu 400 byte kadar artmaktadır. Harici sertifika (Certificate) kullanımında en az 1 kb'lık bir mesaj boyutu oluşmaktadır. Bunların dışında HTTP veya MultiCast kullanımı söz konusu ise SOAP formatlamadan dolayı mesajın sadece başlık kısmı (Message Header) 1 kb boyutunda olacaktır. Çok doğal olarak mesaj boyutlarındaki bu artışlar iletişim hızını olumsuz yönde etkileyebilir. Dolayısıyla en uygun senaryoları (Best Practices) ele almak gerekir. (Bölümün sonunda bu konuya kısaca değinilecektir.)

Peki MSMQ kullanmak için gerekli senaryolar neler olabilir? Aslında kuyruk tabanlı iletişim, güvenilir ağ ortamlarının söz konusu olmadığı, ağ ortamına (Network) arada sırada bağlanabilen uygulamaların haberleşmesi gerektiği durumlarda sıklıkla ele alınmaktadır. Örneğin bir servis uygulaması tarafından, gün içinde offline ortamlardan gelen siparişlerin gün sonunda toplu olarak işlendiği bir senaryo göz önüne alınabilir. Aşağıdaki şekilde buna benzer bir senaryo incelenmeye çalışılmaktadır.

![mk231_4.gif](/assets/images/2007/mk231_4.gif)

Senaryoda, zaman zaman offline duruma düşebilecek olan istemciler söz konusudur. Bu istemciler üzerinden gelen siparişler bir sunucuda toplanmaktadır. Siparişleri işleyen sunucu tahsilatlar için başka bir sunucuyu kullanmaktadır. Burada söz konusu siparişleri işleyen servis ile, tahsilatın yapıldığı sunucu ve PDA cihazında kullanılan uygulamaların sürekli bir bağlantıda olmadığı göz önüne alınmaktadır. Böyle bir durumda bir siparişin işlenmesi zaman alabilir. Ayrıca bu bekleme sürelerinin siparişleri işleyen uygulamayı etkilemesi istenmez. Yani, bekleyen siparişlerin gecikmeli olarakta olsa işlenebildiği ve yeni gelen siparişlerinde toplanabildiği bir ortamın hazırlanması gerekmektedir. Böylece PDA satıcılar siparişleri offline olarak toplayabilir ve daha sonra işlenmek üzere (servis ile bağlantı tekrardan sağlandıktan sonra) servise gönderebilir. Ayrıca web üzerinden gelen siparişlerin işleme alındığı bilgisi anında kullanıcılara gösterilebilir (Laptop ile ifade edilen kullanıcılar). Bu tip senaryolar genişletilebilir. Ancak uygun vakalar çoğunlukla offline çalışma ihtiyacı olan, güvenilir bir ağ ortamında bulunamayan veya bir ağa hiç bağlı olmayan bilgisayarların, asenkron olarak haberleşmesi gereken durumlardır.

> Burada çok önemli bir nokta vardır. MSMQ gibi kuyruk tabanlı iletişimi benimsiyen sistemler asenkron (asynchronous) olarak çalışmaktadır. MSMQ bu anlamda, güvenilir (Reliable) ortam oluşturabilen, Transaction desteği sağlayan, bağlantısız (Disconnected) çalışmaya olanak tanıyan, sürekli depolama (Durable) desteği veren mükemmel birasenkron (Asynchronous) iletişim ortamı sağlamaktadır.

MSMQ bileşenlerinin kullanıldığı uygulamalar herhangibir Windows programı olabilir. Bu nedenle bir Windows uygulamasından (Windows Application), Windows Servisinden (Windows Service), akıllı istemciden (Smart Client), web sitesinden MSMQ kullanılabilir. Bu noktada MSMQ ile ilgili önemli sorunlardan birisi gündeme gelmektedir. Örnek bir windows işletim sistemi tabanlı uygulama, MSMQ kullanarak, kuyruk tabanlı başka bir sistem ile nasıl haberleşebilir? Söz gelimi kuyruk tabanlı iletişimi kullanan IBM MQSeries ile bir MSMQ istemcisi nasıl haberleşebilir?

MSMQ istemcileri yabancı bilgisayarlar (Foreign Computers) ile Connector adı verilen uygulamalar aracılığıyla haberleşebilirler. Connector uygulamaları aslında bir MSMQ sunucusunda çalışır ve yabancı bilgisayarların MSMQ istemciler ile olan haberleşmelerinde aracılık yapar. Bu aracılıkta transaction desteği olan veya olmayan kanallar söz konusudur. IBM MQSeries ile iletişimi sağlayan Connector uygulama Microsoft MSMQ-MQSeries Bridge programıdır. Tabi farklı kuyruk tabanlı sistemler için farklı Connector uygulamaları mevcuttur.

> MSMQ'nun yazının hazrılandığı tarih itibariyle 4.0 versiyonu bulunmaktadır. Bu versiyon ile gelen bazı belirgin özelliklerin bilinmesi gerekmektedir. 4.0 versiyonu alt kuyruklar (SubQueue), zehirli mesajların ele alınması (Poison Message Handling), transaction bazlı uzak kuyruk desteği (Transactional Remote Receive) şeklinde 3 önemli yenilik içermektedir.
> SubQueues (Alt Kuyruklar): Mesajların ek fiziki kuyruklar oluşturmadan mantıksal olarak çeşitli kriterlere göre alt gruplara ayrılabilmesi desteğidir.
> Zehirli Mesajların Ele Alınmas (Poison Message Handling): Zehirli mesajlar, bir kuyrukta arkadan gelen mesajları engelleyen niteliktedir. Bir uygulama kuyruktan bir mesajı okuyup işlemek isterken, mesajdaki hata nedeni ile bu işlemi yapamadığı durumlarda, (eğer süreç bir transaction içerisindeyse) Abort işlemini gerçekleştirip mesajı tekrardan kuyruğa döndürecektir. Bu mesaj daha sonradan tekrardan işlenmek amacıyla uygulama tarafından yeniden okunacak ama hatası nedeni ile yine kuyruğa geri döndürülecektir. Bu durumda transaction'ın sürekli olarak abort edildiği ve mesajın kuyruğa gönderildiği sonsuz bir döngü oluşacaktır. Bu döngüye neden olan mesaj bir süre sonra tekrardan deneme sayısınında aşılması sebebiyle arkadan gelen mesajlarında işlenmesini engelleyecektir. İşte MSMQ 4.0 versiyonu ile birlikte bu tip mesajların daha güçlü bir şekilde ele alınması ve sorunun önüne geçilmesi sağlanmaktadır. Bunun içinde Retry Queue (yeniden deneme kuyruğu) adı verilen kuyruklar kullanılmaktadır. (WCF içerisindeki bağlayıcı tipler buna destek veren özellikler (Properties) içermektedirler)
> Transaction bazlı uzak kuyruk desteği (Transactional Remote Receive): Tek bir mesaj kuyruğunun işleri aldığı bir durumda, başka bilgisayarlardaki uygulamalarında bu işleri değerlendirdiği düşünülsün. Uygulamardan biri sıradaki bir işi işleyemez ise bu işin sonradan değerlendirilmek üzere tekrardan kuyruğa gönderilmesi tercih edilir. Bu tip bir süreç için transaction desteği gerekmektedir. Ayrıca istemci bilgisayarların ve kuyruğun ağ (Network) üzerinden DTC (Distributed Transaction Coordinator) erişiminede izin vermesi şarttır.

MSMQ, WCF içerisinde de doğrudan desteklenmektedir. Üstelik WCF'in sağladığı birleştirilmiş (Unified) model sayesinde, geliştiriciler MSMQ'nun karmaşık alt yapısından uzaklaşmaktadır. Basit anlamda WCF üzerinden MSMQ konseptine bakıldığında aşağıdaki durum söz konusudur.

![mk231_2.gif](/assets/images/2007/mk231_2.gif)

Burada istemci ve servis uygulamaları ortak bir kuyruğu kullanarak iletişim sağlamaktadır. Kuyruk içerisinde yer alan mesajlar fiziki bir ortamda sakalanbilmektedir. Microsoft Managament Console kullanıldığı takdirde kuyruktaki mesajların tutulacağı yer genellikle aşağıdaki şekildende görüldüğü gibi Windows\System32\msmq\Storage klasörüdür.

![mk231_1.gif](/assets/images/2007/mk231_1.gif)

Diğer taraftan gerçek bir dağıtık mimari vakasında mesaj kuyrukları çoğunlukla istemci ve servis uygulamaları için ayrı ayrı tutulurlar. Bu durum aşağıdaki şekilde olduğu gibi ifade edilebilir.

![mk231_3.gif](/assets/images/2007/mk231_3.gif)

Bu senaryoda istemci ve servis uygulamaları kendi ortamlarındaki kuyruk yöneticileri (Queue Manager) yardımıyla haberleşmektedir. Kuyruk yöneticileri mesajları kuyruğa atmak veya kuyruktan okumak için gerekli yönetsel işlemleri üstlenmektedir. İstemci uygulamalar, servis uygulamasına göndermek istedikleri mesajları kuyruk yöneticisine iletirler. Kuyruk yöneticisi bu mesajları taşıma kuyruğuna (Transport Queue) aktarır. Karşı taraf hazır olduğundada servis uygulamasındaki kuyruk yöneticisine gönderilir. Servis tarafındaki kuyruk yöneticisi gelen mesajı alarak hedef kuyruğa (Target Queue) atar. Sonrasında ise servis tarafındaki kuyruk yöneticisi, hedef kuyruğa kabul ettiği mesajı işlenmek üzere servis uygulamasına yönlendirir. Burada iletişim, güvenilir bir ortamda gerçekleştirilir.

WCF (Windows Communication Foundation) tarafında MSMQ desteği için iki farklı bağlayıcı tip (Binding Type) kullanılmaktadır. Bunlar netMsmqBinding ve msmqIntegrationBinding tipleridir. netMsmqBinding iki WCF End Point arasında MSMQ tabanllı bir iletişimi sağlamak amacıyla kullanılır. msmqIntegrationBinding tipi ise bir WCF End Point ve C, C++, COM veya System.Messaging API'si yardımıyla geliştirimiş bir MSMQ destekli uygulama arasında iletişimin sağlanması için kullanılmaktadır. Yanlız bu tip kullanılırken önemli bir kısıt vardır. Bu kısıta göre operasyon sözleşmesi (Operation Contract) tanımlanırken MsmqMessage tipinden tek parametre alan bir metod söz konusu olabilir. Bunun tek sebebi WCF dışında olan bir ortam ile ortak haberleşebilmeyi sağlamaktır.

> WCF tabanlı MSMQ (MicroSoft Message Queue) uygulamlarında servis tarafında tanımlanan operasyonların tek yönlü (OneWay) olacak şekilde tanımlanmaları gerekmektedir. Bunun sebepi MSMQ'nun normal şartlarda tek yönlü iletişimi (One-Way Transport) kullanıyor olmasıdır.

Artık bir örnek üzerinden hareket edilerek WCF içerisinde MSMQ'nun nasıl kullanılabileceğine bakılabilir. Öncelikli olarak servis sözleşmesi (Service Contract) ve uygulayıcı sınıfın yer aldığı WCF Servis Kütüphanesinin (WCF Service Libraray) örnek olarak aşağıdaki gibi tasarlandığı göz önüne alınsın.

![mk231_5.gif](/assets/images/2007/mk231_5.gif)

Servis Sözleşmesi;

```csharp
using System;
using System.ServiceModel;

namespace SiparisKutuphanesi
{
    [ServiceContract(Name="Siparis Servisi",Namespace="http://www.bsenyurt.com/SiparisServisi")]
    public interface ISiparisYonetici
    {
        [OperationContract(IsOneWay=true)]
        void SiparisEt(int urunNo, int miktar);
    }
}
```

Servis sözleşmesinde dikkat edilmesi gereken noktalardan birisi OperationContract isimli nitelikle (Attribute) kullanılan IsOneWay özelliğine true değeri atanmasıdır. Daha önceden de bahsedildiği gibi MSMQ tipinde haberleşmelerde mesajların tek yönlü çalışması söz konusudur. Bu doğal olarak mesajın gönderildikten sonra işlenip işlenmediğinden haberdar olunamaması anlamına da gelmektedir. Dolayısıyla mimari gereği operasyonun tek yönlü (One Way) olacağı belirtilmelidir.

Uygulayıcı Sınıf;

```csharp
using System;
using System.Threading;
using System.ServiceModel;

namespace SiparisKutuphanesi
{
    public class SiparisYonetici:ISiparisYonetici
    {
        #region ISiparisYonetici Members

        public void SiparisEt(int urunNo, int miktar)
        {
            Thread.Sleep(7000);
            // Sipariş ile ilgili işlemler
        }
        
        #endregion
    }
}
```

Uygulayıcı sınıf içerisinde sembolik olarak SiparisEt isimli bir metod bulunmaktadır. Bu metodun herhangibir işlevi yoktur. Sadece MSMQ senaryosunu tamamlayıcı bir öğedir. Diğer taraftan kuyruk oluşumlarını daha kolay izleyebilmek ve asenkron çalışmayı daha kolay takip edebilmek adına metod içerisinde bilinçli olaraktan, çalışan Thread'in 7 saniye süreyle uyutulması sağlanmaktadır.

Servis uygulaması basit bir Console uygulaması olarak tasarlanabilir. Nitekim burada önemli olan noktalar konfigurasyon dosyası içerisinde yapılan değişikliklerdir. Servis uygulamasına ait konfigurasyon dosyasının içeriği ve kaynak kodları aşağıdaki gibidir. (Servis uygulamasında, WCF Servis Kütüphanesi ve System.ServiceModel.dll assembly'larının referanslarının eklenmemesi unutulmamalıdır.)

Servis tarafı konfigurasyon dosyası;

```csharp
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <netMsmqBinding>
                <binding name="SiparisServisiBindingConfig" receiveTimeout="00:10:00" deadLetterQueue="System" durable="true" exactlyOnce="true" maxRetryCycles="2" receiveErrorHandling="Fault" receiveRetryCount="5" retryCycleDelay="00:30:00">
                    <security mode="None" />
                </binding>
            </netMsmqBinding>
        </bindings>
        <services>
            <service name="SiparisKutuphanesi.SiparisYonetici">
                <endpoint address="net.msmq://localhost/private/SiparisKuyrugu" binding="netMsmqBinding" bindingConfiguration="SiparisServisiBindingConfig" name="SiparisServisiEndPoint" contract="SiparisKutuphanesi.ISiparisYonetici" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Konfigurasyon dosyasının içeriği Microsoft Service Configuration Editor yardımıyla da oluşturulabilir. Bu yol izlenirse eğer, daha önceden değinilen TCP, HTTP, Named-Pipes, Peer To Peer gibi bağlantılardan farklı olarak bağlayıcı tip seçilirken aşağıdaki ekran görüntüsünde olduğu gibi MSMQ seçilmelidir.

![mk231_6.gif](/assets/images/2007/mk231_6.gif)

Yapılan bu seçimin ardından editor, istemcilerin MSMQ yada WCF tabanlı bir uygulama olup olmadığı sorulacaktır. Daha öncedende belirtildiği gibi, WCF End Point'leri arasında yada bir WCF End Point ile bir MSMQ uygulaması arasında mesaj kuyruğu tabanlı iletişim sağlanabilmektedir. Bu seçime göre kullanılacak olan bağlayıcı tip (Binding Type) belirlenmektedir. Örnekte netMsmqBinding ele alınmıştır. Bir başka deyişle WCF istemcileri ile MSMQ üzerinden konuşacak bir servis tasarlanmaktadır.

![mk231_7.gif](/assets/images/2007/mk231_7.gif)

Bu seçimin ardından adres bilgisinin girilmesi gerekir. MSMQ kullanıldığı için söz konusu adres aslında yerel makinedeki fiziki bir adresi işaret etmelidir. Burada örnek olarak aşağıdaki ekran görüntüsünde yer alan adres tanımı kullanılmaktadır. Bir başka deyişle, yerel makinede yer alan özel SiparisKuyrugu isimli kuyruk (Queue) adres olarak ele alınmaktadır.

![mk231_8.gif](/assets/images/2007/mk231_8.gif)

> Burada üzerinde durulması gereken noktalardan biriside şudur. SiparisKuyrugu isimli private kuyruk nasıl oluşturulmuştur? Bunun için sistemde MSMQ'nun kurulu olması elbette şarttır. MSMQ'nun kurulması halinde örneğin Vista işletim sistemi üzerinde Microsoft Management Console açılarak Meesage Queuing klasörüne gidilir. Ardından menüden New->Private Queue seçenği işaretlenir.
> ![mk231_9.gif](/assets/images/2007/mk231_9.gif)
> Bu işlemin ardından, New Private Queue öğesinden yararlanarak kuyruğun adı girilir. Örnek uygulamada ExactlyOnce özelliğine true değeri atanmıştır. Bu, mesajların yanlız bir tane gitmesini ve alınmasını sağlamaktadır. Yani tekrar eden mesajların önüne geçilmektedir. Ancak bunun için MSMQ'nun transactional destekli oluşturulması gerekir. Bu nedenle Transactional seçeneği işaretlenmelidir.
> ![mk231_10.gif](/assets/images/2007/mk231_10.gif)
> Sonuç olarak işlemler tamamlandığında, ilk haliyle aşağıdaki gibi bir sonuç ortaya çıkacaktır. (Servis uygulaması çalıştırıldıktan sonra burada retry isimli bir alt klasör daha oluşturulduğu görülebilir.)
> ![mk231_11.gif](/assets/images/2007/mk231_11.gif)

Burada private'ın özel bir anlamı vardır. Bu tanımlamaya göre mesaj kuyruğunun, servis uygulamasının çalıştığı yerel makinede saklanacağı belirtilmektedir. Ancak Windows Domain'e dahil edilmiş bir kuyruk tanımlaması public olarak yapılarak başka bilgisayarlarında aynı kuyruğu kullanmaları sağlanabilir. Bu özellikle ağa çıkarak hedef kuyruk ile doğrudan konuşamayan alt yapılarda işe yarayabilir. Nitekim böylece ağa çıkamayan istemciler aslında public lokasyonda duran bir kuyruğa mesajlarını gönderirler. Bu kuyruğun bulunduğu makinede kuyruktaki mesajları hedef servise yönlendirir.

Konfigurasyon içerisinde kullanılan bazı önemli ayarlar bulunmaktadır. Bu özelliklerin kısa açıklamaları aşağıdaki tabloda görüldüğü gibidir.

Özellik
Açıklama

Durable
true veya false değerini alabilir. True olması halinde mesajların fiziki diskte saklanacağı belirtilir. Bu I/O işlemlerinin yoğun şekilde kullanılmasını gerektirmektedir. Dolayısıyla sistemin en iyi performansının elde edilmesinde genellikle false olarak ayarlanır. Ne varki false değer verilmesi halinde mesajlar Volatile olarak tutulurlar. Bir başka deyişle makine kapatılıp açıldığında Volatile mesajlara ulaşılamaz.

ExactlyOnce
true veya false değer alabilir. True olması halinde sistemde hareket eden mesajların tekrar edilmeyeceği garanti altına alınır. Bir başka deyişle mesajlar kaybolmaz veya yanlışlıklada olsa iki kere gönderilmez. Bu özelliğin true olmasının bir şartıda kuyruğun transactional olarak tanımlanma zorunluluğudur.

Security kısmından Mode
MSMQ normalde iletişim (Transport Level) ve mesaj (Message Level) seviyesinde güvenlik kullanımına izin verir. Ancak kendisine ait bir iletişim güvenliği söz konusu olduğundan SSL kullanılmasını gerektirmez. Mesaj seviyesinde güvenlik ayarları yapıldığında tek şart vardır o da sertifika desteğinin MSMQ için sağlanmış olması gerekmektedir.

ReceiveRetryCount
Kuyruktan uygulamaya doğru bir mesajın ulaştırılması için maksimum deneme sayısını belirtir. Varsayılan değeri 5 tir.

MaxRetryCycles
Bir mesaj uygulama kuyruğundan retry alt kuyruğuna (SubQueue) transfer edildiğinde, RetryCycleDelay süresinden sonra uygulama kuyruğunda işlenmek üzere tekrardan gönderdilir. Buradaki tekrar sayısı MaxRertyCycle ile belirlenir. Varsayılan değeri 2 dir.

RetryCycleDelay
RetryCycle'lar arasındaki süredir. Varsayılan olarak 30 dakikadır. Bir başka deyişle retry alt kuyruğundan uygulama kuyruğuna ne kadar sürede bir gönderilme denemesi yapılacağı belirtilir.

ReceiveErrorHandling
Maksimum ulaştırma sayısı aşılıp hata oluşması halinde nasıl bir etki olacağı belirtilir. Varsayılan olarak bu değer Fault şeklindedir. Yani uygulamaya bir istisna mesajı fırlatılacaktır. Diğer değerleri ise Drop, Reject ve Move'dur. Vakaya göre uygun olan değerin seçilmesi gerekmektedir.

(Burada söz konusu olan ReceiveRetryCount, MaxRetryCycles, RetryCycleDelay, ReceiveErrorHandling özellikleri zehirli mesajların ele alınması-Poison Message Handling için önemlidir)

Servis tarafı uygulama kodları;

```csharp
using System;
using System.ServiceModel;
using SiparisKutuphanesi;

namespace Servis
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(SiparisYonetici));
            host.Open();
            Console.WriteLine(host.State.ToString());
            Console.WriteLine("Kapatmak için bir tuşa basınız.");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

Artık istemci tarafı programlanmaya başlanabilir. İstemci uygulamada basit bir Console uygulaması olarak tasarlanabilir. İstemci için gerekli Proxy sınıfının üretimi için SvcUtil.exe aracından aşağıdaki ekran görüntüsünde olduğu gibi faydalanmak gerekmektedir.

![mk231_12.gif](/assets/images/2007/mk231_12.gif)

Bu işlemin ardından istemci uygulamanın kodları ve konfigurasyon içeriği aşağıdaki gibi düzenlenmelidir.

İstemci tarafı konfigurasyon dosyası;

```csharp
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <netMsmqBinding>
                <binding name="SiparisIstemciBindingConfig" receiveTimeout="00:10:00" maxRetryCycles="2" receiveErrorHandling="Fault" receiveRetryCount="5" retryCycleDelay="00:30:00">
                    <security mode="None" />
                </binding>
            </netMsmqBinding>
        </bindings>
        <client>
            <endpoint address="net.msmq://localhost/private/SiparisKuyrugu" binding="netMsmqBinding" bindingConfiguration="SiparisIstemciBindingConfig" contract="SiparisServisi" name="SiparisIstemciEndPoint">
            </endpoint>
        </client>
    </system.serviceModel>
</configuration>
```

İstemci tarafı kodları;

```csharp
using System;
using System.ServiceModel;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            SiparisServisiClient client = new SiparisServisiClient("SiparisIstemciEndPoint");
            Console.WriteLine("Başlamak için bir tuşa basınız...");
            client.SiparisEt(1, 100);
            client.SiparisEt(2, 100);
            Console.WriteLine("Kapatmak için bir tuşa basınız...");
            Console.ReadLine();
        }
    }
}
```

Gelelim test aşamasına. İlk olarak istemci uygulamanın çalıştırıldığı varsayılsın. Bu noktada servisin çalışmadığı düşünülebilir. Bu durumda uygulamadaki fonksiyonellikler çalışacak ve metod çağrılarına ait mesajlar kuyruğa yazılacaktır. Bu mesajlar Microsoft Management Console yardımıyla SiparisKuyrugu isimli kuyruktan aşağıdaki gibi görülebilir.

![mk231_13.gif](/assets/images/2007/mk231_13.gif)

Görüldüğü gibi iki metod çağrısı sonrası oluşan mesajlar SiparisKuyrugu isimli kuyruğa atılmıştır. Bunların içeriklerinede bakılabilir. Bu içerikte tahmin edileceği gibi, mesajın hedef (Target) ve kaynak (Source) alıcıları, mesaj içeriği gibi bilgiler yer almaktadır. Bu noktadan sonra servis uygulaması tekrardan çalıştırılırsa kuyruktaki mesajların toplandığı ve işletildiği tespit edilebilir. Nitekim servis uygulaması çalıştırıldıktan sonra SiparisKuyrugu altında başka bir mesaj görünmeyecektir.

![mk231_14.gif](/assets/images/2007/mk231_14.gif)

Buraya kadar anlatılanlar ile WCF mimarisinde basit olarak MSMQ bazlı uygulamaların nasıl geliştirilebileceği incelenmeye çalışılmıştır. MSMQ'nunda bazı dezavantajları elbetteki vardır. Herşeyden önce tek yönlü bir iletişimin olması, diğer iletişim tekniklerine göre daha yavaş çalışması (özellikle kuyruğun fiziki olarak tutulması nedeni ile) bu dezavantajlar arasında sayılabilir.

> MSMQ kullanılacağı yerlerde en iyi senaryoları elde edebilmek için bazı kriterlere dikkat etmek gerekebilir. Söz gelimi hızlı ve yüksek performanslı bir mesajlaşma (Fast-Best Effort Queued Messaging) için transaction bazlı olmayan (Non-Transactional) kuyruklar tercih edilmeli ve buna göre ExactlyOnce özelliği ile Durable özelliklerinin değeri false yapılmalıdır.
> Güvenilir olarak noktadan noktaya bir mesajlaşma söz konusu ise (Reliable End-To-End Queued Messaging), dört alternatif vardır. BasicTransfer, Transactional Based, Dead-Letter Based ve Poison Message Based. Bu alternatiflerden hangisinin kullanılacağına göre yine konfigurasyon elemanlarında bazı değişiklikler yapılmalıdır.
> WCF tabanlı olmayan uygulamalar ile haberleşebilmek için msmqIntegrationBinding tipinin kullanmak gerekmektedir. Buda servisin interoperability desteği olmasını sağlamaktadır.
> Bu tip en iyi çözümler için [http://msdn2.microsoft.com/en-us/library/ms731093.aspx](http://msdn2.microsoft.com/en-us/library/ms731093.aspx) adresinden bilgi alınabilir.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde özet olarak MSMQ teknolojisini WCF içerisinde ele aldık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/MSMQKullanimi.rar)