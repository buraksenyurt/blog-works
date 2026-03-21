---
layout: post
title: "WCF - P2P(Peer-to-peer) Programlamaya Giriş"
date: 2008-05-25 09:00:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
---
Dağıtık mimari uygulamaları (Distributed Applications) geliştirilirken çoğunlukla Client/Server veya N-Tier modelleri göz önüne alınmaktadır. Oysaki dağıtık mimari uygulamaları için Peer-to-Peer (P2P) modelide söz konusudur. P2P modelinde istemci ve sunucu arasında bir fark yoktur ve alt yapı hazırlıkları diğer modellere göre biraz daha karmaşıktır. Programlama zorluğu nedeni ile geliştiriciler zaman zaman bu modelden kaçınırlar.

Oysaki günümüzde P2P üzerine kurulu olan pek çok sistem yer almaktadır. P2P uygulamalarına örnek olarak, IRC (Internet Relay Chat), anında mesajlaşma (Instant Messaging), dosya paylaşım (File Sharing), Oyunlar, görsel ve sesli veri aktarımı, veri kopyalama (Data Replication) programları gösterilebilir. Dolayısıyla P2P modelinin dağıtık mimari çözümlerinde aslında önemli bir yeri bulunmaktadır. P2P tabanlı sistemlerde özellikle ölçeklenebilirlik (Scalability) ve güvenilirlik (Reliability), diğer dağıtık mimari modelleri ile kıyaslandığında daha yüksek verimlilik göstermektedir.

> P2P programlar başarılı bir şekilde tesis edildiklerinde scalability ve reliability manasında belirgin üstünlük ve avantajlar sağlamaktadır.
> Scalability: Kaynak kullanımının artmasıyla sistem performansının değer kaybetmeden yükselebilmesi ölçeklenebilirlik olarak ifade edilebilir. Bu oranın yüksek olması bir avantaj olarak görülebilir.
> Reliability: Kurulu olan sistemin güvenilirliğini, aynı zamanda devamlılığını ifade eder.

Çok doğal olarak Windows Communication Foundation (WCF) içerisindede P2P desteği bulunmaktadır. Bu bölümde WCF mimarisi ile P2P çözümlerinin nasıl geliştirilebileceğine giriş yapılmaktadır. Başlamadan önce diğer dağıtık mimari modellerine kısaca bir göz atmakta ve P2P ile aralarındaki farkları analiz etmeye çalışmakta yarar bulunmaktadır. Client/Server (İstemci/Sunucu) modeli aşağıdaki şekil ile basitçe özetlenebilir.

![mk252_1.gif](/assets/images/2008/mk252_1.gif)

Bu modelde istemci, sunucu üzerinden yayınlanan fonksiyonellikler için talepte (Request) bulunur. Sunucunun görevi ise bu taleplere karşılık cevaplar (Response) üretmektir. Bu mimariye verilebilecek en güzel örnek Web sunucuları ve tarayıcı uygulamalardır (Browser Applications). Söz gelimi IIS (Internet Information Services) sunucu görevini üstlenirken, Internet Explorer, Netscape Navigator, Mozilla Firefox gibi tarayıcı uygulamalar istemci rolünü yüklenmektedir. Bu modelde aslında istemci ve sunucu uygulamalar aynı sistemin bir parçasıdır.

Her zaman IIS yada Internet Explorer gibi hazır program arayüzleri olmayabilir. Söz gelimi WCF mimarisinde Self Hosting tekniğine göre sunucu program, Console, Windows, WPF, Windows Service, WAS (Windows Activation Service) uygulaması olacak şekilde geliştirilebilir. Buna paralel olarak istemci tarafı içinde aynı durum geçerlidir. Yinede sonuç itibariyle istemci tarafı talepte bulunan, sunucu tarafı ise bu talepleri karşılayan roldedir. Client/Server modelinde merkezileştirme olanağının bulunması bir avantaj olarak görülebilir.

N-Tier yada çok katmanlı mimaride, dağıtık uygulama geliştirme modellerinden birisidir. Aslında bu model dağıtık mimari tarafında çoğunlukla 3 katmanlı (3-Tier) olacak şekilde uygulanmaktadır. Aşağıdaki şekilde bu durum kısaca ifade edilmeye çalışılmaktadır.

![mk252_2.gif](/assets/images/2008/mk252_2.gif)

Bu model aslında Client/Server mimarinin genişletilmiş bir hali olarak düşünülebilir. Katmanlar ayrı fiziki parçalara bölümlenebilmektedir. Dağıtık mimari açısından özellikle iş mantığının (Business Logic) farklı bir makineye alınması ölçeklenebilirliği (Scalability) arttıran bir faktördür. Hatta bu fiziki bölünme ile yük dengesinin dağıtılması (Load Balancing), daha güvenli (Secure) bir ortamın tesis edilmesi gibi avantajlarda sağlanabilmektedir. Tabiki, bu fiziki bölünüm için donanıma yapılan yatırımda bu işin cabası olarak görülebilir.

P2P modelinde ise sistemin tüm katılımcıları (ki bunlar peer node-boğum olaraktanda adlandırılmaktadır) çoğunlukla hem istemci hemde sunucu görevini üstlenebilmektedir. Client/Server veya N-Tier modelleri ile dağıtık uygulama çözümleri geliştirilmesi P2P'e göre daha kolaydır. Ayrıca yönetimin merkezileştirilmesi yada güvenliğin güçlü bir şekilde sağlanmasıda söz konusudur. Fakat bölümün başındada belirtildiği üzere, ölçeklenebilirlik (Scalability) ve güvenilirlik (Reliability) gibi konularda P2P mimarisi gerçekten öne çıkmaktadır. (Tabiki, ölçeklenebilirlik olgusu, P2P dışındaki modellerdede vardır ancak maliyeti daha yüksek olmaktadır. Özellikle sunucu yatırımları göz önüne alındığında.) Elbetteki bu farklılıklar gerçek hayat vakalarında duruma göre tercih edilebilirler. Hatta bazı vakalarda karma modellerin kullanıldığıda görülmektedir.

Genel olarak P2P modelinde yer alan uygulamalar bir Mesh Network içerisinde gruplanırlar. Söz konusu Mesh Network'lerin iki farklı uygulanış biçimi vardır. Bunlardan birisi Parçalı Bağlı Mesh (Partially Connected Mesh) modelidir ve aşağıdaki şekildekine benzer bir yaklaşım sunmaktadır.

![mk252_4.gif](/assets/images/2008/mk252_4.gif)

Bu modele göre Mesh Network içerisinde yer alan boğumlar (Peer Nodes) yakınlarındaki komşularına doğrudan bağlıdır. Bir başka deyişle sistem içerisindeki programlar en yakın bilgisayardaki ile konuşabilmektedir. (Birbirlerinin komuş boğumları olarakta görülebilirler.) Bu modelde boğumların tamamı birbirlerine bağlı değildir. Bu nedenle örneğin komşu olmayan bir boğumda yer alan katılımcıya mesaj aktarımı için, mesajın sırayla birbirlerine bağlı olan boğumlar üzerinden hareket etmesi gerekmektedir. Bu bir dezavantaj olarak görülebilir. Diğer taraftan bu tip sistemlerde Mesh Network'e dahil olan katılımcı sayısının artması ile birlikte, ölçeklenebilirliğinde arttrığı gözlemlenmektedir. Doğal olarak bu bir avantajdır. Diğer modelde ise Mesh Network içerisinde yer alan tüm katılımcılar (PeerNode) birbirlerine bağlıdır. Bu durum aşağıdaki şekil ile özetlenebilir.

![mk252_3.gif](/assets/images/2008/mk252_3.gif)

Çoğunlukla Mesh Network içerisindeki katılımcı sayılarının düşük olduğu durumlarda tercih edilen bir modeldir. Çok doğal olarak Mesh ağlarında katılımcı uygulamaların birbirlerini bulabilmesi veya haberleşebilmesi için bir takım protokollerin rol oynaması gerekmektedir. Söz gelimi UDP (User Datagram Protocol) gibi. Windows Communication Foundation bu amaçla Peer Name Resolution Protocol (PNRP) isimli protokolü varsayılan olarak kullanmaktadır. Bu protokol Windows XP Service Pack 2 ve Windows Vista ile birlikte gelmektedir. Ancak istenirse PNRP yerine kullanılacak özel çözücüler (Resolver) ele alınabilir ki bu bölümde ele alınacaktır.

> PNRP (Peer Name Resolution Protocol) protokolü sistemde yüklü değilse, Windows XP Service Pack 2 için Add Remove Windows Components kımsına girip, Network Services bileşenlerinden Peer to Peer birimini eklemek yeterli olacaktır.
> ![mk252_5.gif](/assets/images/2008/mk252_5.gif)
> Özellikle uygulama geliştirildikten sonra "System.InvalidOperationException: Resolver must be specified. The default resolver (PNRP) is not available." gibi bir çalışma zamanı istisnasının (Runtime Exception) alınmasının nedeni ilgili PNRP hizmetinin yüklenmemiş olmasıdır.
> PNRP yüklenmesi tek başına yeterli değildir. Peer Name Resolution Protocol Service çalışıyor olması gerekmektedir. Bunun için Administrator Tools bölümden yer alan Services kısmından yararlanılabilir yada komut satırından net start pnrpsvc emri aşağıdaki ekran görüntüsünde oldugu gibi uygulanabilir.
> ![mk252_6.gif](/assets/images/2008/mk252_6.gif)
> Ne varki PNRP sadece XP ve Vista için geçerlidir. Söz gelimi Windows Server 2003 sisteminde çalıştırılamamaktadır. Diğer taraftan PNRP IPv6 alt yapısını kullanmaktadır. Bu nedenle IPv6 desteği olmayan yönlendiricilerin (Routers) olduğu sistemlerde ala alınamamaktadır. Ayrıca Vista ile birlikte PNRP 2.0 versiyonu gelmektedir. Buna göre XP ile Vista sistemlerin birbirleriyle konuşması gerektiği durumlarda XP sistemler için ek yükleme yapılması gerekmektedir. İşte bu tip sorunların olabileceği vakalarda, özel peer çözücülerinin (Custom Peer Resolver) geliştirilmesi ve kullanılması önerilmektedir. Bu sayede Mesh Network içerisine dahil olan katılımcıların tutuluş şekilleride özelleştirilebilir. Söz gelimi veritabanı üzerinde saklanabilir.

P2P programlamada önem arz eden konulardan biriside mesajların ulaştırılma şeklidir. Bu noktada iki farklı teknik bulunmaktadır. Directional ve Flooding. Directional mesajlaşmaya göre, Mesh Network içerisinde yer alan herhangibir boğumdan (Node) çıkan mesaj, hedef boğuma (node) ulaşıncaya kadar komşu boğumlar üzerinden yönlendirilir. Flooding haberleşmede ise mesaj, Mesh Network içerisindeki tüm boğumlara gönderilir ve mesajı alması gereken boğum tarafından yakalanır. Çok doğal olarak Flooding modelinde aynı Mesh Network içerisinde yer alan bir boğumda çalışan uygulamaya mesajın defalarca gelmesi söz konusu olabilmektedir. WCF mimarisi, P2P programlamada Flooding mesajlaşma modelini kullanmaktadır. Bununla birlikte WCF geliştiricisi için en önemli olan noktalardan birisininde bağlayıcı tip seçimi olduğu bilinmektedir. WCF, P2P için bağlayıcı tip (Binding Type) olarak NetPeerTcpBinding sınıfını ele alınmaktadır.

Bu basit teknik detaylardan sonra WCF tarafından P2P tarzı bir uygulamanın nasıl geliştirilebileceği adım adım incelenebilir. Örnekte PNRP hizmeti yerine CustomPeerResolverService tipi kullanılarak bir servis uygulaması tasarlanacak ve IRC benzeri basit bir program ortamı geliştirilecektir. Bu çözümsel yaklaşım aynı zamanda Microsoft tarafındanda önerilmektedir. Bu amaçla özel çözücüyü içeren ayrı bir servis uygulaması yazılmalıdır. Örnek olarak bu bir Console uygulaması olabilir. Söz konusu uygulamanın kod içeriği aşağıdaki gibidir.

Özel Çözücü Service Kodu;

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.PeerResolvers;

namespace IRCServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomPeerResolverService resolver = new CustomPeerResolverService();
            ServiceHost customResolver = new ServiceHost(resolver);

            resolver.Open();
            customResolver.Open();
            Console.WriteLine("Çözümleyici aktif. Çıkmak için bir tuşa basınız...");
            Console.ReadLine();
        }
    }
}
```

Burada dikkat edilmesi gereken ilk nokta CustomPeerResolverService sınıfına ait bir nesne örneği kullanılmasıdır. CustomPeerResolverService sınıfı özel peer çözücü için gerekli temel uyarlamaları (Basic Implementations) içermektedir. Söz konusu tip IPeerResolverContract arayüzünden (Interface) türetilmiştir. IPeerResolverContract arayüzü özellikle peer uyarlamasında özel işlemler yapılması istendiği durumlarda ele alınmaktadır. Söz gelimi peer çözümleyicisinin veritabanı ilişkili olması isteniyorsa, IPeerResolverContract arayüzünü uygulayan bir sınıfın yazılması ve ilgili üyelerin ezilmesi gerekmektedir. Sonrasında ise uygulama içerisinde bu sınıfın, peer çözücü olarak kullanılacağı belirtilmelidir. Örnekte böyle bir ihtiyaç olmadığından.Net Framework 3.0 ile birlikte gelen hazır CustomPeerResolverService sınıfı kullanılmaktadır.

Çözücü Servise ait konfigurasyon içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name="System.ServiceModel.PeerResolvers.CustomPeerResolverService">
                <endpoint address="net.tcp://localhost:4500/IRCResolver" binding="netTcpBinding" bindingConfiguration="ResolverBindingConfig" contract="System.ServiceModel.PeerResolvers.IPeerResolverContract" />
            </service>
        </services>
        <bindings>
            <netTcpBinding>
                <binding name="ResolverBindingConfig">
                    <security mode="None"/>
                </binding>
            </netTcpBinding>
        </bindings>
    </system.serviceModel>
</configuration>
```

services elementi içerisine dikkat edilecek olursa, net.tcp://localhost:4500/IRCResolver adresi üzerinden NetTcpBinding bağlayıcı tipini ele alan, herhangibir güvenlik modunun kullanılmadığı ve IPeerResolverContract sözleşmesini (Service Contract) bildiren bir EndPoint tanımlaması yapıldığı görülmektedir. Elbette, Mesh Network içerisine dahil olacak olan katılımcıların (PeerNode) birbirleriyle haberleşebilmeleri için tasarlanan bu özel peer çözücü servis uygulamasının çalışıyor olması gerekmektedir. Çok doğal olarak bu uygulama ağ ortamında bir sunucu üzerinde host edildiği takdirde (Söz gelimi Windows Server 2003 üzerinde koşan bir Windows Service içerisinden sunulabilir) ağa bağlı tüm istemci uygulamaların birbirleriyle P2P üzerinden haberleşebilmesi sağlanabilir. Üstelik TCP bazlı yayınlama yapıldığı için performans açısından da oldukça tatmin edici sonuçlar alınacaktır.

İstemci tarafında yer alacak ve Mesh ağına katılımcı (PeerNode) olarak dahil olacak program basit bir Windows uygulaması olarak tasarlanmaktadır. Bu uygulamanın görsel arayüzü aşağıdaki gibidir.

![mk252_7.gif](/assets/images/2008/mk252_7.gif)

Kullanıcılar istedikleri bir isim ile Mesh ağına dahil olabileceklerdir. Bununla birlikte mesaj göndermek, diğer katılımcıların mesajlarını görmek veya IRC kanalından ayrılmak gibi işlemleride yapabileceklerdir. Katılımcılar için önemli olan üç operasyon söz konusudur. Mesh ağına dahil olmak, ağdan ayrılmak ve ağdaki herkese mesaj gönderebilmek. Bunların bir servis sözleşmesine ait operasyonlar olduğu açıktır. Dolayısıyla istemci tarafında bu işlevsellikleri barındıran bir sözleşme arayüzü tasarlanmalıdır. Diğer taraftan bu tip bir P2P çalışmada, kanalın çift yönlü iletişim (Duplex Communication) sağlayacak şekilde çalışabiliyor olması ve hatta söz konusu operasyonların asenkron olarak yürütülebilmesi gereklidir. Bu sebepten DuplexChannel kullanılması ve operasyonların OneWay olarak işaretlenmesi gereklidir. Windows uygulamasına ait olan Form sınıfının kendisi bu senaryoda sözleşmeyi (Service Contract) uygulayan tip olacaktır. Bunlara ek olarak istemci tarafından açılacak olan kanal içinde IClientChannel arayüzünün hesaba katılması gerekmektedir. Katılımcı uygulama tarafından bakıldığında sınıf diagramı (Class Diagram) içeriği aşağıdaki gibidir.

![mk252_8.gif](/assets/images/2008/mk252_8.gif)

![mk252_9.gif](/assets/images/2008/mk252_9.gif)

Katılımcı uygulamanın kodları aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ChatApp
{
    public partial class Form1 
        : Form
        ,IIRCSozlesmesi
    {
        string _kullanici=null;
        InstanceContext _instanceContext=null;
        DuplexChannelFactory<IIRCChannel> _fabrika = null;
        IIRCChannel _katilimci = null;
        IOnlineStatus _onlineDurum = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnKatil.Enabled = true;
            btnAyril.Enabled = false;
            btnMesajiGonder.Enabled = false; 
            txtMesajlar.ScrollBars = ScrollBars.Vertical; 
        }

        #region IChat Members
    
        public void DahilEt(string uyeAdi)
        {
            txtMesajlar.Text += String.Format("*** {0} IRC' ye katıldı. ***", uyeAdi);
            txtMesajlar.Text += Environment.NewLine;
        }

        public void MesajGonder(string uyeAdi, string mesaj)
        {
            txtMesajlar.Text += String.Format("({0}) -> {1}", uyeAdi,mesaj);
            txtMesajlar.Text += Environment.NewLine;
        }

        public void Ayril(string uyeAdi)
        {
            txtMesajlar.Text += String.Format("*** {0} IRC' den ayrıldı. ***", uyeAdi);
            txtMesajlar.Text += Environment.NewLine;
        }

        #endregion

        private void btnKatil_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtKullaniciAdi.Text))
                _kullanici = txtKullaniciAdi.Text;
            else
                _kullanici = "İsimsiz";
        
            _instanceContext = new InstanceContext(this);
            _fabrika = new DuplexChannelFactory<IIRCChannel>(_instanceContext, "ClientEndPoint");
            _katilimci = _fabrika.CreateChannel();
            _onlineDurum = _katilimci.GetProperty<IOnlineStatus>(); 
        
            _onlineDurum.Online += delegate(object snd, EventArgs ea)
                                            {
                                                  txtMesajlar.Text += "*** Hat Açık ***";
                                                  txtMesajlar.Text += Environment.NewLine;
                                                  txtMesajlar.Text += _onlineDurum.ToString();
                                                  txtMesajlar.Text += Environment.NewLine;
                                            };
            _onlineDurum.Offline += delegate(object snd, EventArgs ea)
                                            {
                                                  txtMesajlar.Text += "*** Hat Kapalı ***";
                                                  txtMesajlar.Text += Environment.NewLine;
                                                  txtMesajlar.Text += _onlineDurum.ToString();
                                                  txtMesajlar.Text += Environment.NewLine;
                                            };
            _katilimci.Open();
            _katilimci.DahilEt(_kullanici);
    
            btnKatil.Enabled = false;
            btnAyril.Enabled = true;
            btnMesajiGonder.Enabled = true;
        }

        private void btnAyril_Click(object sender, EventArgs e)
        {
            _katilimci.Ayril(_kullanici);
            _katilimci.Close();
            _fabrika.Close();
        
            btnKatil.Enabled = true;
            btnAyril.Enabled = false;
            btnMesajiGonder.Enabled = false;
        }

        private void btnMesajiGonder_Click(object sender, EventArgs e)
        {
            _katilimci.MesajGonder(_kullanici, txtMesaj.Text);
            txtMesaj.Clear();
        }
    }

    public interface IIRCChannel
        : IIRCSozlesmesi, IClientChannel
    {
    }

    [ServiceContract(CallbackContract = typeof(IIRCSozlesmesi))]
    public interface IIRCSozlesmesi
    {
        [OperationContract(IsOneWay = true)]
        void DahilEt(string uyeAdi);

        [OperationContract(IsOneWay = true)]
        void MesajGonder(string uyeAdi, string mesaj);
        
        [OperationContract(IsOneWay = true)]
        void Ayril(string uyeAdi);
    }
}
```

IIRCSozlemesi isimli arayüz (Interface), servis sözleşmesini tanımlamaktadır. Bu sözleşme içerisinde yer alan DahilEt, MesajGonder ve Ayril isimli operasyonlar asenkron (asynchronous) olarak çalışmaktadır ve tek yönlüdür (OneWay). Bu sebepten IsOneWay özelliklerine true değeri atanmıştır. Diğer taraftan CallbackContract olarak IIRCSozlesmesi arayüzünün kendisi bildirilmiştir. Bilindiği gibi çift yönlü iletişim (Duplex Communication) kurulacağından bir geri bildirim sözleşmesinin (Callback Contract) tanımlanması gerekmektedir. Böylece istemciler birbirleri üzerinden bağımsız olarak operasyon çağrıları gerçekleştirebilir ki, P2P haberleşmede tarafların birbirleri üzerinde operasyon geri bildirimlerinde bulunması gereklidir. Bu sayede bir katılımcının göndereceği bir mesaj Mesh ağa dahil olan diğer kullanıcılarada iletilebilir.

Örnekte yer alan form uygulamasının kendisine ait nesne örneği, Mesh ağın bir katılımcısı olarak rol oynamaktadır. Bu sebepten InstanceContext örneği oluşturulurken parametre olarak this verilmiştir. (Burada this doğal olarak çalışma zamanında Form1 nesne örneğini işaret etmektedir.) Sonrasında ise bir DuplexChannelFactory nesne örneği, InstanceContext nesne örneğinin kendisi ve konfigurasyon dosyasında belirtilen EndPoint değeri için örneklenmektedir. DuplexChannelFactory generic sınıfı ile, çift yönlü iletişimi ele alacak olan bir referansın üretilmesi sağlanmaktadır. Bu katılımcıların her biri çift yönlü iletişimi sağlayabilecek şekilde birer kanalı, Mesh ağına doğru açmaktadır. Bu kanalın oluşturulması sırasındada devreye CreateChannel metodu girmektedir. Mesh ağına dahil olan kullanıcı referansından yararlanılarak online ve offline olma durumlarına ait olaylar da ele alınabilir. Bu amaçla IOnlineStatus arayüzünün çalışma zamanında taşıdığı referanstan yararlanılmaktadır. Örnekte önemli olan noktalardan biriside konfigurasyon içerisinde yer alan istemci taraflı EndPoint bildirimidir. Söz konusu uygulama için bu bildirim aşağıdaki gibidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <client>
            <endpoint address="net.p2p://BizimIRC" binding="netPeerTcpBinding" bindingConfiguration="Varsayilan" contract="ChatApp.IIRCSozlesmesi" name="ClientEndPoint" />
        </client>
        <bindings>
            <netPeerTcpBinding>
                <binding name="Varsayilan" port="0">
                    <security mode="None" />
                    <resolver mode="Custom">
                        <custom address = "net.tcp://localhost:4500/IRCResolver" binding="netTcpBinding" bindingConfiguration="ResolverBindingConfig" />
                    </resolver>
                </binding>
            </netPeerTcpBinding>
            <netTcpBinding>
                <binding name="ResolverBindingConfig">
                    <security mode="None"/>
                </binding>
            </netTcpBinding>
        </bindings>
    </system.serviceModel>
</configuration>
```

Konfigurasyon dosyasında dikkat edilmesi gereken en önemli noktalardan birisi netPeerTcpBinding elementi içerisinde yer alan resolver boğumudur. Bu boğum içerisinde mode niteliğine (attribute) verilen Custom değeri ile özel bir peer çözücü servisinin kullanılacağı ifade edilmektedir. Diğer tarafan söz konusu özel peer çözücü servisin hangi adres üzerinden host ediliği, hangi bağlayıcı tipin (Binding Type) kullanıldığı bilgileri ise custom isimli elementin address ve binding niteliklerinde belirtilmektedir. Dikkat edilecek olursa address niteliğinin değeri servis uygulamasının host ettiği adrestir. client elementi içerisinde tanımlanan endpoint boğumunda ise address kısmında net.p2p://BizimIRC adresi yer almaktadır. Bu adres Mesh ağının adresi olarakta düşünülebilir. Bir başka deyişle katılımcıların dahil olacağı Mesh ağının yolu tayin edilmektedir. Ayrıca bağlayıcı tip olarak netPeerTcpBinding kullanılmaktadır. Geliştirilen örnek güvenlik ile ilişkili olaraktan ek bir özellik içermemektedir. Bu nedenle security elementlerinde yer alan mode değerleri None olarak bildirilmiştir. netPeerTcpBinding elementinde yer alan port niteliğinin değeri 0 olduğu için, katılımcı uygulamalar çalıştıkları makinede boş buldukları bir port üzerinden kanal oluşturacaklar ve Mesh ağına açacaklardır. (.Net Remoting mimarisinde geliştirilen örneklerdede port değerine 0 verilmesi ile aynı anlamda olduğuna dikkat edelim.) Artık uygulamalar test edilebilir. Aşağıdaki Flash animasyonunda senaryoya ait sonuçlar irdelenmektedir. (Flash Video boyutu 560 Kb olup yüklenmesi sabır isteyebilir.)

Görüldüğü gibi B isimli katılımcı Mesh ağına dahil olduktan sonra sistem online moda geçmiştir. Bununla birlikte Mesh ağına giren her kullanıcı bilgisi diğer bağlı PeerNode'larada anında iletilir. Böylece katılımcılar ağa dahil olan kullanıcıları görebilmektedir. Katılımcıların gönderikleri mesajlar diğer tüm katılımcıların ekranında anında görülebilmektedir. Diğer taraftan ağdan ayrılan katılımcıların bilgileride diğerlerine iletilmektedir. Burada ilgi çekici noktalardan biriside her katılımcı için ayrı Peer Node ID değerlerinin üretilmesidir. Sistemin offline moda geçmesi ise sadece bir katılımcı kalması durumunda meydana gelmektedir.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde WCF mimarisi üzerinde P2P (Peer-to-Peer) programlamaya giriş yapmaya çalıştık ve örnek olarak basit bir IRC senaryosu geliştirdik. Senaryoda, var olan PNRP (Peer Name Resolution Protocol) servisi yerine.Net Framework ile birlikte gelen CustomPeerResolverService sınıfını kullandık. İlerleyen makalelerimizde P2P tabanlı senaryolarda, geliştirici tarafından yazılmış özel bir peer çözümleyici servisin nasıl ele alınabileceğini ve güvenliği nasıl sağlayabileceğimizi incelemeye çalışacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/PeerToPeerProgramming.rar)