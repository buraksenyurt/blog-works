---
layout: post
title: "Adım Adım State Machine Workflow Geliştirmek"
date: 2008-01-15 06:00:00 +0300
categories:
  - wf
tags:
  - wf
  - csharp
  - xml
  - dotnet
  - linq
  - wcf
  - workflow-foundation
  - wpf
  - xaml
  - http
  - transactions
  - delegates
  - generics
  - visual-studio
---
Öyle iş akışları vardırki, süreç (Process) içerisinde yer alan adımlar arasındaki geçişler herhangibir zamanda ve herhangibir olayın meydana gelmesi sonrasında mümkün olur. Çoğunlukla terminolojide Sonlu Durum Makinesi (Finite State Machine) olarak geçen bu yaklaşıma göre, herhangibir nesnel varlığın zaman içerisinde sahip olabileceği durumlar işaret edilmektedir. Çok doğal olarak bu durum, programatik ortamda yer alan iş problemlerinin çözümündede göz önüne alınmaktadır. İşte bu makalemizde Sonlu Durum Makinesi (Finite State Machine) kavramını irdelemeye ve Windows Workflow Foundation içerisindeki kullanımını araştırmaya çalışacağız. Başlamadan önce Sonlu Durum Makinesi (Finite State Machine) kavramını anlamaya çalışamakta yarar vardır.

Öncelikli olarak sonlu kelimesinin kullanılmasının sebebi söz konusu nesnel varlığın sahip olabileceği durumların (State) sayılı olmasıdır. Bir başka deyişle bu yaklaşıma göre bir makinenin sahip olabileceği durumların sayısı bellidir. Diğer taraftan makinenin zaman içerisinde sahip olabileceği haller onun durumlarını (States) ifade etmektedir. Makine doğal olarak bu durumlara sahip olan nesnel yapıyı temsil etmektedir. Sonlu durum makinelerinde (Finite State Machine) durumlar arasındaki geçişler bir aksiyon sonucu gerçekleşir.

Tahmin edileceği üzere söz konusu aksiyonlar çoğunlukla bir olaydır ve genellikle insan etkileşimi sonrası gerçekleşmektedir. Ne varki burada insan etkileşimi sonucu olay tetiklenmesi zorunluluk değildir. İnsan etkileşimi olması nedeniyle, olaylar herhangibir zaman dilimi içerisinde meydana gelebilir. Bu tip vakalara mühendislik eğitimlerinde verilmekte olan iki basit örnek vardır. Hepimizin yakından tanıdığı, metro istasyonlarında, duraklarda, okullarda veya bazı kafelerde gördüğümüz otomat makineleri ile herkese açık olan çamaşırhanelerde yer alan ve jeton ile çalışan yıkama makineleri bu iki örneği oluşturmaktadır.

> Finite State Machine'lerde insan ile olan etkileşim ön planda olup, durumlar (States) arası geçişler çoğunlukla insanlar tarafından tetiklenen olaylara (Events) bağlıdır.

Otomat makinesinin kendisi göz önüne alındığında zaman içerisinde sahip olabileceği bazı durumlar (State) vardır. Örneğin makine çalışmıyordur, çalışıyordur, para veya jeton bekliyordur, seçilen ürünü veriyordur yada tadilattadır. Burada bahsedilen hallerin tamamı birer durum (State) olarak irdelenir. Bu durumlar arasındaki geçişler için çoğunlukla makinenin bazı araçları insanlar tarafından kullanılır. Dikkat edilecek olursa makine zaman içerisinde herhangibir anda herhangibir durumuna geçebilir. Tabiki bazı durumlara geçişler sırasında bazı koşulların sağlanması gerekebilir. Benzer senaryo yıkama makinesi örneği içinde geçerlidir. (İlerlemeden önce para veya jeton ile çalışan bir çamaşır makinesinin zaman içinde sahip olabileceği durumları ve bu durumlar arasındaki geçişler için gereken olayların neler olabileceğini kağıt üzerinde tasarlamaya çalışmanızı öneririm.)

Finite State Machine'ler otomat, çamaşır makinesi gibi gerçek hayat örnekleri dışında üretim hattında yer alan sanayi makinelerinin otomasyon süreçlerinde, oyun programlamada yer alan karakterlerin zaman içerisindeki hareket dağılımlarında, transaction içerisinde çalışan bir para aktarma veya kredilendirme sürecinde ve benzer senaryolarda göz önüne alınabilir. Modelin böylesine popüler olması, özellikler programatik ortamlarda kolay bir şekile anlaşılabilmelerini sağlamak amacıyla UML formasyonunda da ifade edilmesini gerektirmiştir. Söz gelimi aşağıdaki ekran görüntüsünde otomat makinesinin Finite State Machine diagramı yer almaktadır.

![mk239_1.gif](/assets/images/2008/mk239_1.gif)

Normal şartlarda bu diagramlarda ekranda görülen kırmızı baloncuklar elbetteki yer almaz. Herşeyden önce Finite State Machine içerisinde yer alan makinenin mutlaka bir başlangıç durumu (Initial State) ve son durumu (Finalization State veya Terminal State olarak adlandırılır- iç içe iki yuvarlağın olduğu parça ile ifade edilir) vardır. Başlangıç durumunda makinenin ilk konumdaki hali göz önüne alınır. Son durumda ise makinenin var olan durumlar sona erdikten sonraki hali ele alınmaktadır. Şekilde başlangıç durumundan, Para Bekleniyor isimli duruma geçiş yapılabilmektedir. Bu geçiş için gereken, makineyi kullanan kişinin para atmasıdır. Para Bekleniyor durumuda, para yeterli oluncaya kadar kendisine geçiş yapılmasına sağlayacak şekilde bir olaya sahiptir. Para yeterli olduktan sonra ise Ürün Seçimi Bekleniyor isimli duruma geçiş yapılabilir. Burada ise kişi ürünü seçmekte ve sonrasında makine seçilen ürünü hazırlayarak Bitiş durumuna girmektedir. Bu senaryoda pek çok durum göz ardı edilmiştir. Örneğin seçim yapıldıktan sonra makinenin arıza yapıp ürünü vermemesi halinde girilecek durum veya para üstü verme durumları gibi.

Hemen ikinci bir örnek ile devam edelim. Söz gelimi bir Windows uygulaması üzerinden kontrol edilen uzaktan kumandalı bir otomobilin sahip olabileceği durumlara ait bir Finite State Machine diagramı söz konusu olabilir. Bu diagram aşağıdakine benzer bir şekilde ele alınabilir.

![mk239_2.gif](/assets/images/2008/mk239_2.gif)

Burada otomobilin kendisi programatik ortamda bir nesne ile ifade edilebilir. Durumlar (States) arasındaki geçişleri sağlayan ise Windows uygulamasından tetiklenen nesne olayları (Events) olabilir. Arabanın şekle göre sahip olabileceği durumlar belirlidir. Bu durumlarda söz konusu olabilecek olaylarda aynı şekilde belirli ve sayılıdır. Bir durumdan başka bir durumua geçiş veya geçişlerde birden fazla sayıda olay söz konusu olabilir. Söz gelimi Motor Çalışıyor durumundayken, OnIlerle, OnMotoruDurdur veya OnGeriGit gibi olaylar tetiklenerek üç farklı duruma geçiş yapılması sağlanabilir. Buda bize durumlar arasındaki geçişlerde birden fazla olayın söz konusu olabileceğini göstermektedir. Hatta bazı noktalarda ortak olaylarda söz konusudur. Örneğin araba ileri veya geri giderken OnDur olayı tetiklenerek durması sağlanabilir. OnDur olay hem Geri Gidiyor hemde Ilerliyor durumları (States) için geçerli ortak bir olaydır.

Sanıyorumki buraya kadar anlatılanlar sayesinde Finite State Machine yaklaşımı hakkında biraz fikir sahibi olunmuştur. Bundan sonraki kısımlarımızda ise.Net Framework 3.0 ile gelen Windows Workflow Foundation açısından Finite State Machine modeline bakıyor olacağız. Nitekim söz konusu model gerçek hayat örneklerine benzer olacak şekilde programatik ortamdaki nesnel yapılar içinde söz konusu olabilmektedir. Bu noktada WWF bize kolaylaştırıcı bir yaklaşım sunmakta ve State Machine tarzı iş süreçlerinin.Net uygulamalarında rahatça ele alınabilmelerine olanak tanımaktadır.

Windows Workflow Foundation (WWF) iki temel iş akışı modelini ele alır. Sequential Workflows ve State Machine Workflows. Daha öncedende bahsedildiği gibi Sequential Workflows tipinden olan iş akışlarında adımlar yada aktiviteler arasındaki geçişlerin nasıl ve ne zaman olacağı bellidir. Hatta bu geçişler sırasında koşulların kullanılması çok sık rastlanan bir durumdur. Stata Machine Workflow tipindeki iş akışlarında ise daha öncedende değinildiği gibi adımlar veya durumlar arasındaki geçişler dış olayların tetiklenmesine bağlıdır. State Machine Workflow akışlarından, aktivitenin kendisi StateMachineWorkflowActivity sınıfından örneklenmektedir. (Workflow mimarisindeki herşeyin birer aktivite (Activity) tipi olduğunu hatırlayalım) StateMachineWorkflowActivity tipinin.Net içerisindeki yerine bakıldığında aşağıdaki sınıf diagramında (Class Diagram) yer alan hiyerarşide olduğu görülür.

![mk239_6.gif](/assets/images/2008/mk239_6.gif)

Dikkat edileceği üzere StateMachineWorkflowActivite sınıfıda eninde sonunda bir Activity tipidir. Kendi içerisinde tanımlanmış olan üyelerden bir kaçını açıklayarak devam edelim. CompletedStateName özelliği ile, bitiş durumu (Finalization State) ifade edilir. Bu özellik önemlidir nitekim Workflow'un hangi durumdan sonra sonlanacağını belirtmektedir. Ancak yazılmadığı vakalarda vardır. Benzer şekilde InitialStateName özelliği başlangıçtaki durum aktivitesini işaret etmektedir. Çalışma zamanında istenirse o anda makinenin bulunduğu durum adı CurrentStateName özelliği ile elde edilebilir. Benzer şekilde PreviousStateName özelliği ile o anda bulunulan durumdan bir önceki durum adı elde edilebilir ki bu hangi durumdan gelindiğini öğrenmek için kullanılabilir. Buradaki örnek özellikler (Properties) dışında üst sınıflardan gelen pek çok üye (Member) yer almaktadır. Amacımız şu an için bu tipin tüm üyelerini öğrenmek değildir. Bunun yanında bir StateMachineWorkflowActivity tasarlanırken içerisinde çoğunlukla aşağıdaki şekilde yer alan tipler kullanılır.

![mk239_3.gif](/assets/images/2008/mk239_3.gif)

Burada belkide en kritik ve değerli tip StateActivity sınıfıdır. StateActivity, aslında makinenin içerisinde bulunacağı durumları işaret etmektedir. Çok doğal olarak bir StateActivity içerisinde StateInitializationActivity yada StateFinalizationActivity tanımlanabilir. Ancak bir StateActivity içerisinde bunlardan sadece bir tane bulunabilir. Öte yandan StateActivity içerisinde, StateInitializationActivity veya StateFinalizationActivity tiplerinin tanımlanması zorunlu değildir. Bunlar opsiyonel olarak ele alınmaktadır.

Durumlar (States) arasındaki geçişler için EventDrivenActivity tipi kullanılmaktadır. StateActivity içerisinde birden fazla EventDrivenActivity nesnesi tanımlanabilir. Nitekim daha öncedende bahsettiğimiz gibi, bir durumda (State) söz konu olabilecek birden fazla olay (Event) olabilir. Her EventDrivenActivity mutlaka olayları alabilecek bir aktivite tipi içerir. Bunu sağlayan HandleExternalEventActivity tipidir. Söz konusu aktivite tipini takiben herhangibir başka aktivitede gelebilir. Örneğin bir olayın tetiklenmesinin ardından host uygulama üzerinden çağırılabilecek harici metodların ele alınması, kod işletilmesi gibi işlemler yapılabilmektedir. Şekilde dikkat edileceği üzere EventDrivenActivity içerisinde son olarak SetStateActivity kullanılmaktadır. Bu tip sayesinde bulunulan durumdan diğer bir duruma geçilmesi sağlanmaktadır.

Şekilde dikkat edilmesi gereken noktalardan biriside StateActivity tipleri dışında ve StateMachineWorkflowActivity içerisinde kalan alanda EventDrivenActivity bileşenlerinin tanımlanabilmesidir. Bazı hallerde durumlar (States) arasındaki geçişlerde kullanılmayan olaylar (Events) söz konusu olabilir. Söz gelimi otomobilin selektör yapması herhangibir anda herhangibir duruma geçiş yapılmasını gerektirmeyecek bir vaka olarak ele alınır. Bu sebepten bu vakaya ilişkin olayı ele alacak EventDrivenActivity nesnesinin bir StateActivity içerisinde tanımlanmasına da gerek yoktur.

Bazı durumlarda StateActivity bileşenleri kendi içlerindede birden fazla StateActivity içerebilir. Bu genellikle içerideki aktivitelerin aynı olayları ele aldığı durumlarda söz konusudur. Bu tip aktiviteler Recursive Compositon Activities olarakda adlandırılmaktadır. Aşağıda şekilde bu durum ele alınmaya çalışılmaktadır.

![mk239_4.gif](/assets/images/2008/mk239_4.gif)

Burada her iki StateActivity tipi içerisinde yer alan HandleExternalEventActivity nesneleri aynı olay ile ilgilenmektedir. Böyle bir durumda söz konusu StateActivity aşağıdaki şekilde görüldüğü gibide tasarlanabilir. (Otomobil örneği göz önüne alındığında Geri gitme veya ileri gitme durumları içerisinden Durma durumuna geçilmesi için aynı olaylar ele alınmaktadır.)

![mk239_5.gif](/assets/images/2008/mk239_5.gif)

Görüldüğü gibi StateActivity1 ve StateActivity2 nesne örnekleri genel bir StateActivity nesnesi içerisine alınmıştır. Bununla birlikte her ikisinin EventDrivenActivity nesneleri dışarı alınarak tek bir noktada toplanmıştır. Nitekim her iki alt aktivitede aynı EventDrivenActivity nesnelerini ele almaktadır.

Bu kadar teorik bilgiden sonra bir örnek yaparak devam etmekte yarar vardır. Makale yazılmadan önce yapılmış olan araştırmalarda Microsoft'un Otomat makinesi örneğini kullandığı, APress'in ise bir arabanın durumlarını ele aldığı gözlenmiştir. Bizde senaryo olarak APress tarafından ele alınan Otomobil örneğini kendimize göre adım adım geliştirmeye ve anlamaya çalışacağız. İşe ilk olarak yeni bir State Machine Workflow Library projesi açarak başlayalım. Bunun için Visual Studio 2008 ortamında New Project->WF sekmesinden ilgili proje şablonunu (Project Template) seçmemiz yeterlidir.

![mk239_7.gif](/assets/images/2008/mk239_7.gif)

Proje oluşturulduğunda otomatik olarak Workflow1.cs dosyası tasarım penceresinde açılacak ve aşağıdaki ekran görüntüsü oluşacaktır.

![mk239_8.gif](/assets/images/2008/mk239_8.gif)

Burada görüldüğü gibi Workflow1.cs içerisinde varsayılan olarak bir InitialState bileşeni bulunmaktadır. Şimdi örneğin temasını oluşturan arabanın zaman içerisindeki durumları göz önüne alınabilir. Bu durumları listeledikten sonra ise gerekli tiplerin hazırlanmasına başlanabilir. Herşeyden önce arabanın zaman içerisindeki durumları arasındaki geçişleri sağlayacak olan olayların veri değişimi sağlayacak şekilde tasarlanmış bir arayüz (Interface) içerisinde yer alması sağlanmalıdır. Bu amaçla projeye aşağıda sınıf diagramı (Class Diagram) ve kod çıktısı yer alan arayüz (Interface) eklenir.

![mk239_9.gif](/assets/images/2008/mk239_9.gif)

```csharp
[ExternalDataExchange]
public interface IArabaHizmetleri
{
    event EventHandler<ExternalDataEventArgs> ArabayiCalistir;
    event EventHandler<ExternalDataEventArgs> MotoruDurdur;
    event EventHandler<ExternalDataEventArgs> Dur;
    event EventHandler<ExternalDataEventArgs> Ilerle;
    event EventHandler<ExternalDataEventArgs> GeriGit;
    event EventHandler<ExternalDataEventArgs> ArabadanCik;
    event EventHandler<ExternalDataEventArgs> SelektorYap;

    void OnMesajGonder(string message);
} 
```

Bu arayüz (Interface) basit olarak iş akışına yerel bir servis (Local Service) üzerinden sunulabilecek üye bildirilmlerini içermektedir ki bunlar çoğunlukla olay ve metod tanımlamalarıdır. Diğer taraftan arayüz tipi ExternalDataExchange niteliği (attribute) ile işaretlenmiştir. Bu niteliğin (Attribute) uygulanması sayesinde arayüz tipi, iş akışları tarafından yerel bir servis (Local Service) olarak kullanılabilir hale gelir. Arayüz (Interface) içerisinde durum geçişleri (State Transitions) için gerekli temel olay tanımlamaları yer almaktadır. Örneğin arabanın ilerlemesi için Ilerle yada geriye gitmesi için GeriGit olaylarına ait bildirimler bulunmaktadır. Bununla birlikte arayüz, OnMesajGonder isimli bir metod bildirimi de içermektedir. Bu metod iş akışı (Workflow) tarafından, host uygulamaya mesaj göndermek amacıyla kullanılacaktır.

> Bilindiği gibi arayüzler (Interface), sadece üye bildirimleri içeren tiplerdir. Polimorfik yapıları vardır ve çoklu kalıtıma (Multi Inheritance) destek verirler. Çoğunlukla türetme (Inheritance) için kullanılır ve türeyen üyelerin mutlaka uyması gereken kuralları bildirirler. Plug-In tabanlı programlamada, tip genişletmelerinde, ortak sözleşmelerin sunulmasında (Söz gelimi WCF gibi SOA-Service Oriented Architecture mimarilerinde) vb... gibi senaryolarda sıklıkla kullanılırlar.

Arayüzün tanımlanmasından sonra bunu uygulayan sınıfın tasarlanması gerekmektedir. Bu sınıf (Class) aynı zamanda yerel bir servis (Local Service) olacaktır. Söz konusu sınıf ve MesajGonder için kullanılan yardımcı olay parametresinin içeriği aşağıdaki gibidir.

MesajAlindiEventArgs sınıfı;

![mk239_10.gif](/assets/images/2008/mk239_10.gif)

```csharp
[Serializable]
public class MesajAlindiEventArgs : ExternalDataEventArgs
{
    private string _bilgi;

    public string Bilgi
    {
        get { return _bilgi; }
        set { _bilgi = value; }
    }
    public MesajAlindiEventArgs(Guid ornekId, string bilgi)
            : base(ornekId)
    {
        _bilgi = bilgi;
    }
} 
```

ExternalDataEventArgs sınıfının tüm yapıcı metod (Constructor Method) versiyonları Guid tipinden bir ilk parametre alırlar. Bu sebepten base anahtar kelimesi kullanılarak MesajAlindiEventArgs sınıfına gelen Guid değerinin üst sınıf örneğine gönderilmesi sağlanmaktadır.

ArabaYerelServisi sınıfı;

![mk239_11.gif](/assets/images/2008/mk239_11.gif)

```csharp
public class ArabaYerelServisi :IArabaHizmetleri
{
    #region IArabaHizmetleri Members

    public event EventHandler<ExternalDataEventArgs> ArabayiCalistir;
    public event EventHandler<ExternalDataEventArgs> MotoruDurdur;
    public event EventHandler<ExternalDataEventArgs> Dur;
    public event EventHandler<ExternalDataEventArgs> Ilerle;
    public event EventHandler<ExternalDataEventArgs> GeriGit;
    public event EventHandler<ExternalDataEventArgs> ArabadanCik;
    public event EventHandler<ExternalDataEventArgs> SelektorYap;

    public void OnMesajGonder(string message)
    {
        if (MesajAlindi != null)
        {
            MesajAlindiEventArgs args = new MesajAlindiEventArgs(WorkflowEnvironment.WorkflowInstanceId, message);
            MesajAlindi(this, args);
        }
    }

    #endregion
    
    #region Host uygulama tarafından kullanılan üyeler
    
    public event EventHandler<MesajAlindiEventArgs> MesajAlindi;

    public void OnArabayiCalistir(ExternalDataEventArgs args)
    {
        if (ArabayiCalistir != null)
            ArabayiCalistir(null, args);
    }

    public void OnMotoruDurdur(ExternalDataEventArgs args)
    {
        if (MotoruDurdur != null)
            MotoruDurdur(null, args);
    }

    public void OnDur(ExternalDataEventArgs args)
    {
        if (Dur != null)
            Dur(null, args);
    }

    public void OnIlerle(ExternalDataEventArgs args)
    {
        if (Ilerle != null)
            Ilerle(null, args);
    }

    public void OnGeriGit(ExternalDataEventArgs args)
    {
        if (GeriGit != null)
            GeriGit(null, args);
    }

    public void OnSelektorYap(ExternalDataEventArgs args)
    {
        if (SelektorYap != null)
            SelektorYap(null, args);
    }

    public void OnArabadanCik(ExternalDataEventArgs args)
    {
        if (ArabadanCik != null)
            ArabadanCik(null, args);
    }

    #endregion
}
```

ArabaYerelSinifi isimli sınıf (Class), ilgili arayüzü (Interface) uygulamak dışında Host uygulama tarafından tetiklenebilecek metodlarda içermektedir. Bu metodlar kendi içlerindende akışa ait durum geçişleri için gerekli olayların tetiklenmesinde kullanılmaktadır. Sınıf içerisinde yer alan OnMesajGonder isimli metod parametre olarak string tipinden bir değişken almaktadır. Bu parametre değeri iş akışından (Workflow) gelmekte olup Host uygulamaya iletilmektedir. Bu iletim sırasında MesajAlindi isimli olay devreye girmektedir ki bu olay sadece iş akışını barındıran Host uygulama tarafından ele alınabilir. Tahmin edileceği üzere OnMesajGonder metodunun parametre değeri iş akışı tasarlanırken belirlenecektir. MesajAlindi olayı tetiklenirken parametre olarak ExternalDataEventArgs sınıfından türemiş olan MesajAlindiEventArgs sınıfı kullanılmaktadır.

Bu işlemlerin tamamlanmasının ardından Durum Makinesinin (State Machine) tasarlanmasına başlanabilir. Bu amaçla Workflow1.cs üzerinden gerekli düzenlemelerin yapılması gerekmektedir. İlk olarak arabanın sahip olabileceği tüm durumlar StateActivity bileşenleri yardımıyla iş akışı üzerine alınırlar. Başlangıçta StateActivity bileşenlerinin Name özelliklerinin değerlerinin aşağıdaki tabloda yer aldığı gibi değiştirildiğini düşünelim.

StateActivity Bileşeni
Name Özelliği Değeri
Kısa Bilgi

MotorCalismiyor
Arabanın motorunun çalışmadığı durumu işaret eder.

MotorCalisiyor
Arabanın motorunun çalışmak olduğu durumu işaret eder.

ArabaIlerliyor
Arabanın ileri doğru hareket ettiği durumu işaret eder.

ArabaGeriGidiyor
Arabanın geriye doğru hareket ettiği durumu işaret eder.

ArabadanCikilmistir
Arabadan inildikten sonraki durumu işaret eder.

Bunun sonrasında iş akışına (Workflow) ait ekran görüntüsü aşağıdaki gibi olacaktır.

![mk239_12.gif](/assets/images/2008/mk239_12.gif)

Son işlemleri takiben iş akışının başlangıç ve bitiş durumları belirlenebilir. Bunun için Workflow1' in özellikler (Properties) penceresinden InitialStateName ve CompletedStateName özelliklerine ilgili değerlerin verilmesi gerekmektedir. Senaryo gereği MotorCalismiyor başlangıç ve ArabadanCikilmistir bitiş durumlarını (State) bildirmektedir.

![mk239_13.gif](/assets/images/2008/mk239_13.gif)

Artık ilk durumdan diğerine geçisi sağlayacak olan olay aktivitesi tanımlanabilir. Bu amaçla MotorCalismiyor isimli StateActivity içerisine bir adet EventDrivenActivity bileşeni sürüklenir. EventDrivenActivity bileşeninin Name özelliğine MotorCalistirOlayi adı verilebilir. Sonuç olarak ekran görüntüsü aşağıdaki gibi olacaktır.

![mk239_14.gif](/assets/images/2008/mk239_14.gif)

Daha öncedende bahsettiğimiz gibi EventDrivenActivity içerisinde genel olarak 3 farklı aktivite kullanılır. Bu senaryoda söz konusu olayın tetiklenmesi için HandleExternalEventActivity bileşeni ele alınmalıdır. Diğer taraftan yerel servis üzerinden harici metod çağrısı için CallExternalMethodActivity bileşeni değerlendirilir. Son olarak olayın tetiklenmesi sonrası geçilecek olan durumu işaret etmek için SetStateActivity bileşeni kullanılmalıdır. EventDrivenActivity bileşeni içerisine bahsedilen kontrolleri oluşturmak için MotorCalistirOlayi üzerinde çift tıklanması yeterlidir. Sonuç olarak ilk durum için tasarlanan EventDrivenActivity bileşeninin içeriği aşağıdaki ekran görüntüsünde yer aldığı gibi olacaktır.

![mk239_15.gif](/assets/images/2008/mk239_15.gif)

İlk olarak HandleExternalActivity bileşeni ile başlayalım. Bu bileşenin InterfaceType özelliğine tetiklenecek olan olayın bildirimini içeren arayüz adı verilmelidir. Bu amaçla üç nokta düğmesine basıldığında aşağıdakine benzer bir arabirim ile karşılaşılır. Bu arabirimden aynı proje içerisindeki veya farklı bir projedeki ExternalDataExchange niteliğini uygulayan arayüzler görülebilir. Örneğimizdede IArabaHizmetleri arayüzü aktif olarak gelmektedir.

![mk239_16.gif](/assets/images/2008/mk239_16.gif)

Arayüz seçimi yapıldıktan sonra EventName özelliğinde ele alınabilecek olan, bir başka deyişle interface tipi içerisinde bildirilmiş olan olayların listesi gelecektir. Örneğimizdeki ilk durum için ArabayiCalistir olayı seçilmelidir. EventDrivenActivity için söz konusu olan durum aşağıdaki gibidir.

![mk239_17.gif](/assets/images/2008/mk239_17.gif)

Gelelim CallExternalMethodActivity bileşenine. Bu bileşen içinde yine arayüz (Interface) seçimi yapılmalıdır. InterfaceType özelliğine yapılan atamanın ardından çalıştırılacak olan harici metodun adı MethodName özelliğinden seçilir. Örnekte harici metodun aldığı string bir parametrede söz konusudur. Bu parametrede message isimli özelliğe atanan değer ile belirtilir.

![mk239_18.gif](/assets/images/2008/mk239_18.gif)

Burada dikkat edilmesi gereken noktalardan biriside, OnMesajGonder metodunun parametrik yapısına uygun olacak şekilde bir özelliğin IDE'deki Properties penceresine eklenmiş olmasıdır. Örnekte message isimli olan parametre, özellik penceresine birer bir aynı olacak şekilde gelmiştir. Harici metod çağırılmasınıda tamamladıktan sonra geçilecek olan durum bileşenini belirlemek gerekmektedir. Bunun içinde SetStateActivity bileşeninin TargetStateName özelliğine StateActivity adının atanması yeterlidir. İlk durumda aracın motoru çalıştırıldıktan sonra MotorCalisiyor durumuna (State) geçilmektedir.

![mk239_19.gif](/assets/images/2008/mk239_19.gif)

İstenirse ilk durum için StateInitializationActivity bileşeni de eklenebilir. Böylece makinenin ilk konumdaki durumu için gerekli hazırlıkların yapılması sağlanabilir. Söz gelimi örnek senaryoda OnMesajGonder metodunun harici olarak çağırılması ve mesaj olarakta "Araba hazır" denilmesi sağlanabilir. Bunun için MotorCalismiyor aktivitesi içerisine bir adet StateInitializationActivity bileşeni atanması ve bu bileşenin içerisinede bir adet CallExternalMethodActivity bileşeni eklenerek InterfaceType, MethodName ve message özelliklerinin değerlerinin belirlenmesi yeterlidir.

![mk239_20.gif](/assets/images/2008/mk239_20.gif)

Böylece ilk durum tamamıyle hazırdır. Akışın şu andaki görüntüsü aşağıdaki gibi olacaktır.

![mk239_21.gif](/assets/images/2008/mk239_21.gif)

Burada yapmış olduğumuz adımların aynılarını diğer durumlar içinde gerçekleştirmeliyiz. Makalemizin dahada uzamaması için buradaki adımların gösterilmesini atlıyoruz. Gereken ayarlamalar yapıldıktan sonra iş akışının son hali aşağıdaki ekran görüntüsündeki gibi olmalıdır. Dikkat edileceği üzere olası durum geçişleri ince ok çizgiler ile daha belirgin haldedir.

![mk239_22.gif](/assets/images/2008/mk239_22.gif)

Burada ekstradan selektör yapma durumununda, State Machine üzerinde ayrı bir EventDrivenActivity olarak tanımlanması gereklidir. Bu aktivite içerisinde sadece HandleExternalEventActivity ve CallExternalMethodActivity bileşenlerinin kullanılması yeterlidir. Dikkat edileceği üzere SetStateActivity kontrolünün kullanılması gerekli değildir. Nitekim selektör yapma olayının arkasından geçilecek herhangibir durum göz önüne alınmamaktadır. Bu nedenle SelektorYapOlayı isimli EventDrivenActivity içerisinde aşağıdaki bileşenlerin tasarlanması yeterlidir.

![mk239_23.gif](/assets/images/2008/mk239_23.gif)

Host uygulamaya geçmeden önce şu durumda göz önüne alınmalıdır. Tasarım penceresine bakıldığında ArabaGeriGidiyor ve ArabaIlerliyor StateActivity bileşenleri içerisinde aynı EventDrivenActivity nesnelerinin kullanıldığı görülmektedir ki buda aracı durdurma olayını işaret etmektedir. Bu nedenle makalemizin başındada belirttiğimiz gibi bu StateActivity bileşenlerinin ortak bir StateActivity içerisine alınması düşünülebilir. Bu sebepten tasarım ekranına ortak bir StateActivity bileşeni sürüklenip, diğerlerini içine alması sağlanmalıdır. Aşağıdaki ekran görüntüsü bu durumu açık bir şekilde ifade etmektedir.

![mk239_24.gif](/assets/images/2008/mk239_24.gif)

Öncelikli olarak HareketEdiyor isimli StateActivity içerisine ArabaGeriGidiyor ve ArabaIlerliyor isimli StateActivity nesneleri sürüklenmiştir. Sonrasında ise bunlardan herhangibirisinde yer alan ArabayiDurdurOlayi isimli EventDrivenActivity bileşeni HareketEdiyor isimli StateActivity içerisine çıkartılmış ve diğerininki silinmiştir.

Artık host uygulamanın yazılmasına başlanabilir. Örneğimizde host uygulaması basit bir WPF (Windows Presentation Foundation) programı olarak tasarlanacaktır. Söz konusu WPF uygulamasının State Machine Workflow kütüphanesi (Library) dışında, System.Workflow.Activities, System.Workflow.Components ve System.Workflow.Hosting assembly'larınıda referans etmesi gerekmektedir. YarisPisti isimli WPF uygulamamızın Window1 penceresine ait ekran görüntüsü ve XAML (eXtensible Application Markup Language) içeriği ise aşağıdaki gibidir.

![mk239_25.gif](/assets/images/2008/mk239_25.gif)

XAML içeriği;

```xml
<Window x:Class="YarisPisti.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Window1" Height="300" Width="300">
    <Grid>
        <Label Height="42" Margin="28,0,22,32" Name="lblGelenMesaj" VerticalAlignment="Bottom" Content="Durum Bilgisi"></Label>
        <Button Height="23" HorizontalAlignment="Left" Margin="26,25,0,0" Name="btnYeniAraba" VerticalAlignment="Top" Width="88" Click="btnYeniAraba_Click">Yeni Araba</Button>
        <Button Height="23" Margin="26,60,0,0" Name="btnMotoruCalistir" VerticalAlignment="Top" Click="btnMotoruCalistir_Click" HorizontalAlignment="Left" Width="88">Çalıştır</Button>
        <Button Height="23" HorizontalAlignment="Left" Margin="28,97,0,0" Name="btnMotoruKapat" VerticalAlignment="Top" Width="86" Click="btnMotoruKapat_Click"> Motoru Kapat</Button>
        <Button HorizontalAlignment="Right" Margin="0,129,44,110" Name="btnSelektorYap" Width="84" Click="btnSelektorYap_Click">Selektör</Button>
        <Button Height="23" HorizontalAlignment="Right" Margin="0,26,44,0" Name="btnIlerle" VerticalAlignment="Top" Width="83" Click="btnIlerle_Click">İlerle</Button>
        <Button Height="23" HorizontalAlignment="Right" Margin="0,58,44,0" Name="btnGeriGit" VerticalAlignment="Top" Width="83" Click="btnGeriGit_Click">Geri Git</Button>
        <Button Height="23" HorizontalAlignment="Right" Margin="0,95,44,0" Name="btnDur" VerticalAlignment="Top" Width="83" Click="btnDur_Click">Dur</Button>
        <Button HorizontalAlignment="Left" Margin="28,129,0,110" Name="btnArabadanIn" Width="86" Click="btnArabadanIn_Click">İn</Button>
    </Grid>
</Window>
```

Window1 penceremizin kod içeriği ise aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArabaStateMachineLib;
using System.Workflow.Runtime;
using System.Workflow.Activities;

namespace YarisPisti
{
    public partial class Window1 : Window
    {
        // Workflow çalışma ortamı için gerekli nesne tanımlanır
        WorkflowRuntime _wf;
        ArabaYerelServisi _arabaServisi; // Yerel Servis nesnesi tanımlanır
        Guid ornekId = Guid.Empty; //ExternalDataEventArgs sınıfı parametre olarak Guid almaktadır. Bu sebepten ornekId isimli bir değişlen tanımlanmıştır.

        public Window1()
        {
            InitializeComponent();

            _wf = new WorkflowRuntime(); // Çalışma zamanı oluşturulur
            _wf.StartRuntime(); // WF çalışma zamanı başlatılır
                ExternalDataExchangeService excSrv = new ExternalDataExchangeService(); // Bir adet ExternalDataExchangeService servisi oluşturulur ve şu anki WF çalışma zamanına AddService metodu ile eklenir.
            _wf.AddService(excSrv);

            // Yerel Servis(Local Servis) nesnesi örneklenir.
            _arabaServisi = new ArabaYerelServisi();
            // Host üzerinden ele alınacak MesajAlindi olayı yüklenir.
            _arabaServisi.MesajAlindi+=new EventHandler<MesajAlindiEventArgs>(_arabaServisi_MesajAlindi);
            // Yerel servis ExternalDataExchangeService örneğine eklenir
            excSrv.AddService(_arabaServisi);
        }

        // Label içeriğinin güncellenmesi için aşağıdaki gibi Invoker kullanımı gereklidir. Aksi takdirde çalışma zamanında istisna alınır.
        private delegate void GuncellemeTemsilcisi();
        void _arabaServisi_MesajAlindi(object sender, MesajAlindiEventArgs e)
        {
            ornekId = e.InstanceId;
            GuncellemeTemsilcisi dlg = delegate()
            {
                lblGelenMesaj.Content = e.Bilgi.ToString();
            };
            // Normal Windows uygulamalarında this.Invoke ile çağırabilmemiz mümkünken WPF uygulamalarında Dispatcher nesnesinden yararlanılmaktadır
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, dlg); 
        }

        // Yeni araba aslında Workflow1 tipinden yeni bir State Machine Workflow örneği oluşturulmasını sağlamaktadır.
        private void btnYeniAraba_Click(object sender, RoutedEventArgs e)
        {
            WorkflowInstance wfOrnegi = _wf.CreateWorkflow(typeof(Workflow1), null);
            wfOrnegi.Start(); // State Machine Workflow başlatılır
            ornekId = wfOrnegi.InstanceId; // ExternalDataEventArgs' ta kullanılmak üzere InstanceId değeri alını ve GUID tipinden olan ornekId değişkenine atanır.
        }

        // Olaylar için gerekli argümanların alınması sağlanan metod
        private ExternalDataEventArgs ArgumanAl()
        {
            ExternalDataEventArgs args = new ExternalDataEventArgs(ornekId);
            args.WaitForIdle = true; 
            return args;
        }

        private void btnIlerle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _arabaServisi.OnIlerle(ArgumanAl()); // Servis üzerinden ilgili olay metodu tetiklenir
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMotoruCalistir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _arabaServisi.OnArabayiCalistir(ArgumanAl());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGeriGit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _arabaServisi.OnGeriGit(ArgumanAl());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMotoruKapat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _arabaServisi.OnMotoruDurdur(ArgumanAl());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _arabaServisi.OnDur(ArgumanAl());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnArabadanIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _arabaServisi.OnArabadanCik(ArgumanAl());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSelektorYap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _arabaServisi.OnSelektorYap(ArgumanAl());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki video görüntüsündeki gibi State Machine Workflow başarılı bir şekilde yüklendiği ve çalıştığı görülmektedir. Tabiki geçiş yapılamayan durumlarda söz konusudur. Örneğin araba ileri gidiyorken geri gidiyor durumuna geçilmesi söz konusu değildir. Bu tip hallerde, try...catch blokları devreye girerek üretilen istisna (Exception) mesajları MessageBox içerisinde görülecektir.

Böylece geldik bir uzun makalemizin daha sonuna. Bu makalemizde State Machine Workflow tipinden iş akışlarını kısaca ne olduklarını, nasıl tasarlandıklarını incelemeye çalıştık. Bunu yaparken StateActivity, EventDrivenActivity, HandleExternalEventActivity, SetStateActivity, CallExternalMethodActivity vb aktivite tiplerinden bir kaçına değinme fırsatımızda oldu. Ayrıca durumlar (States) arası geçişleri sağlayan olayların yerel bir servis (Local Service) içerisinde nasıl geliştirilebileceğini gördük. Son olarakta geliştirilen Workflow projesini bir WPF host uygulamasında yürütmeyi inceledik.

Örnek senaryo olarak APress yayınlarından olan ve Bruce Bukovics tarafından yazılan Pro WF kitabında yer alan CarService iş akışının daha basit bir versiyonunu adım adım açıklamalı olarak örneklemeye çalıştık. Özel olarak host uygulamayı WPF üzerinde geliştirdik. Böylece Window uygulamalarında kullandığımız method invoker kavramının burada Dispatcher özelliği üzerinden ele alınabileceğini görme fırsatını elde ettik.(Dispatcher kavramına ilerleyen makalelerimizde değinmeye çalışıyor olacağım) State Machine Workflow tipinden iş akışlarının, bu senaryo dışında gerçek.Net nesneleri içerisindeki kullanımını araştırmanızı ve örneklemeye çalışmanızı şiddetle tavsiye ederim. İlerleyen makalalerimizde Windows Workflow Foundation ile ilgili farklı konulara değinmeye devam ediyor olacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/StateMachineOrnegi.rar)