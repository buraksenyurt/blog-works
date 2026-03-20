---
layout: post
title: "WCF - Replay Attack Etkisini Hafifletmek"
date: 2007-11-07 12:00:00 +0300
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - http
---
WCF (Windows Communication Foundation) ile ilgili bir önceki makalemizde, istemci ve servis arasında güvenilir oturumların (Reliable Session) nasıl açılabileceğinden bahsetmiştik. Güvenilir oturumların yararlarından biriside, olası cevaplama saldırılarının (Replay Attacks) önüne geçmektir. Bilindiği üzere cevaplama saldırılarında, istemci ve servis arasında hareket eden mesajların yakalanarak bozulması, değiştirilmesi, kesilmesi gibi problemler söz konusu olmaktadır. Üstelik değişikliğe uğratılan mesajların zaman içerisinde her hangibir anda, orjinal servis kaynağına yada farklı bir yöne doğru defalarca gönderilmeleride söz konusudur.

Güvenilir oturumlarda, mesajların tekrar etmesini önlemek adına WS-ReliableMessaging şartnamelerine uygun şekilde hareket edilir. Buna göre mesajların benzersiz bir id ile işaretlenmesi ve sıralarının belirlenmesi için numaralandırılması söz konusudur. Ne yazıkki güvenilir bir oturum cevaplama saldırılarını tek başına karşılamakta yeterli olmayabilir. Aslında bunun için geçerli ve yeterli bir senaryo vardır. Aşağıdaki şekil cevaplama saldırılarına ait bir vakayı ifade etmektedir.

![mk230_1.gif](/assets/images/2007/mk230_1.gif)

Bu senaryoda istemci ve servis arasında hareket etmekte olan 7 adet mesaj olduğu varsayılmaktadır. Güvenilir bir oturum sağlandığı düşünülecek olursa, istemcinin servis tarafına göndereceği her mesajda bir sıra numarası olacağı söz konusudur. Ne varki senaryoya göre 3 numaralı mesaj hacker tarafından yakalanır. Sonrasında ise mesajın içeriği ve numarası 6 olarak değiştirilerek servis tarafına gönderilir. Bilindiği gibi güvenilir oturumlarda, servise gelen mesajların farklı zamanlarda ulaşma ihtimali göz önüne alınarak, bu mesajların istemciden gönderildikleri sırada alınmaları için buffer (tampon) sistemi kullanılır. İlerleyen zaman aralığında istemci gerçek 6 numaralı mesajı servis tarafında gönderir. Oysaki 6 numaralı mesaj zaten servis için açılan buffer içerisinde durmaktadır. Dolayısıyla asıl 6 numaralı mesaj servis tarafından geri çevrilerek işlenmeyecektir. İşte bu durum güvenilir oturumların cevaplama saldırıları için tek başına yeterli bir çözüm olmayışının ispatıdır.

Çözümsel bir yaklaşım olarak mesaj seviyesinde (Message Level) veya iletişim seviyesinde (Transport Level) güvenlik göz önüne alınabilir. Ancak bunlarda tek başlarına yeterli değildir. Mesaj seviyesinde güvenlikte, istemci ve servis arasındaki mesaj bilgileri şifrelenmektedir. Ne varki mesaj seviyesinde güvenlik uyarlamaları, çoğunlukla mesaj gövdesindeki içeriğin şifrelenmesi ile ilgilenir. Bu sebepten mesajın başlık (Header) kısmı yinede hacker'lar tarafından yakalanabilir ve sıra numaraları bozulabilir. Peki iletişim seviyesinde güvenlik uyarlamalarında durum nedir? Bu seviyede güvenlik oldukça güçlü bir çözümdür. Ancak iletişim seviyesinde güvenlik uyarlamaları, noktadan-noktaya çalışırlar. Bu konu için şöyle bir örnek düşünülebilir; bir servisin mesajını gönderdiği hedefte başka bir servis olabilir. Mesajı alan servisin, mesajların içeriğine sınırsız erişim yetkisi vardır. Eğer merkez servis aldığı mesajı başka bir servisede gönderiyorsa mesajı bozmadığından emin olmak gerekmektedir.

Bu ana kadar yazılıp çizilenler söz konusu olduğunda, cevaplama saldırılarına karşı tam anlamıyla bir çözüm bulunamadığı düşünülebilir. Lakin, WCF içerisinde özel bir takım teknikler yardımıyla cevaplama saldırılarına karşı daha güçlü durulabilir. Tabi tüm bu tedbirsel teknikler güvenilir oturumlar (Reliable Session) üzerine kurulmaktadır. Bu tedbirler aktifleştirildiğinde, WCF çalışma zamanı (Run-Time) her mesaj için aşağıdaki kriterlere uygun bir tanımlayıcı (Identifier) üretir.

- Benzersiz (Unique)
- Rastgele elde edilmiş (Random)
- İşaretlenmiş (Signed)
- Zaman Damgalı (TimeStamp)

Bu değerlerden oluşan tanımlayıcılar (Identifiers) nonce olarak isimlendirilmektedir. Nonce'ların kullanıldığı bu korunma sisteminin çalışma şekline göre, servise gelen mesajların bozulup bozulmadıkları (Corrupt) bir dizi işlem ile anlaşılmaya çalışılır. Normalde servis tarafına gelen mesajların nonce değerleri tampona (buffer) alınır. Başka bir mesaj geldiğinde başlık bilgilerinden elde edilen nonce değerinin, tamponda olup olmadığına bakılır. Elde edilen nonce değerinin aynısı tamponda var ise gelen mesaj geri çevrilir. Elbette nonce değeri tamponda yoksa mesaj kabul edilir. Kabul edilen bu mesajın nonce değeri yine tampona eklenir. Nonce'ların kullanıldığı bu sistemi tesis edebilmek için WCF içerisinde özel bağlayıcı tiplerin (Custom Binding Types) tanımlanması gerekmektedir.

Windows Communication Foundation çalışma zamanı (WCF Run-Time) mesajların alınıp gönderilmesi sırasında kanal yığınlarını (Channel Stack) kullanır. Mesajlar normal şartlarda bir adrese doğru hareket ederler. Bu hareket TCP, HTTP gibi bir iletişim protokolü üzerinden aktarılırlar. Bu sebepten mesajın ulaştığı yerde bulunan WCF çalışma zamanı (WCF Run-time) karşılama için iletişim kanallarını (Transport Channels) kullanır. Bu kanal, servis tarafındaki host nesnesi açıldığı andan itibaren gelen mesajları dinlemektedir. Servis tarafında uygun iletişim kanalı tarafından alınan mesajlar, daha sonra kodlama kanallarına (Encoding Channels) yönlendirilir. Kodlama kanallarının görevi istemciden gelen mesajları çözümleyerek servis nesnesine iletmek ve servisin istemiceye göndereceği mesajlarıda kodlayarak iletişim kanalına (Transport Channel) iletmektir. Çok doğal olarak iletişim kanalından geçerek mesaj kanalına gelen bilginin Text veya Binary tabanlı olma durumu (hatta WS şartnamelerine göre Microsoft Transmision Opitimision Mechanism'a uygun bir biçimde olması) söz konusudur. Aşağıdaki şekil temel olarak söz konusu kanallar arasındaki iletişimi ifade etmektedir.

![mk230_2.gif](/assets/images/2007/mk230_2.gif)

Servis tarafı göz önüne alındığında en azından iki kanalın var olması gerekmektedir. Dolayısıyla cevaplama saldırılarına karşı savunma yapılırken oluşturulacak olan özel kanal tipleri (Custom Binding Types) geliştirilirken bunlara dikkat edilmelidir. Diğer taraftan WCF içerisinde bu tip bağlayıcı tipleri hazırlamak son derece kolaydır. Basit olarak Service Configuration Editor bu amaçla kullanılabilir. Bu hazırlıklar yapılırken bağlayıcı tip içerisine katılan kanalların sırasıda önemlidir.

Artık bir örnek üzerinden hareket edilerek devam edilebilir. Her zamanki gibi servis sözleşmesi (Service Contract) ve uyarlamasını içeren bir WCF Servis kütüphanesi tasarlayarak işe başlamak gerekir. Söz konusu servis sözleşmesi içerisinde basit olarak işlevselliği çok önemli olmayan iki metod yer almaktadır. Güvenilir oturum (Reliable Session) sağlanması adına bir oturum var olma gerekliğine uygun olacak şekilde tanım yapılmaktadır. Söz konusu servis kütüphanesi içerisinde tanımlı tipler aşağıda görüldüğü gibidir.

![mk230_3.gif](/assets/images/2007/mk230_3.gif)

Servis sözleşmesi (Service Contract);

```csharp
using System;
using System.ServiceModel;

namespace SiparisKutuphanesi
{
    [ServiceContract(Name="Siparis Servisi", Namespace="http://www.bsenyurt.com/SiparisServisi", SessionMode= SessionMode.Required)]
    public interface ISiparisYonetimi
    {
        [OperationContract(IsInitiating=true)]
        void SiparisEkle(int siparisNo);
        [OperationContract(IsInitiating=false,IsTerminating=true)]
        void SiparisleriOnayla(); 
    }
}
```

Sözleşme Uyarlaması;

```csharp
using System;
using System.ServiceModel;

namespace SiparisKutuphanesi
{
    [ServiceBehavior(InstanceContextMode= InstanceContextMode.PerSession)]
    public class SiparisYonetimi:ISiparisYonetimi
    {
        #region ISiparisYonetimi Members

        public void SiparisEkle(int siparisNo)
        {
            // Siparis ekleme adımı
        }

        public void SiparisleriOnayla()
        {
            // Siparis onaylama adımı
        }

        #endregion
    }
}
```

Bu işlemin ardından servis ve istemci taraflarının tasarlanmasına geçilebilir. Olayın basit bir şekilde ele alınması için servis ve istemci tarafındaki programlar basit birer Console uygulaması olarak tasarlanabilir. Bu uygulamalarda belkide en önemli kısımlar konfigurasyon ayarlarıdır. Nitekim konfigurasyon tarafında özel bağlayıcı tip (Custom Binding Type) tanımlamaları yapılacaktır. Servis tarafındaki Console uygulamasının başlangıçtaki hali aşağıdaki gibidir.

Servis uygulaması kod içeriği;

```csharp
using SiparisKutuphanesi;

namespace Servis
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(SiparisYonetimi));
            host.Open();
            Console.WriteLine(host.State.ToString());
            Console.WriteLine("Kapatmak için bir tuşa basınız");
            Console.ReadLine();
            if (host.State == CommunicationState.Opened)
            host.Close();
        }
    }
}
```

Servis tarafındaki konfigurasyon dosyasının ilk hali;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name="SiparisKutuphanesi.SiparisYonetimi">
                <endpoint address="net.tcp://localhost:4500/SiparisServisi.svc" binding="netTcpBinding" name="SiparisServisiEndPoint" contract="SiparisKutuphanesi.ISiparisYonetimi"/>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Başlangıçta servis tarafı netTcpBinding bağlayıcı tipini kullanacak şekilde tasarlanmıştır. Ancak cevaplama saldırılarının önüne kesmek için burada özel bir bağlayıcı tip tasarlanacaktır. Şimdi adım adım bu işlemler gerçekleştirilecektir. İlk olarak, Service Configuration Editor üzerinden aşağıdaki ekran görüntüsünde yer aldığı gibi New Binding Configuration bağlantısına tıklanır.

![mk230_4.gif](/assets/images/2007/mk230_4.gif)

Sonrasıdan açılan Create a New Binding kısmından customBinding seçilerek OK tuşuna basılır. Bu işlemin sonrasında oluşan duruma göre özel bağlayıcı tipin adı, aşağıdaki ekran görüntüsünde olduğu gibi SiparisServisiOzelBaglayici olarak değiştirilebilir.

![mk230_5.gif](/assets/images/2007/mk230_5.gif)

İlk bakıldığında iletişim kanalı olarak httpTransport tipinin ve mesajlaşma kanalı olarakta textMessageEncoding'in kullanıldığı görülmektedir. Örnekte TCP protokolü üzerinden bir haberleşme hedeflendiği için httpTransport kanalı kaldırılmalıdır. Bu işlem için aynı ekranda, httpTransport seçili iken Remove tuşuna basılması yeterlidir. tcpTransport kanalının eklenmesi için aşağıdaki ekran görüntüsünde işaretlenen adımların sırasıyla yapılması yeterlidir.

![mk230_6.gif](/assets/images/2007/mk230_6.gif)

Dikkat edilecek olursa Available Elements kısmında, özel bağlayıcı tip içerisinde kullanılabilecek pek çok kanal çeşidi bulunmaktadır. tcpTransport tahmin edileceği üzere bir iletişim kanalı olarak servise gelen mesajların TCP protokolü üzerinden alınmasını sağlamaktadır. Örnekte mesajlaşma kanalı olarak textMessageEncoding kullanılmaktadır. Bu nedenle oluşturulan özel bağlayıcı tip içerisinde gelen ilgili kanalın kaldırılmasına gerek yoktur.

Sıradaki adımda güvenlik ile ilgili bir kanalın eklenmesi gerekmektedir. Bu amaçla yine Add düğmesi ile (yada Add Binding Element Extension bağlantısı kullanılarak) açılan pencereden security elementi seçilmelidir.

![mk230_7.gif](/assets/images/2007/mk230_7.gif)

security elementi içerisinde ise AuthenticationMode özelliğinin değeri aşağıdaki ekran görüntüsünde olduğu gibi SecureConversation olarak ayarlanır.

![mk230_8.gif](/assets/images/2007/mk230_8.gif)

SecureConversation, Organization for the Advancement of Structured Information Standards (OASIS - [http://www.oasis-open.org/home/index.php](http://www.oasis-open.org/home/index.php)) tarafından kabul edilmiş olan WS-SecureConversation şartnamelerine uygun olacak şekilde güvenli bir oturumun sağlanması garanti etmektedir.

Özet olarak WS-SecureConversation, iki katılımcı arasındaki (örnek senaryoya göre istemci ve servis) mesajlaşmada ehliyet bilgilerinin tamamının gönderilmesini gerektirmeyecek bir ortam sağlamaktadır. Bunun sağlanabilmesi için oturumun en başında, istemci ve servis arasında ehliyet (Credential) bilgileri değiş tokuş edilir ve doğrulanır. Geri kalan mesajlaşmalarda başlangıçtaki ehliyet bilgilerinden türeyen güvenlik fişleri (security tokens) kullanılır. Bir başka deyişle oturum başında zaten taraflar ehliyet bilgileri ile birbirlerini doğruladıklarından, kalan mesajlaşmalarda aynı bilgiler tekrardan kontrol edilmez. Buda çok doğal olarak mesajlaşmanın daha hızlı gerçekleştirilmesini sağlamaktadır.

SecureConversation seçildikten sonra, yine security elementinde servis tarafına yönelik olacak şekilde bazı ayarları kontrol etmek gerekmektedir. İlk olarak aşağıdaki ekran görüntüsünde görüldüğü gibi DetectReplays seçeneğinin true olması sağlanmalıdır ki varsayılan olarak böyledir.

![mk230_9.gif](/assets/images/2007/mk230_9.gif)

Bu kısımda oldukça fazla sayıda ayar ve kafa karıştırıcı özellik yer almaktadır. Ancak en çok dikkat çekenlerden birisi ReplayCacheSize değeridir. Buraya atanan değer, tamponda tutulacak olan nonce'ların sayısıdır. Söz konusu değerin dışına taşılması durumunda, tamponda duran en eski nonce değeri atılacak ve yerine son gelen nonce değeri yazılacaktır. Ancak varsayılan 900000 değeri pek çok vaka için yeterli bir sayıdır. Bellek optimizasyonu adına bu değerin azaltılmasıda düşünülebilir. Fakat az öncede belirtildiği gibi, sayının dışına çıkılması halindeki durumlar göz önüne alınmalıdır. Nitekim eski nonce'ların silindiği ve yeni gelenlerin yazıldığı sıralarda sistem cevaplama saldırılarına (Replay Attack) karşı kısa sürelide olsa savunmasız kalabilir. Bu işlemlerin ardından elbetteki bağlayıcı tipin güvenilir bir oturum (Reliable Session) açabilmesi için, yine Add düğmesi ile açılan pencereden ReliableSession elementi seçilmelidir.

![mk230_10.gif](/assets/images/2007/mk230_10.gif)

Artık özel bağlayıcı tip içerisinde kullanılacak olan tüm kanal ve özellikler belirlenmiştir. Son aşamada dikkat edilmesi gereken nokta söz konusu kanalların uygulanış sırasıdır. Bir başka deyişle kanal yığını (Channel Stack) içerisindeki sıranın önemi vardır. Yukarıda geliştirilen örneğe göre sıra aşağıdaki ekran görüntüsündeki gibi olmalıdır. Buna göre reliableSession ile başlayan sıra security, textMessageEncoding (Mesajlaşma Kanalı) ve tcpTransport (iletişim kanalı) şeklinde devam etmelidir. Bu sırayı ayarlamak için Up ve Down başlıklı düğmeler kullanılabilir.

![mk230_11.gif](/assets/images/2007/mk230_11.gif)

Elbette oluşturulan bu özel bağlayıcı tipin (Custom Binding Type) kullanılabilmesi için endPoint ile ilişkilendirilmesi gerekmektedir. Bu sebepten tek yapılması gereken SiparisServisiEndPoint isimli endPoint seçili iken, Binding özelliğinin değerinin customBinding olarak işaretlenmesidir.

![mk230_12.gif](/assets/images/2007/mk230_12.gif)

Bu işlem sırasında Binding değeri customBinding olarak seçildiğinde BindingConfiguration özelliğinin değeri otomatik olarak SiparisServisiOzelBaglayici olarak değişecektir. Servis tarafında yapılan bu ayarlardan sonra konfigurasyon dosyasının son hali aşağıdaki gibi olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <customBinding>
                <binding name="SiparisServisiOzelBaglayici">
                    <reliableSession />
                    <security authenticationMode="SecureConversation">
                        <secureConversationBootstrap />
                    </security>
                    <textMessageEncoding />
                    <tcpTransport />
                </binding>
            </customBinding>
        </bindings>
        <services>
            <service name="SiparisKutuphanesi.SiparisYonetimi">
                <endpoint address="net.tcp://localhost:4500/SiparisServisi.svc" binding="customBinding" bindingConfiguration="SiparisServisiOzelBaglayici" name="SiparisServisiEndPoint" contract="SiparisKutuphanesi.ISiparisYonetimi" />
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Çok doğal olarak burada yapılan konfigurasyon değişikliklerinin istemci tarafındaki uygulamada da yapılması gerekmektedir. Ama öncesinde istemci için gerekli proxy sınıfının svcutil aracı yardımıyla aşağıdaki gibi üretilmesi sağlanmalıdır.

![mk230_13.gif](/assets/images/2007/mk230_13.gif)

Sonrasında ise bu proxy sınıfı Console tipinden tasarlanan istemci uygulamaya taşınarak kullanılır. İstemci uygulamanın kodları ve konfigurasyon dosyasının içeriği ise aşağıdaki gibidir.

İstemci uygulama kodları;

```csharp
using System;
using System.ServiceModel;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sipariş vermek için tuşa basın...");
            Console.ReadLine();
            SiparisServisiClient cli = new SiparisServisiClient("IstemciEndPoint");
            cli.SiparisEkle(1);
            cli.SiparisEkle(4); 
            cli.SiparisleriOnayla();
            Console.WriteLine("İşlemler tamamlandı...Çıkmak için bir tuşa basınız");
            Console.ReadLine(); 
        }
    }
}
```

İstemci uygulama basit olarak servis üzerinden SiparisEkle ve SiparisleriOnayla metodlarını çağırmaktadır.

İstemci tarafı konfigurasyon dosyası;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <bindings>
            <customBinding>
                <binding name="SiparisIstemciOzelBaglayici">
                    <reliableSession />
                    <security authenticationMode="SecureConversation">
                        <secureConversationBootstrap />
                    </security>
                    <textMessageEncoding />
                    <tcpTransport />
                </binding>
            </customBinding>
        </bindings>
        <client>
            <endpoint address="net.tcp://localhost:4500/SiparisServisi.svc" binding="customBinding" bindingConfiguration="SiparisIstemciOzelBaglayici" contract="SiparisServisi" name="IstemciEndPoint">
            </endpoint>
        </client>
    </system.serviceModel>
</configuration>
```

İstemci tarafındaki konfigurasyon dosyasının daha kolay üretilmesi amacıyla, istemci uygulamaya standard bir app.config dosyası eklendikten sonra Service Configuration Editor yardımıyla açılan pencerede Create a New Client sonrası açılan New Client Element Wizard bölümü kullanılabilir. Burada From service config seçili iken Config File kısmına servis uygulamasındaki konfigurasyon dosyasını işaret etmek yeterlidir. Yanlız burada proxy sınıfı kullanıldığı için sözleşme arayüzü (Contract Interface) adının servis tarafındaki gibi olmadığı unutulmamalıdır. Bu nedenle bu üretim sonrasında oluşan konfigurasyon içerisinde endPoint elementinde yer alan binding niteliğinin (attribute) değeri uygun şekilde değiştirilmelidir. Örnekte bu değer SiparisServisi olarak değiştirilmelidir.

![mk230_14.gif](/assets/images/2007/mk230_14.gif)

Gelinen bu noktadan sonra sistem test edilebilir. Söz konusu sistem cevaplama saldırılarına karşı önlem alan özel bir bağlayıcı tipi kullanmaktadır. Örnek uygulamalarda yine logların izlenmesi işlemi gerçekleştirilirse mesajların içeriğinde yazının başındada belirtilen tanımlama değerlerinin yer aldığı (timestamp gibi...), bununla birlikte mesajların boyutlarının dahada arttığı görülür. Mesaj boyutlarındaki bu artış çok doğal olarak paketlerin büyümesi anlamınada gelmektedir. Ancak vaka içerisinde Replay Attack olasılığı var ise bu göz ardı edilmeli ve gereken tedbirler alınmalıdır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/ReplayAttacks.rar)