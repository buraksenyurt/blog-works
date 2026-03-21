---
layout: post
title: "Dayanıklı WCF Servisleri(Custom Persistence Providers)"
date: 2009-01-23 12:00:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
---
Hatırlayacağınız üzere bir önceki makalemizde, dayanıklı WCF servislerinin (Durable WCF Services) nasıl geliştirilebileceğini incelemeye başlamıştık. Kısaca hatırlatmak gerekirse dayanıklı WCF servislerini şu şekilde tanımlayabiliriz; belirli bir süre için durumlarını bir depolama alanında saklayarak koruyabilen ve t süre sonrasında istemci uygulama tarafından bırakıldığı haliyle kullanılabilen servisler. Konunun ilk aşamasında, varsayılan olarak SQL sunucusu üzerindeki bir depolama alanının kullanıldığı senaryo üzerinde durmuştuk. (Hatırlayaım, SQL sunucusu üzerinde veriyi saklamak yönetsel açıdan pek çok işi kolaylaştırmaktadır.) Bu makalemizde ise kaldığımız yerden devam edeceğiz ve aşağıdaki iki soruya yönelik çözümler geliştirmeye gayret edeceğiz.

1- İstemci uygulama servis örneğini kullandıktan sonra kapatılır (İstemcinin kapatma işlemi sırasında servis örneğinin saklama alanından silinmesini sağlayacak metodu çağırmadığı varsayılır). Bu durumda t süre sonrasında istemci uygulama aynı servis örneğinin içeriğini kullanmak isterse nasıl bir süreç izlenmeli ve kodlama yapılmalıdır?

2- Varsayılan olarak kullanılan SqlPersistenceProviderFactory tipinin sağladığı SQL tabanlı saklama stratejisi yerine özelleştirilmiş başka bir sağlayıcı kullanılabilir mi?(Örneğin servis örnekleri dosya tabanlı bir sistemde veya farklı bir veritabanı üzerinde saklanabilirler mi? Oracle, Access vb.)

İlk olarak 1nci sorumuza cevap arayarak yazımıza devam edelim. Bilindiği üzere istemci uygulama, servise ait bir proxy nesnesi ürettiğinde ve bunun üzerinde bazı uzak operasyonları çağırdığında, her iki taraf arasında oturum (Session) bazlı olarak dolaşan bir InstanceId değeri söz konusudur. Buna göre istemci uygulamanın daha önceden depolama alanına attığı bir servis içeriğini tekrardan kullanabilmesi için, depolama alanında duran bu servis örneğinin InstanceId değerine ihtiyacı vardır. Bu değer bilindiği üzere servis tarafında üretilip istemciye içerik ile birlikte gönderilir ve operasyon çağrılarında karşılıklı olarak kullanılır. Dolayısıyla istemci uygulama, kullanmak istediği dayanıklı WCF servisine ait GUID tipinden olan InstanceId değerine sahip ise, saklanan veri içeriğini kullanıp işlemlerine bıraktığı yerden devam edebilir. Bu noktada tabiki istemcinin doğru InstanceId değerine sahip olması önemlidir.

Diğer taraftan Exception oluşmasına neden olabilecek pek çok durum vardır. Söz gelimi istemci uygulama servis örneğinin depolama alanından kaldırılmasına neden olan bir operasyon çağrısı yaptıktan sonra, InstanceId değerini kullanarak aynı servisin içeriğine kesinlikle ulaşamaz. Çünkü servis tarafındaki depolama alanında bu InstanceId değerine sahip içerik artık bulunmamaktadır.(Hemen hatırlayalım, varsayılan olarak CompleteInstance=true değerine sahip olan servis operasyonu, Instance içeriğinin depolama alanından silinmesine neden olmaktadır.) Geliştirme sırasında bu ve benzeri istisnai durumlara dikkat edilmesi önemlidir. En azından istisna yönetim mekanizmalarından yararlanılmalı ve sistemin işleyişi çok fazla aksatılmamalıdır.

Şimdi dilerseniz ilk sorumuzun cevabını bulmak için kodlamaya yavaş yavaş geçelim. Bu noktada işlemleri daha kolay izleyebilmek adına bir önceki makalede geliştirdiğimiz CommonService isimli serviste küçük bir değişiklik yapıyoruz. Söz konusu değişiklikler ile en büyük amacımız, istemci tarafında CommonValue değerini takip ederek, ilk sorumuzu daha kolay analiz edebilmektir. Buna göre IncreaseValue metodunun, servis içerisindeki CommonValue alanının değerini geriye döndürmesini sağlıyoruz. Tabi bu değişikliğin hem servis sözleşmesinde hemde servis sözleşmesini uyarlayan tipin içerisinde yapılması gerekmektedir.

> Servis tarafındaki sözleşmelerde değişiklik olması halinde (örneğin operasyonun dönüş tipi veya parametrelerinde değişiklik yapılması yada yeni operasyonların eklenmesi vb...) bu hizmeti kullanan istemci uygulamaların, servis referanslarını Update etmeleri mutlak suretle gereklidir.

Örneğimizde servis kütüphanesinde yapmış olduğu basit değişiklikler aşağıdaki gibidir.

ICommonService isimli servis sözleşmesinde yapılan değişiklik;

```csharp
[OperationContract]
int IncreaseValue(int value);
```

Servis sözleşmesini uyarlayan CommonService sınıfında yapılan değişiklik;

```csharp
[DurableOperation()]
public int IncreaseValue(int value)
{
    commonValue += value;
    return commonValue;
}
```

Gelelim istemci tarafına. İstemci tarafında bu kez basit bir Windows uygulaması kullanıyor olacağız. (İstemci uygulamamız sadece test amaçlıdır bu nedenle çok fazla beklentimiz olmamalıdır:) İstemcimizin Form1 tasarımı aşağıdaki gibidir.

![mk267_1.gif](/assets/images/2009/mk267_1.gif)

İstemci uygulamamızın kod içeriği ise şu şekildedir;

```csharp
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WinClientApp.CommonServiceReference;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WinClientApp
{
    public partial class Form1 
        : Form
    {
        private ServiceCommonClient client=null;

        public Form1()
        {
            InitializeComponent();
            // Proxy nesnesi örneklenir.
            client = new ServiceCommonClient("WSHttpContextBinding_ServiceCommon");
            lblStatus.Text = "Servis için proxy nesnesi örneklendi";
            Text += " (" + DateTime.Now.ToLongTimeString() + ")";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start metodu çağırılır. Bu metod çağrısı ile servis tarafında InstanceData tablosuna kayıt atılır.
            client.Start();
            lblStatus.Text = "Start metodu çağırıldı";
            txtInstanceId.Text = client.GetInstanceId().ToString(); 
        }

        private void btnIncreaseValue_Click(object sender, EventArgs e)
        { 
            // Sembolik olarak değer arttırımı servis üzerinden yapılır
            lblCommonValue.Text=client.IncreaseValue(10).ToString();
            lblStatus.Text = "IncreaseValue metodu çağırıldı";
        }

        private void btnStop_Click(object sender, EventArgs e)
        { 
            // Stop metodu çağırılır. Bu metod çağırıldığında servis tarafında InstanceData tablosunda saklanan satır silinir
            client.Stop();
            lblStatus.Text = "Stop metodu çağırıldı";
        }

        // Daha önceden kayıt edilmiş bir instance' ı kullanmak için aşağıdaki kod parçası kullanılır
        private void btnGetContext_Click(object sender, EventArgs e)
        {
            // IContextManager için System.WorkflowServices.dll assembly' ının referans edilmesi gerekir
            IContextManager conMng = ((IContextChannel)client.InnerChannel).GetProperty<IContextManager>();
            IDictionary<string, string> context = conMng.GetContext();
        
            if (!context.ContainsKey("instanceId"))
            {
                context.Add("instanceId", txtInstanceId.Text);
                conMng.SetContext(context);
            }
        }
    }
}
```

Bu uygulamada daha önceden depolanan bir servis örneğine ulaşılmak istendiğinden, System.ServiceModel.Channels isim alanındaki IContextManager isimli arayüzden yararlanılmaktadır. Bu arayüz System.WorkflowServices.dll assembly'ı içerisinde yer aldığından söz konusu.Net Framework 3.5 kütüphanesinin istemci uygulamaya referans edilmesi gerekmektedir. Örneğimiz tabiki geliştiriciler için yazılmıştır. Son kullanıcıya verildiği takdirde çalışma zamanında pek çok istisnanın oluşması muhtemel ve kaçınılmazdır.(Unutmayın şu anda bir test uygulaması geliştiriyoruz ve analiz yapmaya çalışıyoruz.)

İlk olarak servis kütüphanesini (Bir önceki makalemizden hatırlayacağınız gibi WCF kütüphaneleri otomatik olarak WcfSvcHost programı yardımıyla host edilebilmekte ve WcfTestClient ile test edilebilmektedir.) ve windows uygulamamızı çalıştıralım. Sonrasında ilk olarak Start düğmesine basacağız. Ardından IncreaseValue değerini arttırmak için bir kaç işlem yaptırılabiliriz. Ancak hiç bir şekilde Stop düğmesine basmayacağız. Bu en önemli test noktamız. Nitekim Stop düğmesine basılması sonrasında servis tarafında çağırılan operasyon, sahip olduğu DurableOperation niteliğinin CompleteInstace özelliğinin true değerine sahip olması nedeni ile instance'ın kaldırılmasına neden olacaktır. Testin bu aşamasında uygulamamızın ekran görüntüsüde aşağıdakine benzer olacaktır. Yine burada testin ikinci aşaması için önem arz eden konulardan biriside servis tarafından üretilen InstanceId değeridir. Bu değeri saklamamız gerekiyor.(Lütfen Form üzerindeki tarihe dikkat edin.)

![mk267_2.gif](/assets/images/2009/mk267_2.gif)

Testimizin ikinci aşamasında uygulamamızı kapatıp çıktığımızı düşünelim. Eğer veritabanına gider ve InstanceData tablosuna bakacak olursak, servis örneğinin satır bazında eklenmiş olduğunu rahatlıkla görebiliriz.

![mk267_3.gif](/assets/images/2009/mk267_3.gif)

XML verisinde ise servis tarafındaki CommonValue değişkeninin aşağıdaki şekilde görüldüğü gibi 41 olarak tutulduğu gözlemlenebilir.

![mk267_4.gif](/assets/images/2009/mk267_4.gif)

Gel zaman git zaman aradan uzunca bir süre geçer ve biz aynı uygulamayı bir kere daha çalıştırırız. Ancak bu sefer, Start metodu yerine daha önceden sakladığımız servis InstanceId değerini TextBox kontrolüne ekleyip GetContext isimli metodu çağırmalıyız. Buna göre daha önceden kayıt edilmiş olan servis örneğin içeriğinin kullanılması için ilk adımı atmış oluruz ki uygulamanın çalışma zamanı ekran görüntüsüde aşağıdaki gibi olacaktır.

![mk267_5.gif](/assets/images/2009/mk267_5.gif)

Dikkat edilecek olursa uygulamayı çalıştırdığımıda Form'un Text'i üzerinde saat bilgisi olarak 00:16:51 yazmaktaydı.(Gerçektende aynı günün akşamında örneği tekrardan çalıştırdığımı söyleyebilirim). Ancak şu anda saat 21:31:45 ve servis örneğinin veritabanında kayıtlı olan içeriğini kullanaraktan, CommonValue değişkeninin son değerini 51 olarak değiştirmiş bulunuyoruz. Dolayısıyla bir servisin nasıl dayanıklı hale getirilebileceğini ve bununla birlikte istemcilerin daha önceki instance'lara nasıl ulaşabileceklerini görmüş olduk.

Elbetteki gerçek hayat vakalarında söz konusu instanceId değerlerini istemci tarafında daha düzenli bir şekilde saklamak gerekebilir. Belikde uygulama kapatılırken instanceId değeri konfigurasyon düzeyinde saklanabilir ve bir sonraki açılışta değerlendirilerek istemcinin, servis içeriğinin son hali üzerinden otomatik olarak devam etmeside sağlanabilir. Tahmin edeceğiniz üzere dayanıklı WCF servisleri, Workflow tarzındaki uygulamalarda daha yaygın olarak kullanılmaktadır. Nitekim WF uygulamaları kendi içlerinde zaten var olan bir sürerlik yapısına sahiptir ve bir akışın anlık olarak uzun süreliğine saklanması mümkündür. Öyleki WCF servislerini dayanıklı hale getirmek içinde Workflow'un ana assembly'larından olan System.WorkflowServices.dll'dan yararlanılmaktadır.

Gelelim makalemize konu olan ikinci sorumuza. Özelleştirilmiş bir sürerlik sağlayıcısını (Custom Persistence Provider) nasıl geliştirebiliriz? Burada iş biraz daha zorlaşmaktadır. Bu konudaki en güzel örnek Microsoft tarafından geliştirilmiş olup [MSDN](http://go.microsoft.com/fwlink/?LinkId=87352) den indirip inceleyebilirsiniz. Biz çok daha basit haliyle ele alacağız ve bu nedenle özellikle asenkron yaklaşımları göz ardı edeceğiz.

İlk olarak PersistenceProviders isimli bir sınıf kütüphanesi (Class Library) oluşturarak işe başlayabiliriz. Söz konusu kütüphane içerisinde özel bir sürerlik sağlayıcısı (Persistence Provider) kullanacağımız için System.ServiceModel.dll, System.WorkflowServices.dll assembly'larını referans etmemiz gerekiyor. Bunların haricinde örneğimizde kullanacağımız özel sağlayıcı, çalışma zamanındaki servis örneklerine ait verileri SOAP (SimpleObjectAccessProtocol) serileştirme yaparak saklayacaktır.

> Elbetteki özel sürerlik sağlayıcısının hangi tipte veri kaynağı ile çalışacağı tamamen geliştiriceye ve ortam şartlarına bağlıdır. Sadece dosya sistemi değil Oracle gibi SQL Server harici veritabanları da kolaylıkla ele alınabilir.

Biz örneğimizde SOAP serileştirmesini kullanacağımızda System.Runtime.Serialization.Formatters.Soap.dll assembly'ınında sınıf kütüphanesine referans edilmesi gerekmektedir. Model temel olarak Abstract Factory tasarım desenine uygun olarak geliştirilmelidir.

![mk267_6.gif](/assets/images/2009/mk267_6.gif)

Doğruyu söylemek gerekirse biraz karmaşık görünen bir yapı. Ancak incelendiği takdirde içerisinde asenkron operasyonların dahi yer aldığını (ki bunun uyarlamalarını eklemedik kodlarımıza), Abstract Factory deseninin uygulandığını rahatlıkla görebiliriz. Bir başka deyişle XmlPersistenceProviderFactory sınıfı, özel depolama işlemleri için XmlPersistenceProvider tipini örneklemektedir. Bu, WCF servislerinde özel sağlayıcıların üretimi sırasında kullanılan standart yoldur. Örnekte kullandığımız XmlPersistenceProviderFactory tipi, abstract PersistenceProviderFactory tipinden türemektedir. Bu sınıfın içerisinde ezilmek (override) zorunda olan CreateProvider isimli metod ise PersistenceProvider tipinden bir referans döndürmektedir.

Örneğimizde söz konusu dönüş referansı olarak PersistenceProvider sınıfından türetilen XmlPersistenceProvider tipi kullanılmaktadır. Buna ek olarak PersistenceProviderFactory tipi CommunicationObject'ten türemektedir ve bu sebeptende CreateProvider haricinde ek metodlarıda uyarlamaktadır. Söz gelimi OnBeginClose, OnEndClose gibi metodlar veya DefaulfCloseTimeout, DefaultOpenTimeout gibi özellikler CommunicationObject tipi içerisinde abstract olarak tanımlandıklarından otomatik olarak XmlPersistenceProviderFactory sınıfınada uygulanmaktadırlar. Ek olarak, XmlPersistenceProvider tipinin ortaklaşa kullanacağı Create,Load,Update,Delete gibi işlevselliklere ait metodlar, XmlPersistenceProviderFactory tipi içerisinde yer almaktadır.

İki sınıf arasındaki iletişimin sağlanması amacıyla XmlPersistenceProvider tipinin yapıcı metodu (Constructor Method) içerisine, XmlPersistenceProviderFactory sınıfının çalışma zamanı referansı aktarılmaktadır. Böylece XmlPersistenceProvider tipi içerisinden, factory sınıfında tanımlanan genel üyelere erişilebilir. (PersistenceProvider türevli sınıf istenirse ProviderPersistenceFactory türevli sınıf içerisinde dahili tip-inner type olarakta tasarlanabilir. Microsoft'un örneğinde bu tarz bir kullanım söz konusudur.) XmlPersistenceProviderFactory tipi ile ürettiği XmlPersistenceProvider sınıfının içerikleri ise aşağıdaki gibidir.

XmlPersistenceProviderFactory içeriği;

```csharp
using System;
using System.ServiceModel.Persistence;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

namespace PersistenceProviders
{
    public class XmlPersistenceProviderFactory
        : PersistenceProviderFactory
    {
        #region Gerekli Sınıf Değişkenleri

        // Servis örneklerinin saklanacağı varsayılan klasörü tutan değişken
        private string fileStoragePath = Path.Combine(System.Environment.CurrentDirectory, "Instances");
        // Soap serileştirmesini yapacak olan SoapFormatter değişkeni
        SoapFormatter sFrmtr = null;
        Logger lgr = null;
        Guid InstanceId;

        #endregion

        #region Yardımcı Metodlar

        private string InstanceFileName()
        {
            // Dosya adı belirlenir. Ad belirlenirken ayırt edici olması için base sınıftan Id özelliğinin değeri kullanılır
            string fileName = Path.Combine(fileStoragePath, InstanceId.ToString() + ".sxml");
            return fileName;
        }

        #endregion

        #region XmlPersistenceProvider sınıfı için kullanılan ortak CRUD metodları
    
        // Servis örneği için depolama kaynağının oluşturulması ve içeriğinin serileştirilmesi amacıyla kullanılır
        public object CreateInstance(object instance, TimeSpan timeout)
        {
            string fileName = InstanceFileName();
        
            // Soap formatter tipinden yararlanılarak servis örneğinin serileştirilmesi sağlanır
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                sFrmtr.Serialize(stream, instance);
            }

            lgr.AddEventEntry(fileName + " için Create işlemi yapıldı, Serileştirme gerçekleştirildi.", System.Diagnostics.EventLogEntryType.Information);
            return null;
        }

        // Servis örneğinin depolama kaynağının silinmesi amacıyla kullanılır
        public void DeleteInstance(object instance, TimeSpan timeout)
        {
            // Dosya adı elde edilir
            string fileName = InstanceFileName();
            // Eğer dosya sistemde var ise
            if (File.Exists(fileName))
            {
                // Dosya silinir
                File.Delete(fileName);
                // Silme bilgsi loglanır
                lgr.AddEventEntry(fileName + " için Delete işlemi yapıldı.", System.Diagnostics.EventLogEntryType.Information);
            }
            else // yok ise
            {
                // Dosya sistemde bulunamadıysa normal şartların aksine başka bir nedenle silinmiş olabilir. Bu durum loga aktarılır
                lgr.AddEventEntry(fileName + " sistemde bulunamadığından Delete işlemi yapılamadı", System.Diagnostics.EventLogEntryType.Warning);
            }
        }

        // Kaydedilmiş olan servis örneğinin kaynaktan alınıp kullanılabilir object haline getirilmesi için kullanılır
        public object LoadInstance(TimeSpan timeout)
        {
            string fileName = InstanceFileName();
            object result = null;
            if (File.Exists(fileName)) // dosya mevcutsa
            {
                // loglama yap
                lgr.AddEventEntry(fileName + " için Load işlemi yapıldı", System.Diagnostics.EventLogEntryType.Information);
                // servis örneğinin dosya içerisinde tutulan serileştirilmiş örneğini deSerialize ederk geriye döndür
                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    result = sFrmtr.Deserialize(stream);
                    stream.Close();
                }
                return result;
            }
            else // dosya bulunamıyorsa
            {
                // uyarı mesajı niteliğinde loglama yap
                lgr.AddEventEntry(fileName + " sistemde bulunamadığından Load işlemi gerçekleştirilemedi", System.Diagnostics.EventLogEntryType.Warning);
                return null;
            }
        }

        // Kaynakta tutulan servis örneğinin güncellenmesi işlemini gerçekleştirir.
        public object UpdateInstance(object instance, TimeSpan timeout)
        {
            // Dosya adı alınır
            string fileName = InstanceFileName();
            
            // Soap formatter tipinden yararlanılarak servis örneğinin serileştirilmesi sağlanır
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                // Güncellenen servis örneği tekrardan aynı dosya üzerine serileştirilir
                sFrmtr.Serialize(stream, instance);
            }

            lgr.AddEventEntry(fileName + " için Update işlemi yapıldı, Serileştirme gerçekleştirildi.", System.Diagnostics.EventLogEntryType.Information);
            return null;
        }

        #endregion
    
        public XmlPersistenceProviderFactory()
        {
            // Eğer servis örneklerinin saklanacağı varsayılan klasör yoksa oluştur
            if (!Directory.Exists(fileStoragePath))
                Directory.CreateDirectory(fileStoragePath);

            // Soap Formatter değişkeni oluşturulur
            sFrmtr = new SoapFormatter();
    
            // Loglama işlemini yapacak olan sınıf örneği oluşturulur
            lgr = new Logger("CustomXmlPersistenceProvider", "Custom Xml Persistence Log(WCF)");
    
            lgr.AddEventEntry("XmlPersistenceProviderFactory örneği oluşturuldu", System.Diagnostics.EventLogEntryType.SuccessAudit);
        }

        public override PersistenceProvider CreateProvider(Guid id)
        {
            InstanceId = id;
            return new XmlPersistenceProvider(id, this);
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return TimeSpan.FromSeconds(10); }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return TimeSpan.FromSeconds(10); }
        }

        protected override void OnAbort()
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override void OnEndClose(IAsyncResult result)
        {
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }
    } 
}
```

Görüldüğü gibi basit serileştirme (Serialization) ve ters-serileştirme (DeSerialization) işlemleri gerçekleştirilmektedir. Bununla birlikte örneğin bizim için (en azından benim için) kolay anlaşılabilir olması adına asenkron metodların içerikleri uyarlanmamıştır.

XmlPersistenceProvider sınıfının içeriği;

```csharp
using System;
using System.ServiceModel.Persistence;

namespace PersistenceProviders
{
    public class XmlPersistenceProvider
        : PersistenceProvider
    {
        XmlPersistenceProviderFactory XmlPrsFactory;

        public XmlPersistenceProvider(Guid id, XmlPersistenceProviderFactory xmlFactory)
            : base(id)
        {
            XmlPrsFactory = xmlFactory;
        }

        #region PersistenceProvider ve CommunicationObject Üyeleri

        // Servis örneği depolama alanında ilk oluşturulduğunda çalışır.
        public override IAsyncResult BeginCreate(object instance, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        // Silme işlemi başladığında devreye girer.
        public override IAsyncResult BeginDelete(object instance, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        // Yükleme işlemi başladığından devreye girer.
        public override IAsyncResult BeginLoad(TimeSpan timeout, AsyncCallback callback, object state)
         {
            return null;
        }

        // Servis örneği güncellemeye başladığında devreye girer
        public override IAsyncResult BeginUpdate(object instance, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        // Depolama alanında servis örneğinin kayıt altına alınmasında kullanılır
        public override object Create(object instance, TimeSpan timeout)
        {
            return XmlPrsFactory.CreateInstance(instance, timeout);
        }

        // Servis örneğinin depolama alanında silinmesinin ele alındığı metoddur
        public override void Delete(object instance, TimeSpan timeout)
        {
            XmlPrsFactory.DeleteInstance(instance, timeout);
        }

        // Servis örneğinin veri içeriğinin depolama alanına ilk kayıt edildiği aşamanın sonunda devreye giren metoddur
        public override object EndCreate(IAsyncResult result)
        {
            return null;
        }

        // Servis örneğine ait veri içeriğinin depolama alanında silindiği aşamının sonunda devreye giren metoddur
        public override void EndDelete(IAsyncResult result)
        {
        }

        // Servis örneği verisinin depolama alanında ortama yüklendiği aşamın sonunda devreye giren metoddur
        public override object EndLoad(IAsyncResult result)
        {
            return null;
        }

        // Servis örneği verisinin depolama alanında güncelleştirilmesi sonrasında devreye giren metoddur
        public override object EndUpdate(IAsyncResult result)
        {
            return null;
        }

        // Servis örneği verisinin depolama alanından yükenmesi aşamasının ele alındığı metoddur
        public override object Load(TimeSpan timeout)
        {
            return XmlPrsFactory.LoadInstance(timeout);
        }

        // Servis örneği verisinin depolama alanında güncelleştirilmesi aşamasının ele alındığı metoddur
        public override object Update(object instance, TimeSpan timeout)
        {
            return XmlPrsFactory.UpdateInstance(instance, timeout);
        }

        // Kapatma operasyonun tamamlanması için gereken varsayılan sürenin belirlendiği özelliktir
        protected override TimeSpan DefaultCloseTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(10);
            }
        }

        // Açma operasyonunun tamamlanması için gereken varsayılan sürenin belirlendiği özelliktir
        protected override TimeSpan DefaultOpenTimeout
        {
            get
            {    
                return TimeSpan.FromSeconds(10);
            }
        }

        protected override void OnAbort()
        {
        }
    
        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override void OnEndClose(IAsyncResult result)
        {
        }
    
        protected override void OnEndOpen(IAsyncResult result)
        {
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }
    
        #endregion
    }
}
```

Dikkat edileceği üzere XmlPersistenceProvider tipi ağırlıklı olarak CRUD işlemleri için gerekli olan zorunlu metod çağrılarını PersistenceProvider abstract sınıfından devralmaktadır. Create, Update, Delete ve Load isimli zorunlu olarak ezilen metodlar, XmlPersistenceProviderFactory tipi içerisinde tanımlanmış olan CreateInstance, UpdateInstance, DeleteInstance ve LoadInstance fonksiyonelliklerini çağırmaktadır. Aslında Factory sınıfı kendisini kullanan birden fazla PersistenceProvider türevli tipide ele alabilir. Hepsi için ortak olabilecek pek çok fonksiyonellik fabrika tipi içerisinde toplanabileceği gibi, PersistenceProvider türevli tiplerede yayılabilir. (Bence bu konuyu araştırabilirsiniz. Tek bir PersistenceProviderFactory türevli tip üstünden birden fazla PersistenceProvider türevli tipi kullanabilmek ve bunları farklı vakalar için ele alabilmek.)

Son olarak, kütüphanemizde süreci, oluşabilecek çalışma zamanı hatalarını loglamak amacıyla Logger isimli birde yardımcı sınıfımız bulunmaktadır. Bu sınıf yardımıyla kodun gerekli yerlerinde ilgili bilgileri günlüklere işleme şansına sahip oluyoruzki, geliştirme (Development) ve özellikle test aşamalarında bu oldukça işimize yarayacak bir özelliktir. Söz gelimi WCF kütüphanesinin bir Windows Servisi içerisinde host edilmesi halinde, hataların ayıklanması için ekstra çaba sarfetmek gerekmektedir. Bu nedenle Windows Servisinin host olarak kullanıldığı bir senaryoda log kayıtları hayati bilgiler verebilir ve hataların tespit edilmesi çok daha kolaylaşabilir.

![mk267_7.gif](/assets/images/2009/mk267_7.gif)

Logger sınıfımızın içeriği ise aşağıdaki gibidir;

```csharp
using System.Diagnostics;

namespace PersistenceProviders
{
    /// <summary>
    /// Loglama işlemleri için yardımcı sınıftır
    /// </summary>
    public class Logger
    {
        private string LogSource;

        /// <summary>
        /// Event Viewer içerisinde ayrı bir log sekmesi oluşturur
        /// </summary>
        /// <param name="logSource">Log verisinin kaynağı(Genellikle işlem adı yada program adı olarak kullanılabilir)</param>
        /// <param name="logName">Event Viewer altındaki boğumun adıdır</param>
        public Logger(string logSource, string logName)
        {
            LogSource = logSource;
            if(!EventLog.Exists(logName))
                 EventLog.CreateEventSource(logSource, logName);
        }

        /// <summary>
        /// Yapıcı metodda oluşturulan log kaynağına event yazdırır
        /// </summary>
        /// <param name="mesaj">Event ile ilişkili bilgi</param>
        /// <param name="entryType">Event in EventLogEntryType tipinden değeridir</param>
        public void AddEventEntry(string mesaj, EventLogEntryType entryType)
        {
            EventLog.WriteEntry(LogSource, mesaj, entryType);
        }
    }
}
```

Örnek sürerlik sağlayıcımız, uygulamanın olduğu yerde (yada serivis host eden uygulamanın olduğu yerde) Instances adlı bir klasör açmaktadır.

> Uygulamada dosyaların kaydedileceği klasör bilgisi konfigurasyon seviyesinde de tutulabilir. Hatırlayalım, SqlPersistenceProviderFactory sağlayıcısı kullanıldığında örneklerin saklanması için kullanılacak veritabanı bağlantısı, ConnectionString elementinin üzerinden alınmaktadır.

Çalışma zamanındaki servislere ait örnekler bu klasör altında sxml uzantılı olarak tutulmaktadır. İçerikleri tahmin edileceği üzere SOAP formatındadır. Dosya adları servis örneğine ait InstanceId değerleri olarak verilmektedir. Akış son derece basittir. DurableOperation niteliğinin CanCreateInstance özelliğinin değeri true olan operasyon çağrısı ile bu klasörde bir dosya oluşturulmakta ve servis örneğine ait içerik serileştirilmektedir. Sonrasında yapılan operasyon çağrılarında bu dosya içeriğinin ters serileştirilerek object olarak elde edilmesi ve içeriğinin kullanılması söz konusudur. Yine DurableOperation niteliğinin CompleteInstance özelliği true olan operasyon çağırıldığında söz konusu servis örneğine ait dosya, Instances klasöründen silinmektedir. Tabi istenirse söz konusu dosyanın silinmeyip yeniden isimlendirilerek örneğin belirli süreliğine sistemde tutulması gibi işlemlerde yapılabilir.

Bu teorik akışı elbette test ederek onaylamamız gerekmektedir. Bunun için basit bir istemci ve servis host uygulaması yazabilir yada özel sürerlik sağlayıcılarını kullanan servis kütüphanesini doğrudan Visual Studio 2008 ortamında çalıştırabiliriz. Hangisi kullanılırsa kullanılsın servis tarafındaki konfigurasyon içeriğinde bir önceki makalemizde yaptığımız gibi PersistenceProviderFactory türevli tipin belirtilmesi gerekmektedir. Örnek uygulamada bu amaçla geliştirilmiş olan CustomServiceLib isimli servis kütüphanesi bir önceki makalede geliştirdiğimiz ICommonService sözleşmesi ile CommonService tipini kullanmaktadır. App.config dosyasının içeriği ise aşağıdaki gibidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="CommonServiceBehavior">
                    <serviceDebug includeExceptionDetailInFaults="true" />
                    <serviceMetadata httpGetEnabled="true" />
                    <persistenceProvider type="PersistenceProviders.XmlPersistenceProviderFactory,PersistenceProviders, Verison=1.0.0.0" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="CommonServiceBehavior" name="CustomServiceLib.CommonService">
                <endpoint address="" binding="wsHttpContextBinding" bindingConfiguration="" name="CommonServiceWsHttpEndPoint" contract="CustomServiceLib.ICommonService" />
                <endpoint binding="mexHttpBinding" bindingConfiguration="" name="MexEndPoint" contract="IMetadataExchange" address="Mex" />
                <host>
                    <baseAddresses>
                        <add baseAddress="http://localhost:40001/CommonServiceV2" />
                    </baseAddresses>
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Görüldüğü üzere, persistenceProvider elementi içerisinde özelleştirilmiş olan XmlPersistenceProviderFactory tipi belirtilmektedir. Söz konusu tipin bulunduğu kütüphane referans etme yoluyla kullanıldığından br önceki örnekte olduğu gibi Culture veya public key bilgileri bulunmamaktadır. Elbetteki özelleştirilmiş depolama sağlayıcısını bünyesinde barındıran kütüphaneyi Global Assembly Cache'e atabiliriz. Bu durumda konfigurasyon içerisinde Full Qualified Name'in belirtilmesi gerekmektedir.

CustomServiceLib kütüphanesini direkt olarak Visual Studio 2008 üzerinden çalıştırarak ilerleyebiliriz. WcfSvcHost ve paralelinde Wcf Test Client çalıştırıldıktan sonra ilk yapmamız gereken Start metodunu çağırmak olmalıdır. Bu noktada uygulamanın çalıştığı yerdeki Instances klasörüne baktığımızda aşağıdaki ekran görüntüsünde olduğu gibi sxml uzantılı ve GUID değeri ile adlandırılmış bir dosyanın oluşturulduğunu görürüz.

![mk267_8.gif](/assets/images/2009/mk267_8.gif)

Bu adımdan sonra test olarak IncreaseValue metodu değişik değerler ile çağırlabilir. Operasyonun her çağırılmasında, servis örneğine ait içeriğin oluşturulan dosyadan yüklenmesi, ters serileştirildikten sonra içeride yer alan CommonValue değerinin arttırılması ve örneğin son halinin tekrardan aynı dosya üzerine serileştirilmesi adımları gerçekleşir. Örneğin şu noktada sxml isimli dosyanın içeriğine bakılırsa aşağıdaki XML bilgisinin tutulduğu görülebilir.

![mk267_10.gif](/assets/images/2009/mk267_10.gif)

Bu andan itibaren Stop metodu çağırılmadan Wcf Test Client uygulamasından çıkılabilir. Hatta aynı örnek defalarca çağırılıp Stop operasyonu kullanılmadan test edilebilir. Her durumda Instances klasöründe birer dosya oluşturulacak ve herhangibir nedenle silinene kadar sahip oldukları InstanceId değerleri için servis tarafı istemcilere hizmet verebilecektir. Ancak Stop metodu çağırılırsa bu durumda Instances klasöründe bununla ilişkili olan dosyanın silindiği açık bir şekilde görülebilir. İşleyişi daha net bir şekilde anlayabilmek için kodu debug ederek çalıştırmanızı öneririm. Bu durumda Create, Update, Delete, Load metodları arasındaki geçişleri çok daha kolay bir şekilde analiz edebilirsiniz. Son olarakta servis kütüphanesini kullanan bir sunucu uygulama ve istemci geliştirerek yeni sağlayıcıyı test etmenizi ve sistemin düzgün yürüdüğünden emin olmanızı öneririm.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde dayanıklı WCF servisleri ilişkili iki önemli soruya cevap bulmaya çalışltık. İlk olarak istemcilerin var olan InstanceId'leri kullanarak servislerin, sunucu üzerinde uzun süreliğine saklanabilen hallerini nasıl kullanabileceklerine değinik. Sonrasında ise özel bir sürerlik sağlayıcısının (Custom Persistence Provider) nasıl yazılabileceğini gördük. Tabiki buradaki örnekleri geliştirmek, genişletmek tamamen sizin elinizdedir. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örneği İndirmek İçin Tıklayın](/assets/files/2009/CustomDurableProviders.rar)