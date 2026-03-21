---
layout: post
title: "İlk Bakışta .Net Process Yönetimi"
date: 2006-07-07 12:00:00 +0300
categories:
  - csharp
tags:
  - Framework
  - process-management
  - clr
  - common-language-runtime
---
Bir Windows uygulamasını çalıştırdığımızda işletim sistemi bellek üzerinde söz konusu programın çalışabilmesi için bir Process açar. Bu Process içerisinde, uygulamanın çalışması için gerekli bellek ayırma işlemleri, harici olarak kullanılan Module'ler (örneğin başka Com nesneleri veya.net assembly'ları gibi) ve process içi Thread'ler bulunur. Çoğunlukla bir Process açıldığında bu Process içerisinde mutlaka bir main thread bulunur. Hatta basit bir Console uygulamasını çalıştırdığınızda Main Thread dışında başka aynı Proces'e dahil başka Thread'ler ile de karşılaşabiliriz. Dolayısıyla uygulama için açılan Process'in birden fazla Thread içereceği durumlar söz konusu olabilir. Literatürde multi-threading olarak geçen bu olay, bir Process'in içerisinde iş yapan eş zamanlı parçaların olması anlamına gelmektedir.

![mk166_1.gif](/assets/images/2006/mk166_1.gif)

Yukarıdaki şekilde bir Process'in çalışma zamanında sahip olabileceği içerik ifade edilmeye çalışmaktadır. Buradaki bilgilerden yola çıkarak bir Process'in en azından bir Thread içerdiğini, bellek üzerinde kullandığı ve ihtiyaç duyduğu bilgileri veri olarak paylaşımlı bir alanda tuttuğunu ve belleğe yüklenirken beraberinde ihtiyaç duyabileceği başka modulleride içerebileceğini söyleyebiliriz. Burada yer alan TLS (Thread Local Storage) kısmını, bir Thread'in kendisi için sonradan hatırlaması gereken bilgileri tuttuğu yer olarak düşünebiliriz. Bu saklama alanları sayesinde Thread'lerin bir önceki konumlarında sahip oldukları içeriğe erişebilmeleri ve kaldıkları yerden devam edebilmeleri mümkün olabilmektedir. İşte bu nedenle bu bilgiler özel olarak TLS (Thread Local Storage) alanında tutulurlar. Tek işlemcili bir sistem göz önüne alındığında TLS alanları son derece önemlidir. Ancak HyperThreading teknolojili veya birden fazla işlemciye sahip sistemlerde durum daha farklı olabilir. Konumuz Thread'lerin çalışmasını incelemek olmadığı için bu konuda çok fazla detaya girmeyeceğiz.

.Net Framework yukarıdaki şekilde görülen yapıdaki bilgileri çalışma zamanında elde edebilmemizi sağlayan bazı tipler (types) içerir. System.Diagnostics isim alanında bulunan bu tipler yardımıyla çalışma zamanda bir Process'e ait çeşitli bilgilerini elde edebiliriz. Diğer taraftan dilersek çalışma zamanında başka Process'leri çalıştırabilir, var olanları yok edebilir, bazı Process'lere parametrik bilgi aktarabilir ve hatta Process'lerden dış ortama sunulan bir takım stream'leri okuyabiliriz. Aşağıdaki tabloda Process API'sini kullanarak yapabileceklerimizin bazıları maddeler halinde belirtilmektedir.

Process ' ler ile ilgili Managed Code (Yönetimli Kod) Tarafında Yapabileceklerimizden Bazıları

1
Sistemde var olan Process'lerin bilgilerini alabiliriz. Örneğin Process'lerin adlarını, başladıkları süreleri, sistem tarafından kendilerine verilen Identity değerlerini vb. gibi.

2
Bir Process'i kod içerisinden çalıştırabiliriz. Bu işlemi yaparken çalıştırılacak Process için gerekli başlangıç bilgilerini söyleyebilir. (Start işlemi)

3
Bir Process'i çalıştırıp kendi içinden dış ortama sunduğu verileri başka Process'ler içerisinde yakalayabiliriz. Özellike Main metodu içerisinden dış ortama atılan stream'leri diğer Process'ler içerisinde yakalayabilmemiz mümkündür. (Yada tam dersi durumda söz konusudur.)

4
Bir Process'i çalıştırmadan önce söz konusu Process için gerekli parametreleri kod içerisinden gönderebiliriz. Örneğin internet explorer için bir Process açarken gezilecek URL bilgisini de parametre olarak verebiliriz.

5
Bir Process'in, çalışma zamanında içerdiği Thread'leri yakalayabilir ve bir takım bilgilerine ulaşabiliriz.

6
Bir Process'in belleğe açılması ile birlikte, söz konusu Process'e ilave olan ve çalışması sırasında kullanılan module bilgilerini elde edebiliriz.

7
Bir Process'i istersek çalışma zamanında sonlandırabiliriz. (Kill işlemi)

Burada bahsedilenleri dahada genişletebiliriz. Bizim için en önemli yardımcı tip Process isimli sınıftır. Process sınıfının çeşitli static metodları sayesinde o an çalışmakta olan tüm Process'leri elde edebiliriz; ayrıca belirli bir Process'i sistem tarafından verilen Identity değeri ile veya Process adı ile başlatabiliriz. Dilerseniz ilk örneğimiz ile işe başlayalım ve Process API'sini daha detaylı bir şekilde öğrenmeye çalışalım. İlk olarak sistem de yer alan Process'leri ve bunlara ait bir takım bilgileri nasıl elde edebileceğimize bakacağız. Bunun için aşağıdaki kod parçasını herhangibir Console uygulamasında denememiz yeterli olacaktır.

```csharp
private static void ProcessInfos()
{
    Process[] currentProcesses = Process.GetProcesses("manchester");
    Console.WriteLine("Process Count {0}", currentProcesses.Length.ToString());
    foreach (Process pro in currentProcesses)
    {
        try
        {
            Console.WriteLine("{0,6}, {1,20}, Başlangıç : {2,6}, Thread Sayısı {3,5}", pro.Id.ToString(), pro.ProcessName, pro.StartTime.ToShortTimeString(), pro.Threads.Count.ToString());
        }
        catch 
        {
            continue;
        }
    }
}
```

Herhangibir sistemde o anda çalışan Process'leri elde edebilmek için Process sınıfının static GetProcesses metodunu kullanırız. Örneğimizde bu metoda parametre olarak yerel makine adını (local machine name) vermekteyiz. GetProcesses metodu geriye Process tipinden bir dizi döndürmektedir. Bu dizinin herbir elemanı Process tipinden olduğu için güncel Process'lere ait çeşitli bilgileri elde edebiliriz. Tabi elde ettiğimiz bu bilgiler ilgili Process'lere ait anlık verilerdir.

Dolayısıyla Process'lerin son durumu hakkındaki bilgileri elde edebilmek için sürekli kontrol işlemi gerektirecek kodlar yazılması gerekebilir.(Bu tip bir düzenlemede Polling benzeri bir mantık söz konusudur.) Biz örneğimizde basit olarak her bir Process'in sistem tarafından verilen Identity değerini, adını, başladığı zamanı ve içerdiği Thread sayısını ekrana yazdırmaktayız. (Process sınıfını ve üyelerini kullanırken System.Diagnostics isim alanını uygulamamıza eklemeyi unutmamamlıyız.) Metodumuzu test ettiğimiz örnek bir Console uygulamasının çıktısı aşağıdakine benzer olacaktır.

![mk166_2.gif](/assets/images/2006/mk166_2.gif)

Process sınıfının başka static metodlarıda vardır. Örneğin GetCurrentProcess metodu o an çalışmakta olan uygulamaya ait Process bilgisini elde etmenizi sağlar. GetProcessById static metodu sistemdeki Identity değerine göre bir Process nesne örneği üretir. GetProcessesByName static metodu bir Process adını parametre olarak alarak, o anda bu isim ile açık olan kaç tane Process varsa bunların hepsini Process sınıfı tipinden bir dizi olarak geri döndürür. Son olarak static Start metodu ile, çalışma zamanında bir Process içerisinden başka Process'leri çalıştırabiliriz.

```csharp
Process current = Process.GetCurrentProcess();
Console.WriteLine("{0,15} {1,15}","Id : ",current.Id.ToString());
Console.WriteLine("{0,15} {1,15}", "Start Time : ", current.StartTime.ToShortTimeString());
Console.WriteLine("{0,15} {1,15}","Process Name : ", current.ProcessName);
Console.WriteLine("{0,15} {1,15}", "Machine Name : ", current.MachineName);
```

![mk166_3.gif](/assets/images/2006/mk166_3.gif)

Yukarıdaki kod parçasında o anda çalışmakta olan uygulamaya ait Process bilgilerinden bazıları ekrana yazdırılmaktadır. Burada örnek olarak bir kaç Process bilgisini okuyoruz. Bazı durumlarda çalışma zamanında herhangibir uygulama adını ele alaraktan söz konusu program için açık olan Process'ler hakkında bilgi almak isteyebiliriz. (Örneğin sistemde açık olan Visual Studio.Net IDE'lerinin tamamı hakkında bilgi almak gibi.) Bu durumda Process sınıfının static GetProcessesByName metodunu kullanırız. Bu metod geriye Process sınıfı tipinden bir dizi döndürür. Metodun geriye dizi döndürmesinin sebebi, çalışma zamanında bir uygulamaya ait birden fazla Process'in açılmış olabileceğidir. Aşağıdaki kod parçası basit olarak çalışma zamanında Windows işletim sisteminin bilinen programlarından olan Notepad uygulamasına ait Process bilgilerini vermektedir. Kod içerisinde, elde edilen dizinin uzunluğuna bakılarak söz konusu uygulamaya ait açık Process'lerin olup olmadığının tespiti kolayca yapılabilir. Bu kontrolün yapılmasının sebebi açık Process olmaması halinde, null referans olacağından uygulamanın istem dışı bir biçimde sonlanabilecek olmasıdır.

```csharp
Process[] prcNotePad = Process.GetProcessesByName("Notepad");
if (prcNotePad.Length > 0)
{
    for (int i = 0; i < prcNotePad.Length; i++)
    {
        Console.WriteLine("{0,15} {1,15}", "Id : ", prcNotePad[i].Id.ToString());
        Console.WriteLine("{0,15} {1,15}", "Start Time : ", prcNotePad[i].StartTime.ToShortTimeString());
        Console.WriteLine("{0,15} {1,15}", "Process Name : ", prcNotePad[i].ProcessName);
        Console.WriteLine("{0,15} {1,15}", "Machine Name : ", prcNotePad[i].MachineName);
        Console.WriteLine("----------------");
    }
}
```

![mk166_4.gif](/assets/images/2006/mk166_4.gif)

Sıradaki örneğimizde her hangibir.Net kodu içerisinden başka Process'leri nasıl çalıştırabileceğimizi göreceğimiz aşağıdaki kod parçasını ele alacağız. Kod içerisinden başka Process'ler başlatabilmek için Process sınıfının static Start metodunu kullanırız.

```csharp
// Bilinen bir Windows uygulaması için parametre destekli Process başlatılır. Bir internet explorer uygulamasının alabileceği parametrelerden birisi Url bilgisidir.
Process.Start("iexplore", "http://www.bsenyurt.com");

// Start metodu parametre olarak ProcessStartInfo tipinden bir nesne örneğide alabilir. Aşağıdaki kod satırı Windows ' un Calculator uygulamasını çalıştırmakta ve bir Process açmaktadır.
ProcessStartInfo startInfo = new ProcessStartInfo("calc");
Process.Start(startInfo);

// Herhangibir uygulamanın path bilgisinden yararlanarak, söz konusu uygulama için bir Process başlatılabilir.
Process.Start(@"D:\HaftaIci\ExceptionHandling\ExceptionHandling\bin\Debug\ExceptionHandling.exe");
```

Dikkat ederseniz bu kod parçasındaki örneklerde farklı şekillerde Process başlatma işlemleri ele alınmıştır. Çoğunlukla Windows işletim sistemini göz önüne aldığınızda komut satırından veya Start menüsünde yer alan Run seçeneği ile doğrudan çalıştırabileceğiniz uygulamalar vardır. Bu nedenle aynı uygulama adlarını exe uzantılı veya uzantısız olabilecek şekilde kod içerisinde yazabilir ve ayrı Process'leri başlatabiliriz. Start metodunun yukarıda kullanılan versiyonları dışında ele alabileceğiniz farklı aşırı yüklenmiş versiyonlarıda vardır. Örneğin aşağıdaki versiyonda dikkat ederseniz Process'i açmak için UserName, Domain ve Password bilgileri gerekmektedir. Buna göre bir uygulamayı gerekli yetkileri bildirerektende başlatabiliriz.

![mk166_5.gif](/assets/images/2006/mk166_5.gif)

Makalemizin giriş kısmında çalışma zamanındaki Process'lerin beraberlerinde yükledikleri module'ler olabileceğinden bahsetmiştik. Sonuç itibariye bir.Net uygulamasının varsayılan hali belleğe yüklendiği zaman, işletim sistemi tarafından ilgili Process'e dahil edilen bazı ek module'ler söz konusu olabilir. Bu Module'ler çoğunlukla Com nesneleri, C tabanlı sistem kütüphaneleri olabileceği gib başka.Net kütüphaneleride (dll'ler) olabilir. Aşağıdaki kod parçası parametre olarak verilen herhangibir uygulamanın, işletim sistemi tarafından açılan Process'ine dahil olan Module'lerine ilişkin bir takım bilgiler vermektedir.

```csharp
private static void LookingModules(Process currentProc)
{
    ProcessModuleCollection modules = currentProc.Modules;
    int totalSize = 0;
    foreach (ProcessModule mdl in modules)
    {
        Console.WriteLine("{0,25} : {1,15} bytes", mdl.ModuleName, mdl.ModuleMemorySize.ToString());
        totalSize += mdl.ModuleMemorySize;
    }
    Console.WriteLine(totalSize.ToString() + " bytes");
    Console.ReadLine();
}
```

![mk166_6.gif](/assets/images/2006/mk166_6.gif)

Biz örneğimizde, var olan uygulamaya ait Process nesne örneğini ele aldık. Buna göre Process'imize dahil olan diğer Module'lerin isimlerini ve bellekte kapladıkları alanları görebiliriz. Dikkat ederseniz Process sınıfının Modules isimli özelliği geriye her bir elemanı ProcessModule sınıfı tipinden olan ProcessModuleCollection türünden bir koleksiyon örneği döndürmektedir. ProcessModule tipini kullanarak bir Process içerisine dahil olan herhangibir Module hakkında çalışma zamanı bilgilerine ulaşabiliriz.

Çalışma zamanında Process'leri başlatabileceğimiz gibi onları yok etmek isteyebiliriz de. Bunun için tek yapmamız gereken Process sınıfına ait Kill metodunu kullanmak olacaktır. Aşağıdaki kod parçası ile çalışma zamanında sistemde var olan tüm Internet Explorer uygulamaları (doğal olarak Process'leri) sonlandırılmaktadır. Bu işlemi gerçekleştirmek için iexplore adını içeren Process'lerin listesi GetProcessesByName metodu ile çekilmiş ve elde edilen dizideki her bir Process tipi nesne örneği için Kill metodu çağırılmıştır. Her zamanki gibi söz konusu uygulama için sistemde açık Process'ler olup olmadığını kontrol etmek ve Kill işlemini bir try...catch bloğu altında güvenli olarak gerçekleştirmek çalışma zamanında oluşabilecek istenmeyen hataların önüne geçilmesinde önemli bir rol oynayacaktır.

```csharp
Process[] prcs = Process.GetProcessesByName("iexplore");
if (prcs.Length > 0)
{
    for (int i = 0; i < prcs.Length; i++)
    {
        try
        {
            prcs[i].Kill();
        }
        catch{}
    }
}
```

Bazen bir Process içerisinden, başka Process'lerce dış ortama aktarılan verileri okumak isteyebiliriz. Çoğunlukla string bazlı stream'lerin bu şekilde dış ortama aktarılması halinde, Process API'sini kullanarak her hangibir uygulama içerisinden bu çıktıları yakalayabiliriz. Tabiki böyle bir ihtiyacın hangi durumlarda doğabileceğinide düşünmek lazım. Aşağıdaki örnekte, Support.exe adlı Console uygulaması dış ortama bir string mesaj vermektedir. Bunun için tipik olarak Console sınıfının WriteLine metodu kullanılmıştır.(Consolo.out'ta kullanılabilinir.) Örnek kod parçamız ise, Support.exe adlı Console uygulamasını çalıştırıp bir Process içerisine dahil etmekte ve Support.exe uygulamasının dış ortama verdiği sonuçları bir Stream dahilinde kendi Process'i içerisine almaktadır.

```csharp
ProcessStartInfo strInfo = new ProcessStartInfo(@"D:\Vs2005Projects\C# 2.0\Support\Support\bin\Debug\Support.exe");
strInfo.UseShellExecute = false;
strInfo.RedirectStandardOutput = true;
Process pro=Process.Start(strInfo);
StreamReader reader = pro.StandardOutput;

//string okunan;
//while((okunan=reader.ReadLine())!=null)
//{
// Console.WriteLine(okunan);
//}

Console.WriteLine(reader.ReadToEnd());

Console.ReadLine();
pro.WaitForExit();
```

Kodumuzda ilk olarak ProcessStartInfo tipinden bir nesne örneği oluşturuyor ve ilgili Process için bir takım özellikleri belirliyoruz. Özellikle ProcessStartInfo'nun işaret ettiği Support.exe isimli uygulamadan dış ortama aktarılan stream'i okuyabilmek için RedirectStandartOutput özelliğine true değerini atamış olmamız gerekmektedir. Bundan sonrasında ise Process'i başlatıyor ve StandartOutput özelliğini kullanarak bir stream yakalıyoruz. Yakaladığımız stream üzerinden gelen veriyi kod içerisinde değişik şekillerde okuyabiliriz.

Kodumuzda bu işin iki farklı yapılış yolunu görüyorsunuz. Bir tanesi satır satır okuma işlemini diğeri ise tüm string bilgiyi okuma işlemini gerçekleştiriyor. Kodumuzun sonunda dikkat ederseniz WaitForExit metodunu çağırıyoruz. Process sınıfının WaitForExit isimli metodunu kullanmamızın amacı ise; ilgili harici uygulamanın sonlanmasını beklemek. Böylece harici Process'sonlandırılmadan, asıl uygulamamızın sonlandırılmamasını sağlamış oluyoruz. Diğer Process içerisinde çalışacak olan Console uygulamasına ait Main metodu kodu ise aşağıdaki gibidir.

```csharp
static void Main(string[] args)
{
    Console.WriteLine("Diğer Process' den Merhabalar."); 
}
```

Uygulamanın çalıştırılması sonucu;

![mk166_7.gif](/assets/images/2006/mk166_7.gif)

Bir Process'den başka bir Process'e stream bazlı veri çekmek dışından istenirse çalıştırılacak Process'e uygulama içerisinde yine stream bazlı veri aktarımıda gerçekleştirilebiliriz. Bu durumda Process'in StandardInput özelliğini ele almamız gerekecektir. StandardInput özelliği, StandardOutput özelliğinin tersine stream içerisine veri yazabilmek için bir StreamWriter nesne örneğini döndürür.

Son olarak bir Process'e dahil olan Thread'lerin nasıl ele alınabileceğini basit bir örnek ile incelemeye çalışacağız. Aşağıdaki kod parçasında Visual Studio.Net 2005 ortamına ait çalışma zamanı thread'lerine ilişkin bazı bilgilerin nasıl elde edilebileceğimizi gösteren örnek bir kod parçası bulunmaktadır.

```csharp
Process[] currProc = Process.GetProcessesByName("devenv");
if (currProc.Length > 0)
{
    ProcessThreadCollection currThreads = currProc[0].Threads;
    foreach (ProcessThread trd in currThreads)
    {
        Console.WriteLine("{0,7} {1,15} {2,10} {3,15} ", trd.Id.ToString(), trd.PriorityLevel.ToString(), trd.StartTime.ToString(),trd.ThreadState.ToString());
    }
}
```

![mk166_8.gif](/assets/images/2006/mk166_8.gif)

Bir Process'in içerisinde çalışan Thread'leri elde edebilmek için Process sınıfına ait nesne örneği üzerinden Threads özelliği çağırılır. Threads özelliği geriye ProcessThreadCollection tipinden (bu tip güvenli bir koleksiyondur) bir nesne örneği döndürür. Her bir elemanı ProcessThread sınıfı tipinden olan bu koleksiyon yardımıyla bir Process içerisinde yer alan tüm Thread'leri elde edebiliriz. Örnek kodumuzda, elde edilen her bir Thread'in Id (Sistem tarafından verilen identity değeri), PriorityLevel (öncelik seviyesi), StartTime (Başlangıç zamanı) ve ThreadState (Thread'in o anki durumu) bilgileri çekilmektedir.

Görüldüğü gibi.Net uygulamaları çalıştırıldıklarında işletim sistemi tarafından açılan Process'ler söz konusudur. Biz bu Process'leri managed (yönetimli) taraftan kontrol edebiliriz. Gerekli Process bilgilerinin çalışma zamanında elde edilebilmesi, istendiğinde başka Process'lerin parametrik olarak başlatılabilmesi (ya da sonlandırılabilmesi) gibi imkanlara sahibiz. Hatta çok basit seviyede de olsa Process'ler arası veri transferide yapabilmekteyiz. Bu makalemizde çok kısada olsa Process'lerin ne olduğunu ve managed (yönetimli) tarafta nasıl ele alınabileceklerini incelemeye çalıştık ve böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](/assets/files/2006/ProcessTest.rar)
