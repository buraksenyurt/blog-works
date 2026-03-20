---
layout: post
title: "Windows Servislerine Giriş"
date: 2004-04-28 15:00:00 +0300
categories:
  - windows-services
tags:
  - windows-services
  - csharp
  - dotnet
  - threading
  - performance
  - transactions
  - visual-studio
---
Bu makalemizde windows servislerine kısa bir giriş yapıcak ve en basit haliye bir windows servisinin,.net ortamında nasıl oluşturulacağını incelemeye çalışacağız. Öncelikle Windows Service nedir, ne amaçlarla kullanılır bunu irdelemeye çalışak daha sonra windows servislerinin mimarisini kısaca inceleyeceğiz.

Windows servisleri, işletim sisteminde arka planda çalışan, kullanıcı ile etkilişimde bulunduğu herhangibir arayüze sahip olmayan, kaynakların izlenmesi, system olaylarının log olarak tutulması, network erişimlerinin izlenmesi, veritabanları üzerindeki transaction'ların izlenmesi, sistem performansına ati bilgilerin toplanması, sistem hatalarının (system exceptions), başarısız program denemelerin (failure) vb. gibi geri plan işlemlerinin takip edilmesinde kullanılan, sisteme kayıt edilmiş (register), çalıştırılabilir nesnelerdir.

Aslında, windows NT,2000,XP yada 2003 kullanıcısı iseniz, windows servisleri ile mutlaka ilgilenmişsinizdir. Sistemlerimizde çalışan pek çok servis vardır. Bu servislerin neler olduğuna, Administrative Tool bölümünde Services kısmından bakabiliriz. Örneğin aşağıda XP Professional sisteminde yüklü olan örnek servisler yer almaktadır.

![mk67_1.gif](/assets/images/2004/mk67_1.gif)

Şekil 1. Win XP için Örnek Windows Servisleri

İşte biz,.net sınıflarını kullanarak, burada yer alacak windows servisleri yazma imkanına sahibiz. Windows Servislerinin mimari yapısı aşağıdaki şekilde görüldüğü gibidir.

![mk67_2.gif](/assets/images/2004/mk67_2.gif)

Şekil 2. Windows Servis Mimarisi

Mimariden kısaca bahsetmek gerekirse; Service Application (Servis Uygulaması), istenilen fonksiyonelliklere sahip bir veya daha fazla windows servisini içeren bir uygulamadır. Servis Kontrol Uygulaması (Service Controller Application) ise, servislerin davranışlarını kontrol eden bir uygulamadır. Son olarak, SCM, sistemde yüklü olan servislerin kontrol edilmesini sağlayan bir windows aracıdır. Dolayısıyla biz, bir windows servis uygulaması yazarken, bunun içerisine birden fazla servis koyabiliriz. Bu servis uygulaması ve başka servis uygulamalarının davranışlarını servis kontrol uygulamaları yardımı ile kontrol edebiliriz. Diğer yandan, yazmış olduğumuz tüm servis uygulamaları ile birlikte sistemdeki servisleri, SCM aracılığıyla yönetebiliriz.

.Net Framework, windows servislerini oluşturabilmemiz için gerekli sınıfları içeren System.ServiceProcess isim alanına (namespace) sahiptir. Bu isim alanındaki sınıfları kullanarak, bir servisi oluşturabilir, sisteme yükleyebilir, yürütebilir ve kontrol edebiliriz. Aşağıdaki şekil, basit olarak, ServiceProcess isim alanındaki sınıflar ile yapabileceklerimizi temsil etmektedir.

![mk67_3.gif](/assets/images/2004/mk67_3.gif)

Şekil 3. System.ServiceProcess isim alanındaki sınıflar ile yapabileceklerimiz.

Buradaki sınıflar yardımı ile bir windows servisi oluşturmak istediğimizde izlememiz gereken bir yol vardır. Öncelikle, servisi oluşturmamız gerekir (Create). Bunun için ServiceBase sınıfını kullanırız. ServiceBase sınıfında yer alan metodlar yardımıyla, bir windows servisini oluşturabiliriz. Oluşturulan bu servisin daha sonra, kullanılabilmesi için sisteme install edilmesi ve register olması gerekmektedir. Bu noktada devreye ServiceInstaller ve ServiceProcessInstaller sınıfları girer. Bir windows servis uygulamasını install etmeden önce, bu servis için bir iş parçacığı (process) oluşturulmalı ve yüklenmelidir. İşte bu noktada devreye ServiceProcessInstaller girer. ServiceInstaller ve ServiceProcessInstaller sınıfları aslında bir servisin sisteme yüklenebilmesi için gerekli metodları otomatik olarak sağlarlar. Ancak bu sınıfların uygulandığı bir windows servis uygulamasının tam olarak sisteme yüklenmesi ve Services kısmında görülebilmesi için, InstallUtil isimli.net aracı kullanılır ve oluşturulan windows servis uygulaması sisteme yüklenir.

Sisteme yüklenmiş olan servislerin kontrol edilebilmesi amacıyla, ServiceController sınıfındaki metodları kullanabiliriz. Yani, bir servisin Start, Stop, Pause, Continue gibi dabranışlarını kontrol edebiliriz. Bu amaçla SCM aracını kullanabileceğimiz gibi, ServiceController sınıfındaki metodlarıda kullanabilir ve böylece herhangibir uygulamdan bir servisi başlatabilir, durdurabilir vb. işlemlerini gerçekleştirebiliriz.

Bir windows servisinin oluşturulması, sisteme yüklenmesi, yürütülmesi ve kontrol edilmesi her nekadar karışık görünsede, vs.net burada gereksinimin duyduğumuz işlemlerin çoğunu bizim için otomatik olarak yapmaktadır. Windows servislerinin oluşturulmasında kullanılan System.ServiceProcess isim alanının yanında, servislerin durumunun izlenebilmesi, çeşitli performans kriterlerinin servis içerisinden kontrol edilebilmesi, sisteme ait logların servis içerisinde kullanılabilmesi gibi işlemleri gerçekleştirebileceğimiz sınıfları içeren System.Diagnostics isim alanıda vardır.

![mk67_4.gif](/assets/images/2004/mk67_4.gif)

Şekil 4. System.Diagnostics isim alanı sınıfları.

Bir windows servis uygulaması ile normal bir vs.net uygulaması arasında belirgin farklılıklar vardır. Herşeyden önce bir windows servis uygulamasının kullanılabilmesi için, sisteme install edilmesi ve register olması gereklidir. Diğer önemli bir farkta, bir windows servis uygulaması arka planda çalışırken, bu servisin çalışması ile ilgili oluşabilecek hatalardır. Normal bir windows uygulamasında hatalar sistem tarafından kullanıcıya bir mesaj kutusu ile gösterilebilir yada program içerisindeki hata kontrol mekanizmaları sayesinde görsel olarak izlenebilir. Oysa bir windows servis uygulaması çalışırken meydana gelen hatalar, event log olarak sistemde tutulurlar.

Windows servisleri ile ilgili bu kısa bilgilerin ardından, dilerseniz birlikte basit bir windows servis uygulaması geliştirelim. Bunun için ilk yapmamız gereken vs.net ortamında, bir windows service applicaiton projesi açmaktır.

![mk67_5.gif](/assets/images/2004/mk67_5.gif)

Şekil 5. Windows Servis Uygulaması Açmak.

Bu işlemi gerçekleştirdiğimizde, vs.net herhangibir kullanıcı arayüzü olmayan bir proje oluşturacaktır.

![mk67_6.gif](/assets/images/2004/mk67_6.gif)

Şekil 6. Windows Servis Uygulaması ilk oluşturulduğunda ekranın görnümü.

Servis uygulamasını geliştirmek için yazacağımız kodlara geçmek için, to switch to code windows linkine tıklamamız gerekiyor. Bu durumda vs.net tarafından otomatik olarak oluşturulmuş kodları görürüz. İlk dikkat çekici nokta, Service1 isimli sınıfın ServiceBase sınıfından türetilmiş olmasıdır.

```csharp
public class Service1 : System.ServiceProcess.ServiceBase
```

ServiceBase sınıfı OnStart, OnStop, OnPause, OnContinue gibi bir takım metodlar içeren bir sınıftır. Bir windows servisi oluşturulurken bu servis sınıfının, ServiceBase sınıfından türetilmesinin sebebi, bahsetmiş olduğumuz metodların, servis sınıfı içerisinde override edilmeleridir. Bir servis SCM tarafından yada windows içinden başlatıldığında, servisin başlatıldığında dair start mesajı gelir. İşte bu noktada servisin OnStart metodundaki kodlar devreye girer. Aynı durum benzer şekilde, OnStop olayı içinde geçerlidir. Tüm bu metodlar ve başka üyeler temel sınıf olan ServiceBase sınıfı içerisinde toplanmıştır. Bunun sonucu olarak, bir sınıfı servis olarak tanımlamışsak, ServiceBase sınıfından türetir böylece servis başlatıldığında, durdurulduğunda vb. olaylarda yapılmasını istediğimiz işlemleri, türeyen sınıfta bu davranışların tetiklediği, OnStart, OnStop gibi metodları override ederek gerçekleştirebiliriz.

Sonuç itibariyle vs.net, bir servis uygulaması oluşturduğumuzda, buradaki varsayılan servisi, ServiceBase sınıfından türetir ve otomatik olarak OnStart ve OnStop metodlarını aşağıdaki şekilde ekler.

```csharp
protected override void OnStart(string[] args)
{
}

protected override void OnStop()
{
}
```

Peki bu metodlar ne zaman ve ne şekilde çalışır. Bir windows servisinin, normal windows uygulamalarında olduğu gibi desteklediği olaylar vardır. Windows servisleri 4 olayı destekler. Bir windows servisinde meydana gelen olayların çalışma şekli aşağıdaki şekildeki gibidir.

![mk67_7.gif](/assets/images/2004/mk67_7.gif)

Şekil 7. Servis Olaylarının Ele Alınış Mekanizması.

Buradan görüldüğü gibi, sistemde, yazmış olduğumuz windows servisi ile ilgili olarak 4 olay meydana gelebilir. Bunlar Start, Stop, Pause ve Continue olaylarıdır. Bir servis SCM tarafından başlatıldığında bu servis için Start olayı meydana gelir. Daha sonra SCM, servisin içinde bulunduğu Servis Uygulamasına Start komutunu gönderir. Buna karşılık servis uygulaması içindeki ilgili servis bu Start komutunu alır ve karşılığında, OnStart metodunda yazan kodları çalıştırır.

Diğer yandan bir servis durdurulduğunda, Stop olayı meydana gelir. Ancak bu noktada SCM Start tekniğinden farklı olarak hareket eder. SCM önce oluşan olayın sonucunda ilgili servis uygulaması içindeki servisin, CanStop özelliğine bakar. CanStop özelliği true veya false değer alabilen bir özelliktir ve servisin durdurulup durdurulamıyacağını dolayısıyla, bir Stop olayı meydana geldiğinde, servise ati OnStop metodunun çalıştırılıp çalıştırılamıyacağını belirtir. SCM bu nedenle Stop olayı meydana geldiğinde ilk olarak bu özelliği kontrol eder. Eğer özellik değeri true ise, servise Stop komutunu gönderir ve sonuç olarak OnStop metodundaki kodlar çalıştırılır.

Stop tekniğinde gerçekleşen özellik kontrol işlemi, Pause ve Continue olayları içinde geçerlidir. Bu kez SCM, servisin CanPauseAndContinue özelliğinin boolean değerine bakar. Eğer bu değer true ise, servise Pause komutunu veya Continue komutunu gönderir ve bunlara bağlı olan OnPause ve OnContinue metodlarındaki kodların çalıştırılmasını sağlar. Pause olayı bir servis duraklatıldığında oluşur. Continue olayı ise, pause konumunda olan bir servis tekrar çalışmasına devam etmeye başladığında oluşur.

Bir diğer önemli nokta oluşturulan service1 isimli sınıfın main metodundaki kodlardır.

```csharp
static void Main()
{
    System.ServiceProcess.ServiceBase[] ServicesToRun;
    ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service1() };
    System.ServiceProcess.ServiceBase.Run(ServicesToRun);
}
```

Burada görüldüğü gibi ServiceBase sınıfı tipinden bir nesne dizisi tanımlanmış ve varsayılan olarak bu nesne dizisinin ilk elemanı Service1 ismi ile oluşturulmuştur. Bu aslında bir servis uygulamasının birden fazla windows servisini içerebileceğinide göstermektedir. Burada ServiceBase sınıfı tipinden bir nesne dizisinin tanımlanmasının nedenide budur. Zaten oluşturduğumuz Service1 sınıfı, ServiceBase sınıfından türetildiği için ve sahip olduğu temel sınıf metodlarıda bu türeyen sınıf içerisinde override edildiği için, polimorfizmin bir gereği olarak, servis nesnelerini bir ServiceBase tipinden dizide tutmak son derece mantıklı ve kullanışlıdır.

Son satıra gelince. ServiceBase sınıfının Run metodu, parametre olarak aldığı serviseleri, SCM tarafından başlatılmış iseler, belleğe yüklemekle görevli bir metoddur. Elbette servislerin belleğe Run metodu ile yüklenebilmesi için, öncelikle Start komutu ile başlatılmaları gerekmektedir. ServiceBase.Run metodu aslında normal bir windows uygulamasındaki Application.Run metodu ile aynı işlevselliği gösterir.

Şu ana kadar oluşturduklarımız ile bir servisin omurgasını meydana çıkardık. Ancak halen servisimiz herhangibir işlem yapmamakta. Oysaki bir servis ile, örneğin, sistemdeki aygıt sürücülerine ait olay log'larını, düşük bellek kullanımı sonucu yazılım veya donanım ekipmanlarında meydana gelebilecek hata (error), istisna (exception), warning (uyarı) gibi bilgilere ait log'ları izleyebiliriz.

Şimdi dilerseniz ilk yazdığımız servis ile Uygulama Log'larının (Application Log) nasıl tutulduğunu incelemeye çalışalım. Aslında bizim izleyebileceğimiz üç tip log vardır. Bunlar, System Log'ları, Application Log'ları ve Security Log'larıdır.

![mk67_8.gif](/assets/images/2004/mk67_8.gif)

Şekil 8. Olay Log'larının Türleri.

Normal şartlar altında, vs.net ortamında bir windows servis ilk kez oluşturulduğunda, servise ait AutoLog özelliği değeri true olarak gelir. Bu durumda Start,Stop,Pause ve Continue olayları meydana geldiğinde, servis sisteme kurulduğunda veya sistemden kaldırıldığında, servis uygulamasına ait loglar, Application Log'a otomatik olarak yazılırlar. Biz bu uygulamamızda kendi özel loglarımızı tutmak istediğimizden bu özelliğin değerini false olarak belirliyoruz.

Kendi olay loglarımızı yazabilmemiz için, EventLog nesnelerine ihtiyacımız vardır. EventLog nesneleri System.Diagnostics isim alanında yer alan EventLog sınıfı ile temsil edilirler. Bu nedenle uygulamızda öncelikle bir EventLog nesne örneği oluşturmalıyız.

```csharp
private System.Diagnostics.EventLog OlayLog;
```

Bir EventLog nesnesi oluşturduktan sonra, bu nesne üzerinden CreateEventSource metodunu kullanarak servisimiz için bir event log oluşturabiliriz. Buna ilişkin kodları OnStart metodu içine yazarsak, servis başlatıldığında oluşturduğumuz log bilgilerininde otomatik olarak yazılmasını sağlamış oluruz.

```csharp
protected override void OnStart(string[] args)
{
    OlayLog=new EventLog(); /* EventLog nesnemizi olusturuyoruz.*/

    if(!System.Diagnostics.EventLog.SourceExists("Kaynak")) 
    {
        System.Diagnostics.EventLog.CreateEventSource("Kaynak","Log Deneme"); /* Ilk parametre ile, Log Deneme ismi altinda tutulacak Log bilgilerinin kaynak ismi belirleniyor. Daha sonra bu kaynak ismi OlayLog isimli nesnemizin Source özelligine ataniyor.*/
    }
    OlayLog.Source="Kaynak";
    OlayLog.WriteEntry("Servisimiz baslatildi...",EventLogEntryType.Information); /* Log olarak ilk parametrede belirtilen mesaj yazilir. Log'un tipi ise ikinci parametrede görüldügü gibi Information'dir.*/
}
```

Bu işlemlerin ardından servisimiz için basit bir görevide eklemiş olduk. Şimdi sırada bu servisin sisteme yüklenmesi var. Bunun için, daha önceden bahsettiğimiz gibi, ServiceProcess.ServiceInstaller ve ServiceProcess.ServiceProcessInstaller sınıflarını kullanmamız gerekiyor. Servis uygulamamız için bir tane ServiceProcessInstaller sınıfı nesne örneğine ve servis uygulaması içindeki her bir servis içinde ayrı ayrı olamak üzere birer ServiceInstaller nesne örneğine ihtiyacımız var. Bu nesne örneklerini yazmış olduğumuz servis uygulamasına kolayca ekleyebiliriz. Bunun için, servisimizin tasarım penceresinde sağ tuşa basıyor ve Add Installer seçeneğini tıklıyoruz.

![mk67_9.gif](/assets/images/2004/mk67_9.gif)

Şekil 9. Add Installer.

Bunun sonucu olarak vs.net, uygulamamıza gerekli installer sınıflarını yükler.

![mk67_10.gif](/assets/images/2004/mk67_10.gif)

Şekil 10. Installer nesne örneklerinin yüklenmesi.

Bunun sonucu olarak aşağıda görülen sınıf uygulamamıza otomatik olarak eklenir. Bu aşamada buradaki kodlar ile fazla ilgilenmiyeceğiz.

```csharp
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace OrnekServis
{
    [RunInstaller(true)]
    public class ProjectInstaller : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
        private System.ComponentModel.Container components = null;

        public ProjectInstaller()
        {
            InitializeComponent();
        }
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            this.serviceInstaller1.ServiceName = "Service1";
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {this.serviceProcessInstaller1,this.serviceInstaller1});

        }
    }
}
```

Ancak servisimizin, Sistemdeki service listesinde nasıl görüneceğini belirlemek için, ServiceInstaller nesne örneğinin, Display Name özelliğini değiştirebiliriz.

![mk67_12.gif](/assets/images/2004/mk67_12.gif)

Şekil 11. Servisimizin görünen adını değiştiriyoruz.

Diğer taraftan yapmamız gereken bir işlem daha var. ServiceProcessInstaller nesne örneğinin Account özelliğinin değerini belirlemeliyiz. Bu özellik aşağıdaki şekilde görülen değerlerden birisini alır.

![mk67_13.gif](/assets/images/2004/mk67_13.gif)

Şekil 12. Account Özelliğinin Alabileceği Değerler.

Biz bu uygulama için LocalSystem değerini veriyoruz. Bu, Servis Uygulamamızın sisteme yüklenirken, sistemdeki kullanıcıların özel haklara sahip olmasını gerektirmez. Dolayısıyla sisteme giren her kullanıcının servisi yükleme hakkı vardır. Eğer User seçeneğini kullanırsak, servisin sisteme yüklenebilmesi için geçerli bir kullanıcı adı ve parolanın girilmesi gerekmektedir.

Artık yazmış olduğumuz servis uygulamsı için installer'larıda oluşturduğumuza göre uygulamamız derleyip, sisteme InstallUtil aracı ile yükleyebilir ve register edebiliriz. Bunun için, servis uygulamamızın exe dosyası ile, InstallUtil aracını aşağıdaki gibi kullanırız.

![mk67_11.gif](/assets/images/2004/mk67_11.gif)

Şekil 13. InstallUtil aracı yardımıyla bir servisin sisteme yüklenmesi.

İşte InstallUtil aracı servisimizi sisteme yüklerken, servis uygulamamıza eklediğimiz ServiceProcessInstaller ve ServiceInstaller sınıflarını kullanır. Bu işlemin ardından vs.net ortamında, Server Explorer'dan servislere baktığımızda, AServis isimli servisimizin yüklendiğini ancak henüz çalıştırılmadığını görürüz.

![mk67_14.gif](/assets/images/2004/mk67_14.gif)

Şekil 14. Servis sisteme yüklendi.

Eğer servisin otomatik olarak başlatılmasını istiyorsak, ServiceInstaller nesnesinin StartType özelliğini Automatic olarak ayarlamamız yeterli olucaktır.

![mk67_15.gif](/assets/images/2004/mk67_15.gif)

Şekil 15. Servisin Başlatılma Şeklinin Belirlenmesi.

İstersek servisimizi, yine Server Explorer'dan servis adına sağ tıklayıp açılan menüden start komutuna basarak çalıştırabiliriz. Servisimizi çalıştırdıktan sonra, yine Server Explorer pencersinden Event Logs sekmesine bakarsak, bahsetmiş olduğumuz olay logları haricinde kendi yapmış olduğumuz olay logununda eklendiğini görürüz.

![mk67_16.gif](/assets/images/2004/mk67_16.gif)

Şekil 16. Servisin çalışmasının sonucu.

Burada görüldüğü gibi Log Deneme ismi ile belirttiğimiz Event Log oluşturulmuş, Source olarak belirttiğimiz Kaynak nesnesi oluşturulmuş ve OnStart metoduna yazdığımız kodlar yardımıyla, mesaj bilgimiz information (bilgi) olarak yazılmıştır. Buraya kadar anlattıklarımız ile bir windows servisinin nasıl yazıldığını ve sisteme yüklendiğini en temek hatları ile görmüş olduk. İlerleyen makalelerimizde, windows servisleri ile ilgili daha farklı uygulamlar geliştirmeye çalışacağız. Hepinize mutlu günler dilerim.