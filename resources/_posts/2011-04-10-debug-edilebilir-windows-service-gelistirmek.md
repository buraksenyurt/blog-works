---
layout: post
title: "Debug Edilebilir Windows Service Geliştirmek"
date: 2011-04-10 10:34:00 +0300
categories:
  - windows-services
tags:
  - windows-services
  - debug
---
Uzun süre önce dış kaynak (Outsource) olarak görev aldığım bir bankacılık uygulamasında Windows Service tabanlı entegrasyon işlemleri için görevlendirilmiştim. Herşeyden önce bu servislerin bankacılık uygulaması olması nedeniyle, farklı ve yabancı sistemleri de ilgilendiren iş adımları bulunmaktaydı. Bu sebepten söz konusu Windows Service uygulamalarının hem kod içerikleri hem de iş kuralları oldukça karışık olabiliyordu. İlgili Windows Service örneklerinin geliştirilmesi bir yana, bunların test ortamına atılması ve sonuçlarının takip edilmesi ise başlı başına bir dertti

[![blg232_Giris](/assets/images/2011/blg232_Giris_thumb.jpg)](/assets/images/2011/blg232_Giris.jpg)


![Confused smile](/assets/images/2011/wlEmoticon-confusedsmile_1.png)

Genellikle servisin çalışma durumunu izlemek adına özellikle Exception bloklarında veya metod başlangıç ile bitiş noktalarında (örneğin OnStart başında ve sonunda) işletim sisteminin uygulamaya özel Event Log’ larına bilgi atmaktaydım. Bu bilgileri atarken de durumun kritikliğine göre Warning, Exclamation, Error gibi hazır sistem ikonlarından yararlanıyor ve çalışma zamanındaki durumu analiz etmeye çalışıyordum.

Ancak bir developer için, kodun çalışma zamanındaki durumunu incelemenin sayısız yolu olduğu da bir gerçek. Öyleki, Debug etmek bence en güzel yollardan birisi. Lakin bir Windows Service uygulamasının Debug edilmesi de sanıldığı kadar kolay değil

![Thinking smile](/assets/images/2011/wlEmoticon-thinkingsmile.png)

İşte bu yazımızda internet üzerinden yaptığım araştırmalar sonucu öğrendiğim ve bir Windows Service uygulamasının nasıl debug edilebeceğine dair uygulanabilen yöntemlerden birisini ele alıyor olacağız. Olabildiğince basit bir şekilde anlatmaya gayret edeceğim bu vaka çalışmamızda, adım adım ilerliyor olacağız

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_2.png)

Öyleyse gelin şu metal entegre üzerindeki böcekleri ayıklamaya çalışalım.

İlk olarak Windows Service Application tipinden bir uygulama oluşturmamız gerekiyor. Bu uygulama içerisinde yer alan ControllerService isimli Windows Service tipinin içeriği başlangıçta aşağıdaki gibidir. Söz konusu servis içerisindeki Timer örneğinin 10 saniyede bir çalıştırdığı olay metoduna göre, belirli bir klasördeki (ki path bilgisi App.config dosyasından çekilmektedir) dosyaların şifrelenmesi için bir akış çalıştırmaktadır ki aslında bunun konumuz için çok da büyük bir önemi yoktur

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_2.png)

```csharp
using System.Configuration; 
using System.IO; 
using System.ServiceProcess; 
using System.Timers;

namespace FileControllerService 
{ 
    public partial class ControllerService 
        : ServiceBase 
    { 
        private string _path = null; 
        private Timer _timer = null;

        public ControllerService() 
        { 
            InitializeComponent(); 
            _path = ConfigurationManager.AppSettings["Path"]; 
            _timer = new Timer(10000); 
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed); 
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e) 
        { 
            string[] files=Directory.GetFiles(_path); 
            foreach (string file in files) 
            { 
                File.Encrypt(file); 
            } 
        } 
        protected override void OnStart(string[] args) 
        { 
            _timer.Start(); 
        }

        protected override void OnStop() 
        { 
            _timer.Stop(); 
        } 
    } 
}
```

Bir Windows Service tipi temel olarak ServiceBase tipinden türemektedir. Windows Service Application proje şablonunda yer alan bu hazır uyarlamaya göre Program sınıfı içerisindeki Main metodunun kod yapısı da aşağıdaki gibi olacaktır.

```csharp
using System; 
using System.Configuration; 
using System.ServiceProcess; 
using InvestigationLib;

namespace FileControllerService 
{ 
    static class Program 
    { 
        static void Main() 
        { 
            ServiceBase[] ServicesToRun; 
            ServicesToRun = new ServiceBase[] 
            { 
                new ControllerService() 
            }; 
            ServiceBase.Run(ServicesToRun); 
        } 
    } 
}
```

Dikkat edileceği üzere Servisin çalıştırılması işini ServiceBase tipinin static Run metodu üstlenmektedir. Ne varki bu tip bir servisin varsayılan olarak Debug edilmesi mümkün değildir. Bir başka deyişle servise ait OnStart ve OnStop gibi metodların içerisine girilememektedir. Bu, özellikle karmaşık iş kuralları ve modelleri içeren Windows Service örnekleri düşünüldüğünde, geliştirme sürecini zorlaştıran ve uzatan bir durumun oluşması anlamına gelmektedir. Elbette servisin pek çok noktasından örneğin işletim sistemi Event Log’ larına bilgi gönderilebilir veya bir Text dosyası içerisine servisin çalışması sırasındaki sürecin önemli adımlarına ait bilgiler (mesela Exceptipon mesajları) yazdırılabilir. Ancak değişken, parametre ve metod sayılarının iyiden iyiye fazlalaştığı, referans edilen kütüphanelerin çok olduğu bir modelde bu içerikleri izlemekte, üretmekte zor olmaktadır.

Aslında bir geliştirici için kodun yazılması ve test ortamına alınması esnasında debug edilebilyor olması son derece önemlidir. Şimdi dilerseniz yazımıza konu olan bu basit Windows Service örneğinin debug edilebilir bir versiyonunu nasıl geliştirebileceğimize bir bakalım

![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_2.png)

Durumu anlamanın en kolay yolu bitmiş olan örneğin sınıf diagramına bakmak olacaktır (Yani çözüme biraz tersten bakmamız gerekmektedir) İşte çözümsel yaklaşıma ait sınıf diagramı görüntüsü.

[![blg232_ClassDiagram](/assets/images/2011/blg232_ClassDiagram_thumb.gif)](/assets/images/2011/blg232_ClassDiagram.gif)

Şimdi bu sınıf diagramını adım adım üretmeye başlayalım. İlk olarak bir arayüz sözleşmesi (Interface Contract) oluşturarak işe başlayacağız. Buradaki en büyük amacımız birden fazla Windows Service tipinin Debug edilebilir versiyonları için ortak bir sözleşme sunmaktır.

```csharp
namespace InvestigationLib 
{ 
    public interface IWindowsServiceContract 
    { 
        void OnStart(string[] args); 
        void OnStop(); 
    } 
}
```

IWindowsServiceContract arayüzünün içinde iki basit metod bildirimi olduğu görülmektedir. Örneğimizi çok basit tutmak istediğimizden OnPause, OnContinue, OnShutdown gibi metod bildirimlerini göz önüne almadık. Ancak siz kendi denemelerinizi yaparken veya bir ürün geliştirmesi sırasında bu tekniği uygularken mutlaka söz konusu diğer metodları da düşünmelisiniz. Aslında yaptığımız bir anlamda ServiceBase ile override edilen metodlara ait bir sözleşme bildiriminde bulunmaktan ibarettir.

Bu sözleşme bildiriminin ardından ServiceBase türevli olan ve IWindowsServiceContract arayüzünü implemente eden tiplerin fonksiyonelliklerini çağırabilen başka bir sınıf daha üretilir. WindowsServiceCaller…

```csharp
using System.ServiceProcess;

namespace InvestigationLib 
{ 
    public class WindowsServiceCaller 
        :ServiceBase 
    { 
        IWindowsServiceContract _winSrv;

        public WindowsServiceCaller(IWindowsServiceContract winSrv)            
        { 
            _winSrv = winSrv;   
        } 
        protected override void OnStart(string[] args) 
        { 
            _winSrv.OnStart(args); 
        } 
        protected override void OnStop() 
        { 
            _winSrv.OnStop(); 
        } 
    } 
}
```

WindowsServiceCallar tipi için söylenebilecek iki önemli nokta vardır. Bunlardan ilki ServiceBase tipinden türemiş olması ve senaryomuza göre OnStart ile OnStop metodlarını override etmesidir. Diğer yandan ezilen OnStart ve OnStop metodları içerisinden yapılan çağrılar, tipin yapıcı metodu (Constructor) içerisinden alınan IWindowsServiceCaller arayüzünü implemente eden herhangibir nesne örneğine aittir. Bir başka deyişle bu tip, OnStart ve OnStop metod bildirimlerini sözleşme olarak sunan IWindowsServiceContract arayüzünü implemente eden bir tipin, çalışma zamanındaki asıl OnStart ve OnStop fonksiyonelliklerini kullanmaktadır. (Oh oh ohhh!!! Including var, inheritance var, Polimorphysm var, overriding var ![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_2.png)… Nesne Yönelimli Programlama-Object Oriented Programming temellerini hatırlamanın zamanı)

Dolayısıyla bu tanımlamanın ardınan debug edilmesi gereken kod içeriğini taşıyan asıl servis tipi geliştirilir ki tesadüf bu olsa gerek bu tipte IWindowsServiceContract arayüzünü implemente etmektedir

![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_2.png)

```csharp
using System; 
using System.IO; 
using System.Timers;

namespace InvestigationLib 
{ 
    public class DebugableControllerService 
        :IWindowsServiceContract 
    { 
        string path=String.Empty; 
        Timer timer = null;

        public DebugableControllerService() 
        { 
            timer = new Timer(10000); 
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed); 
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e) 
        { 
            try 
            { 
                string[] files = Directory.GetFiles(path); 
                foreach (string file in files) 
                { 
                    File.Encrypt(file); 
                } 
            } 
            catch (Exception excp) 
            { 
                //TODO:Handle Exception 
                // Eğer Windows Service projesinin özelliklerinden Output Type -> Console Application olarak seçilirse buradan Console penceresine de bilgi yazdırılabilir 
            } 
        }

        #region IWindowsServiceContract Members

        public void OnStart(string[] args) 
        { 
            path = args[0]; 
            timer.Start(); 
        }

        public void OnStop() 
        { 
            timer.Stop(); 
        }

        #endregion 
    } 
}
```

DebugableControllerService sınıfı aslında ilk başta debug edemediğimiz ControllerService sınıfının asıl fonksiyonelliklerini içermektedir. Tabi ControllerService tipinin yaptığı gibi ServiceBase’ den türemek yerine IWindowsServiceContract arayüzünü uygulamaktadır. Böylece WindowsServiceCaller tipinin kullanabileceği bir sınıf oluşturulmuştur. Ancak hazırlıklar bu tipleri yazmakla bitmez. Birde söz konusu debug edilecek tip örneğini başlatacak/yürütüecek bir başka sınıfın olmasında yarar vardır

![Crying face](/assets/images/2011/wlEmoticon-cryingface.png)

İşte başlatıcı tipimiz.

```csharp
using System.Threading;

namespace InvestigationLib 
{ 
    public static class ServiceStarter 
    { 
        public static void Run(string[] args,IWindowsServiceContract winSrv) 
        { 
            winSrv.OnStart(args); 
            Thread.Sleep(45000); 
            winSrv.OnStop();            
        } 
    } 
}
```

Bu static sınıf içerisindeki Run metodu IWindowsServiceContract arayüzünü implemente eden her hangibir tip üzerinden OnStart ve OnStop metodlarını çağırabilir ve böylece simülasyon işlemini başlatabilir. Geriye yapılması gereken tek bir işlem kalmaktadır. Windows Service projesindeki Main metodu içeriğini aşağıdaki kod parçasında görüldüğü gibi değiştirmek.

```csharp
using System; 
using System.Configuration; 
using System.ServiceProcess; 
using InvestigationLib;

namespace FileControllerService 
{ 
    static class Program 
    { 
        static void Main() 
        { 
            #region Debug Edilebilir Versiyon 
           
            DebugableControllerService implementation = new DebugableControllerService(); 
            if (Environment.UserInteractive) 
                ServiceStarter.Run( 
                    new string[]{ 
                        ConfigurationManager.AppSettings["Path"]                        
                    } 
                    , implementation); 
            else 
                ServiceBase.Run(new WindowsServiceCaller(implementation));

            #endregion 
        } 
    } 
}
```

Debug edilmek istenen kod içeriğini taşıyan DebugableControllerService örneğinin oluşturulmasından sonra ServiceStarter tipinin Run metodundan yararlanarak sürecin başlatılması sağlanır. Şu aşamada kod içerisinde breakpoint’ ler yardımıyla ilerlenmesi mümkündür

![Laughing out loud](/assets/images/2011/wlEmoticon-laughingoutloud.png)

Hatta örneğin olmayan bir klasör içeriğinin Directory tipinin GetFiles metodu yardımıyla okunmaya çalışılması sırasında oluşan Exception mesajları da, Visual Studio arabiriminin Output ekranına düşmektedir. Aynen aşağıdaki ekran çıktısında görüldüğü gibi.

[![blg231_DebugWindow](/assets/images/2011/blg231_DebugWindow_thumb.gif)](/assets/images/2011/blg231_DebugWindow.gif)

Ancak istenirse işlemlerin daha kolay takip edilmesi adına Console penceresine bilgi yazdırılması da mümkün olabilir. Bunun için Windows Service projesinin Output tipinin Console Application olarak değiştirilmesi yeterlidir.

[![blg232_ConsoleOutput](/assets/images/2011/blg232_ConsoleOutput_thumb.gif)](/assets/images/2011/blg232_ConsoleOutput.gif)

Buna göre Exception bloğunu aşağıdaki gibi değişitirsek debug işlemleri sırasında Console ekranına çıktı üretebilmeyi de sağlayabiliriz.

```csharp
void timer_Elapsed(object sender, ElapsedEventArgs e) 
{ 
    try 
    { 
        string[] files = Directory.GetFiles(path); 
        foreach (string file in files) 
        { 
            File.Encrypt(file); 
        } 
    } 
    catch (Exception excp) 
    { 
        Console.WriteLine(excp.Message); 
    } 
}
```

[![blg232_WriteToConsole](/assets/images/2011/blg232_WriteToConsole_thumb.gif)](/assets/images/2011/blg232_WriteToConsole.gif)

Elbette Debug işlemleri sonrasında hataları bertaraf edilen servis kodunun son hali asıl Windows Service kodu ile değiştirilmeli ve bu şekilde install edilmelidir. Örneğimiz bu haliyle artık Console penceresine bilgilendirme de bulunabilir. Ne Text dosyaya, ne işletim sistemindeki Event Log’ lara ne veritabanı üzerindeki ilgili tablolara loglama işlemleri yapmak için gerekli atraksiyonlar ile uğraşmamıza gerek kalmamaktadır. Uygulamanın dışına çıkmadan, olduğumuz Visual Studio ortamı (Environment) içerisinden gerekli izleme ve Debug işlemleri yapılabilir. Doğrudan Console penceresinden gerekli izlemeler de yapılabilecektir. Bunu da unutmayalım

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_2.png)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[FileControllerService.rar (51,95 kb)](/assets/files/2011/FileControllerService.rar)