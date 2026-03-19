---
title: "İlk Bakışta Windows Workflow Foundation"
pubDate: 2008-01-01 12:00:00
categories:
  - WF
tags:
  - WF
  - C#
  - .NET
  - WCF
  - Workflow Foundation
  - XML
  - Web Service
  - HTTP
  - Threading
  - Transactions
  - Serialization
  - Delegates
  - Generics
  - Visual Studio
---

# İlk Bakışta Windows Workflow Foundation
Değerli Okurlarım Merhabalar,

Gerçek dünyada pek çok iş probleminin çözümünde iş akışlarından (Workflow) yararlanılır. Temelde bir iş probleminin çözümünde veya amacının gerçekleştirilmesinde izlenen yol birdir. Önce problem yönetilebilir küçük parçalara bölünür. Bu parçalar, gerçekleştirilmesi gereken görevler (Tasks) olarak düşünülebilir. Her bir görevin (Task) içerisinde ona ait gerçekleştirilmesi gereken ne varsa adımlar (Steps) halinde tasarlanır. Bu adımlar dahil oldukları görevin tamamlanmasında rol oynarlar. Adımlar arasındaki geçişler basit olabileceği gibi çeşitli çevresel koşul veya faktörlerede bağımlı olabilir. Bir başka deyişle adımlar arası geçişlerde koşullar (Conditions) söz konusu olabilir. Adımlar düzenli bir sırada olup aralarındaki geçişler önceden tanımlanmış ve belirli olabileceği gibi, çeşitli olaylara göre farklı şekillerde ele alınabilirlerde. Sonuç olarak ortaya iş probleminin çözümü için tasarlanmış bir süreç (Process) ve kontrollü bir akış (Control Flow) çıkar.

İş akışları (Workflow) sayesinde, iş problemlerinin çözümlenmesi, istenirse genişletilebilmesi son derece kolay bir şekilde gerçekleştirilebilmektedir. Bir iş akışına pek çok yerde kolaylıkla rastlayabiliriz. Bunların bir kısmı için hiç bir geliştirme yapılamamakla birlikte çoğu için bilgisayar teknolojisinden yararlanılmaktadır. Bir başka deyişle bazı iş çevrelerinde izlenen süreçler bilinçsiz olarak kendiliğinden bir akışa sahip olabilmektedir. Ancak bilgisayar teknolojisinin hızla yaygınlaşması ve verimliliği arttırması nedeni ile, iş akışları yazılımsal ve donanımsal faktörler üzerinden ele alınmaktadır. Bu anlamda iş akışlarının (Workflow) ilk uygulamaları dökümanların bir noktadan başka bir noktaya taşınması olmuştur. Zaten buda SharePoint Server gibi gelişmiş bir sistemde iş akışlarının neden kullanıldığını açıklamaktadır. Nitekim portal tarzı uygulamalarda döküman yönetimi (Document Management) ve paylaşımı esastır. Bu noktada, dökümanların kullanıcılar (Users) ve sistemler (Systems) arasındaki hareketinde çeşitli onay mekanizmalarının devreye girmesi muhtemeldir. Buda çok doğal olarak bir iş akışı ile ifade edilip tanımlanabilir.

Bir kaç yıl önce çalıştığım özel bir yazılım firmasında şirketlerin iş akışlarının (Workflow) tasarlanabildiği bir yazılım projesinde görev almıştım. İş çevrelerinin çözüm bekleyen çok fazla sayıda problemi vardır. Söz gelimi bir elemanın işe alım yada işten çıkartılma süreçleri, şirketin mali yapısını gösteren raporların onay mekanizmaları, üretim hattına ait süreçler ve daha pek çoğu. Bilgisayar ve yazılımlar sayesinde bu örnek süreç ve benzelerine ait iş akışlarının kullanılması son derece kolaylaşmaktadır. Örneğin işe alım sürecini ele alalım. Elemanın CV'sinin insan kaynakları departmanına verilmesi ve bilgisayar ortamına alınması, departman içerisinde CV'nin ilgili mercilere gönderilerek onaylarının alınması, onay verilmesi halinde işe alınmak istenen personelin görüşmeye çağırılması, görüşme sonrası tüm bilgilerin kayıt altına alınarak sürece dahil olan diğer kişilerede gönderilmesi, gönderme işlemlerinde mail sisteminden yararlanılması gibi işlemler söz konusu olacaktır. Bu işlemlerin her biri arasındaki geçişler karar yapıları ve onay mekanizmaları ile gerçekleşmektedir. Söz gelimi CV'nin ilk değerlendirmesinde en üst mercinin red etmesi halinde iş başvurusunda bulunan kişiye olumsuz cevap verilmesi ve süreç içerisindeki ilgili kişilerede bu durumun mail, sms gibi yollarla aktarılması için bir onay mekanizması ve koşullandırmanın olması gerekmektedir.

Dikkat edilecek olursa iş akışılarının yukarıdaki gibi cümlesel olarak ifadesi anlaşılmasını zorlaştırmaktadır. Hatta bu sürecin kapsadığı alana (Domain) dahil olan kişilerin bilgisayarlarında adımları kolayca izleyebilmesi ve kimin üstüne hangi görev (Task) düşüyorsa bunu yapabilmesi demek aslında bir yazılım sisteminin geliştirilip kurulması anlamına gelmektedir. Buda doğal olarak yukarıdakinden daha uzun ve karmaşık olan iş akışı anlatımlarının önce kağıt üzerinde grafiksel olarak tasarlanması ve sonrasında gerekli yazılımın hazırlanması anlamına gelmektedir. Geliştiriciler (Developers) bu tip iş akışlarını sistemlere uygularken görsel tasarlayıcılarıda (Visual Designer) ele alırlar. Bir başka deyişle iş akışlarının görsel olarak tasarlanabilmeside önemlidir. Bu amaçla geliştirilmiş pek çok yazılım (Software) söz konusudur.

> Karmaşık iş kurallarını içeren akışların görsel olarak ele alınması, akışın içerisinde yer alan adım (Step) ve kuralların (Rule) kolayca tasarlanabilmesi hatta kodlanabilmesi demektir. Bu bir iş akışının sonradan kolayca değiştirilebilmesi bir başka deyişle genişletilebilmesininde kolaylaştırılması anlamına gelmektedir.

Bu yazılımların genel amacı, pek çok iş akışının çoğunlukla birden fazla bilgisayarın olduğu sistemlerde kurulması ve kullanılabilmesidir. Elbette birden fazla bilgisayar olması şart değildir. Bazı durumlarda tek bir bilgisayar üzerindeki programlar için söz konusu olabilecek iş akışlarıda var olabilir. (Aslında Windows Workflow Foundation mimarisinin bu modeldeki iş akışlarına daha yakın olduğunu düşünebiliriz.)

Bu kısa bilgilerden sonra bir iş akışını tanımlamak çok daha kolaylaşmaktadır. Bir iş akışı (Workflow) herhangibir iş probleminin çözümü için gereken adımları (Steps), onay mekanizmalarını ve karar yapılarını (Condition) içeren bir model sunmaktadır. Bir başka deyişle bir iş akışı, belirli kurallar (Rules) üzerine sıralanmış adımlar topluluğu olarakta düşünülebilir. Özellikle bilgisayar teknolojileri üzerinden baktığımızda bir iş akışının aslında farklı bir programlama modeli sunduğuda göz önüne alınabilir. Bu açılardan bakıldığında iş akışı denildiğine akla gelen pek çok kavramda bulunmaktadır. Bu kavramlar aşağıdaki maddelerde belirtildiği gibidir;

- Görsel Şemalar
- Kurallar (Rules)
- Politikalar (Policies)
- Sisteme giren (Input) ve çıkan (Output) veriler
- Kişiler (Users)
- Organizasyonlar (Organizations)
- Yordamlar (Procedures)
- Temel Görevle (Tasks)
- Adımlar (Steps)
- Aktiviteler (Activities)

Peki Windows Workflow Foundation ile kastedilen nedir? Microsoft bu Foundation ile iş akışlarının tasarlandığı bir programmı üretmiştir? Aslında Windows Workflow Foundation tek bir iş alanı (Single Domain) içerisinde yer alan tek bir uygulamayı (Single Application) hedeflemektedir. Bir başka deyişle Windows Workflow Foundation.Net uygulamalarının kullanabileceği iş akışlarının (Workflow) tasarlanması için gerekli altyapıyı sunan bir Framework 3.0 yaklaşımıdır.

> İş akışları çoğunlukla BizTalk Server'un sundukları ile karşılaştırılır. BizTalk ile özellikle elektronik ticarete uygun olacak şekilde farklı platformlar üzerinde yer alan sistemlere ait iş süreçleri başarılı bir şekilde ele alınabilmektedir. Oysaki Windows Workflow Foundation sadece işletim sistemi seviyesinde (Operating System Level) düşünülmüştür.
> Bu anlamda Microsoft otoriteleri BizTalk'un interapplication (Birden fazla uygulama-Mutliple Applications) olarak ele alınması gerektiğini, Windows Workflow Foundation'ın ise intraapplication (Tek bir uygulama-Single Application) şeklinde düşünülmesi gerektiğini vurgulamaktadır. Ancak bu bir kısıt değildir. Nitekim WWF içerisinde Web Servisleri gibi SOA (Service Oriented Architecture) modelleri sayesinde dış platformlara çıkılması ve iş sürecinin bu şekilde genişletilmeside mümkündür.

Windows Workflow Foundation mimarisi sayesinde iş akışları görsel olarak tasarlanıp kodlanabilirler. Ancak tasarlanan bu iş akışlarının işe yarayabilmesi için bir uygulama tarafından ele alınmaları şarttır. Söz konusu uygulamalar host görevini üstlenmekte olup bir veya daha fazla iş akışını barındırıp kullanabilirler. Windows Workflow Foundation mimarisi pek çok fayda sağlamaktadır. Söz konusu faydalar aşağıdaki maddeler ile özetlenebilir.

- İş akışlarının kolayca ve etkili bir biçimde tasarlanabilmesi için görsel tasarımcı (Visual Designer) sunar.
- Sistemin insan ile etkileşimde olduğu modellerde aktiviteler (adımlar) arasındaki süre farkları olabilir. Bu nedenle iş akışının güncel durumlarının kaydedilebiliyor ve daha sonra sonra başka bir zaman diliminde tekrar yüklenebiliyor olması gerekir. WWF bunu sağlamaktadır.
- İş akışları için gerekli çalışma zamanı (Run-Time) ortamı dışında, pek çok tipte farklı aktivite (Activity) için destek, aktivitelerin denetlenmesi (Monitoring), izlenmesi (Tracing) sağlanmaktadır.
- Sequential iş akışı desteği vardır. Bu akışlar çoğunlukla sistem etkileşimi olan bir başka deyişle insan faktörü fazla bulunmayan durumlarda söz konusudur. Bu tip akışlarda adımların sırası ve düzeneği bellidir.
- State Machine iş akışı desteği vardır. Bu tip akışlarda insan etkileşimi söz konusudur. Çoğunlukla aktivitiler arasındaki geçişlerin bazı olayların tetiklenmesine bağlı olduğu durumlarda kullanılır.

Windows Workflow Foundation, iş akışlarını esas alan bir programlama modeli sunmaktadır. Söz konusu programlama modeli deklaratif (declarative) yaklaşımı ele almaktadır. Buna göre iş mantığı ayrık bileşenler içerisinde kapsüllenir (encapsulation). Nitekim bileşenler arasında akışların nasıl yölendirileceğini belirten kurallar deklaratif olarak tanımlanabilirler.

Windows Workflow Foundation mimarisinin nasıl kullanıldığını daha net kavrayabilmek için basit örnekler üzerinden devam etmekte yarar var. İlk örnekte son derece basit bir iş akışını tasarlayıp bunu kullanacak olan bir Console uygulaması geliştiriyor olacağız. Sonrasında ise iş akışını farklı.Net uygulamalarında da kullanabilmek adına bir sınıf kütüphanesi (Class Library) içerisinde tasarlayıp örnek bir Windows uygulaması içerisinden çalıştıracağız. Daha öncedende belirtildiği gibi WWF içerisinde tasarlanmış bir iş akışının (Workflow) işe yarayabilmesi için bir host uygulama tarafından ele alınması gerekmektedir. İlk örneğimizde Host program Console uygulaması olarak tasarlanacaktır. Örneklerimizi Visual Studio 2008 RTM sürümü üzerinde tasarlıyor olacağız. Ancak istenirse gerekli genişletmeler yüklenerek Visual Studio 2005 ilede WWF geliştirmeleri yapılabilir. Elbette.Net Framework 3.0' ın yüklü olması şarttır.

İlk olarak aşağıdaki ekran görüntüsünde olduğu gibi Workflow sekmesinde yer alan proje şablonlarından (Project Templates) birisinin seçilmesi gerekmektedir.

![mk237_1.gif](images/mk237_1.gif)

Dikkat edilecek olursa Sequential ve State Machine iş akışlarının (Workflow) geliştirilmesi için gerekli proje şablonları (Project Template) bulunmaktadır. Her iki tip içinde birer sınıf kütüphanesi (Class Library) ve Console uygulaması şablonu yer almaktadır. Çok doğal olarak geliştirilen iş akışlarının farklı tipte.Net uygulamalarında kullanılacağı durumlar söz konusu olduğunda Workflow Library şablonlarını uygulamak daha doğru bir yaklaşım olacaktır. Windows Workflow Foundation ile geliştirilen iş akışlarında aktivitelerin (Activity) büyük önemi vardır. Geliştiriciler isterlerse kendi özel aktivite tiplerinide (Custom Activity Types) yazabilirler. Bunun içinde Workflow Activity Library şablonu kullanılır.

Makalemizdeki ilk örneğimizde sonuçları hemen irdeleyebilmek adına Sequential Workflow Console Application tipinden bir uygulama yazıyor olacağız. Buna ilişkin proje şablonu seçildikten sonra uygulama içerisine aşağıdaki ekran görüntüsünde olduğu gibi Workflow1 isimli bir iş akışı tipinin eklendiği görülür.

![mk237_2.gif](images/mk237_2.gif)

İş akışı içerisine örnek aktiviteleri eklemeden önce proje içerisinde oluşturulan tiplerden bahsetmekte yarar vardır. Herşeyden önce bir iş akışı projesi oluşturulduğunda System.Workflow.Activities, System.Workflow.ComponentModel ve System.Workflow.Runtime isimli assembly'ların referans edildiği görülür.

![mk237_4.gif](images/mk237_4.gif)

Tahmin edileceği gibi bu assembly'lar içerisinde iş akışlarının geliştirilmesi, çalıştırılması, denetlenmesi ve izlenmesi için gerekli temel tipler yer almaktadır. Bunların dışında console uygulamasına dahil edilmiş farklı assembly referanslarıda bulunmaktadır. Söz gelimi iş akışı içerisinde transaction desteğini sağlamak için System.Transactions, akış içerisinden web servisleri ile iletişime geçebilmek için System.Web, System.Web.Services gibi assembly'ları örnek olarak gösterebiliriz.

Proje içerisine varsayılan olarak atılan Workflow1 isimli sınıfın (Class) hiyerarşik yapısı ise aşağıdaki sınıf diyagramında (Class Diagram) olduğu gibidir.

![mk237_3.gif](images/mk237_3.gif)

Örnek Sequential Workflow Console Application olduğundan, içeride kullanılacak varsayılan iş akışıda Sequential Workflow olarak ele alınmaktadır. Bu sebepten dolayı sealed (kendisinden türetme yapılamaz) olarak tanımlanmış Workflow1 isimli sınıf ilk etapta SequentialWorkflowActivity sınıfından türemektedir. Ancak enteresan olan bir nokta vardır. Örnek geliştirilirken kullanılacak olan standart pek çok akitivite bileşeninin Activity isimli sınıfdan türediği görülecektir. Dikkate değer olan, aktiviteleri (adımları-Steps) içeren Workflow tipinin kendisininde aslında bir aktivite nesnesi olmasıdır. (Sınıf diyagramında pek çok tip yer almaktadır. Bu tiplerin detaylarını ilerleyen makalelerimizde inceleme fırsatı bulacağız.)

Örneğimizdeki iş akışının içeriğini geliştirmeden önce Program.cs dosyası içerisindeki kod parçalarına kısaca bakalım. Şu aşamada Workflow1 tipine ait iş akışını (Workflow) veya başkalarını çalıştıracak ve yürütücek olan kısım Main metodunun içeriğidir.

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;

namespace MerhabaWWF
{
    class Program
    {
        static void Main(string[] args)
        {
            using(WorkflowRuntime workflowRuntime = new WorkflowRuntime())
            {
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                workflowRuntime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) {waitHandle.Set();};
                workflowRuntime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e)
                                                                            {
                                                                                Console.WriteLine(e.Exception.Message);
                                                                                waitHandle.Set();
                                                                            };

                WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof(MerhabaWWF.Workflow1));
                instance.Start();

                waitHandle.WaitOne();
            }
        }
    }
}
```

Şimdi Main metodu içerisinde neler yapıldığına kısaca bir bakalım. WorkflowRuntime sınıfı iş akışlarının devreye sokulması, bunların çalışması için gerekli ortamın hazırlanması, çalışma zamanı iş akışlarının izlenmesi, denetlenmesi gibi işlemleri üstlenen önemli bir sınıftır. Pek çok önemli olayı (Event) vardır. Bunlardan örnekte hazır olarak sunulan WorkflowCompleted olayı iş akışının tamamlanması sonrasında devreye girmektedir. Bir iş akışı işlemleri tamamlandıktan sonra geriye değer döndürebilir.

Bu sebepten dolayı WorkflowCompletedEventArgs tipinden olan olay parametresinin OutputParameters özelliğinden yararlanılarak WorkflowCompleted olay metodu içerisinde sonuç alınması sağlanabilir.Tahmin edileceği gibi WorkflowTerminated olayı, herhangibir hata nedeni ile (çoğunlukla bir istisna-exception) iş akışı tamamlanamadığında devreye girmektedir. Bu olayla ilişkili olan WorkflowTerminatedEventArgs sınıfı üzerinden hareket edilerek oluşan istisna (Exception) referansı çalışma zamanında (runtime) yakalanabilir. Dikkat edilecek olursa Visual Studio tarafından otomatik olarak üretilen bu kod parçasında olaylara ait fonksiyonelliklerin hazırlanmasında isimsiz metodlardan (anonymous methods) yararlanılmaktadır.

Using bloğu içerisinde oluşturulan örneklerden bir diğeride AutoResetEvent sınıfına ait nesnedir. Bu sınıf temel olarak thread senkronizasyonu yönetimi amacıyla tasarlanmıştır. Host uygulamanın kendi içerisinde bir veya daha çok iş akışını tetiklemesi sonrasında askıda kalması istenen bir durum değildir. Nitekim bir iş akışı (Workflow) çalıştırıldığında Workflow çalışma zamanı (Runtime) bunu host uygulamadaki ana thread'in dışında ayrı bir thread içerisine alacaktır. AutoResetEvent sınıfının Set metodu bu tip bir durumda kalındığında bekleyen thread'in serbest bırakılmasını sağlamaktadır. Dikkat edilecek olursa Set metodu hem WorkflowCompleted hemde WorkflowTerminated olay metoduları içerisinde çağırılmakta ve bekleyen thread'in serbest bırakılması sağlanmaktadır.

Main metodu içerisinde örneklenen diğer bir nesne sınıfıda WorkflowInstance'dır. Bu sınıfta sealed olarak tanımlanmıştır. Bir başka deyişle kendisinden türetme yapılamamaktadır. Aynı zamanda yapıcı metodları (Constructor) erişimide yoktur. Kendisi ancak WorkflowRuntime sınıfının static CreateWorkflow metodu ile örneklenebilmektedir. Bu metodun aşırı yüklenmiş (Overload) farklı versiyonları bulunmaktadır. Özellikle iş akışına dışarıdan parametre değerleri aktarılabilmesini sağlayan versiyonu vardır ki bu önemlidir. Nitekim pek çok iş akışının başlangıcında bir takım dış parametre bilgilerine ihtiyaç vardır. Diğer taraftan bir XOML (eXtensible Object Markup Language) dosyasında tanımlı herhangibir iş akışının yüklenmesini sağlayan versiyonuda bulunmaktadır.

Main metodunun sonunda AutoResetEvent sınıfına ait nesne örneği üzerinden WaitOne fonksiyonu çağırılır. Bu çağrı, çalışan ana thread'in bekleyen iş akışları tamamlanıncaya kadar duraksatılması için önemlidir. Nitekim iş akışının içerisindeki adımların gerçekleştirilme sürelerinin belirsiz olması ihtimali vardır. WaitOne metodu ilerlenip ilerlenmeyeceğine, diğer thread'lerden gelen sinyallere göre karar vermektedir. Tahmin edileceği gibi örnekte yer alan söz konusu sinyal Set metodu ile yayınlanmaktadır.

Artık iş akışı içerisine örnek bir adım (Step) ekleyerek devam edebiliriz. Daha önceden de belirtildiği gibi adımlar aslında birer aktivite (Activity) olarak düşünülebilirler. WWF mimarisi çok sayıda hazır aktivite sunmaktadır. Bu aktivite bileşenlerine iş akışına ait tasarım penceresindeylen ToolBox kısmından da erişilebilir. Var olan tüm aktivite bileşenleri System.Workflow.ComponentModel isim alanında (Namespace) yer alan Activity sınıfından (Class) türemektedir. Hazır aktivite bileşenlerine örnek olarak IfElseActivity, CodeActivity, CallExternalMethodActivity, DelayActivity, EventDrivenActivity, ReplicatorActivity, ParallelActivity, TerminateActivity ve daha pek çoğu verilebilir.

Biz örneğimizde basit olması açısından CodeActivity ve IfElseActivity bileşenlerini kullanıyor olacağız. CodeActivity bileşeni sayesinde iş akışının herhangibir adımında çalıştırılması istenen kodlar ele alınabilmektedir. Temel olarak bileşenin ExecuteCode özelliğine atanan değerin işaret ettiği metod, aktiviteye gelindiğinde çalıştırılacak olan kodları içermektedir. IfElseActivity bileşeni yardımıyla bir iş akışının herhangibir noktasına karar yapıları eklenebilmektedir. IfElseActivity bileşeni kendi içerisinde birden fazla IfElseBranchActivity örneği içerebilmektedir. Bu IfElseBranchActivity örneklerinin her biri karar yapısı içerisindeki farklı dallanmaları ifade etmektedir.

Dilerseniz örnek üzerinden devam ederek iş akışını tamamlamaya çalışalım. Örnek senaryoda bir ürünün stoktaki miktarına göre çalışacak bir iş akışı tasarlayacağız. Söz gelimi stok miktarının belirli bir değerin altına inmesi halinde çalışacak bir iş akışı söz konusu olabilir. Hatta farklı aralık değerlerine göre farklı dallanmalar yapılmasıda sağlanabilir. (Bu noktada iş akışının ne kadar anlamlı olduğu çok önemli değildir. Nitekim hedeflenen, temel bir iş akışının WWF altında geliştirilmesi ve kullanılmasıdır.) İlk olarak tasarım zamanında iken Workflow1 üzerine bir IfElseActivity bileşenini ToolBox'tan sürükleyerek bırakalım. Bırakılma işleminden sonra ilk etapta aşağıdaki görüntü ortaya çıkacaktır.

![mk237_5.gif](images/mk237_5.gif)

Dikkat edileceği gibi IfElseActivity içerisine varsayılan olarak iki adet IfElseBranchActivity bileşeni daha eklenmiştir. Bu noktada soldaki IfElseBranchActivity bileşeninin belirtilen koşul true olduğunda çalıştırılması, diğerinin ise false durumuna karşılık olarak değerlendirilmesi doğru bir yaklaşımdır. Her bir IfElseBranchActivity kendi içerisinde başka aktivitelerin tetiklenmesinede neden olmaktadır. Drop Activites Here kısmına bu amaçla çeşitli aktivite bileşenleri (Activity Components) bırakılabilir. IfElseBranchActivity'lerin en önemli üyesi aşağıdaki ekran görüntüsündende görülebileceği gibi Condition özelliğidir (Property).

![mk237_6.gif](images/mk237_6.gif)

Condition özelliğine Code Condition ve Declarative Rule Condition olmak üzere iki farklı değer verilebilir. Peki bunlar ne anlama gelmektedir? Code Condition değerinin atanması halinde koşulun bir metod ile değerlendirileceği belirtilir. Söz konusu metod bool bir değerlendirme yapmalıdır. Declarative Rule Condition seçeneği sayesinde ise, koşul kodun dışında ayrı bir şekilde ele alınır. Bu çeşit bir kullanım çalışma zamanında (Run-Time) yeniden derleme gerektirmeden koşul değişikliği yapma imkanı sunmaktadır. Örneğimizde ilk olarak Declarative Rule Condition seçeneğini inceliyor olacağız. Bu kriter seçildikten sonra Condition özelliği altına iki alt üyenin daha eklendiği görülecektir.

![mk237_7.gif](images/mk237_7.gif)

Bunlardan ConditionName koşulun adını ifade ederken, Expression ise koşula ait ifadeyi içermektedir. ConditionName özelliğine sembolik olarak Miktar50Altinda verdiğimizi düşünelim. Bundan sonra Expression özelliği yanındaki üç nokta düğmesine tıklarsak aşağıdaki arabirim ile karşılaşırız.

![mk237_8.gif](images/mk237_8.gif)

Bu ekranda geriye true veya false döndürebilecek şekilde bir koşul tanımlaması yapılmaktadır. Dikkat edilecek olursa metin kutusu içeriğinde intellisense desteği vardır. Ancak bu noktada iş akışının doğurduğu önemli bir ihtiyaçta ortaya çıkmaktadır. Bir şekilde iş akışına, ürüne ait stok miktarı değerinin aktarılması gerekmektedir. Lakin koşulun değerlendirmesi gereken durum stok miktarının belirli bir değerin altında olması halinde yapılacak işlemler ile ilgilidir. Dolayısıyla iş akışına dışarıdan paramete aktarılması gerekmektedir.

> Bir iş akışına aktarılacak olan parametreler herhangibir.Net CLR (Common Language Runtime) tipi olabilir. Bu önemli bir avantajdır, nitekim bir iş akışı (Workflow) önceden tanımlı veya geliştirici tarafından yazılmış herhangibir tip verisi ile başlatılabilir. İş akışlarının herhangibir.Net CLR tipini parametre olarak alabilmesinde object tipi etkin bir rol oynar. Elbetteki bir iş akışına birden fazla parametrede gönderilebilmektedir.

Bunun yapmanın yolu ise son derece basittir. Nitekim Workflow tipi aslında bir sınıftır. Dolayısıyla Workflow tipine özellikler (Property) ekleyerek dış ortamdan parametre aktarımı sağlanabilir. Bu nedenle örnekte yer alan Workflow1 sınıfına aşağıdaki gibi bir özelliğin ilave edilmesi yeterlidir.

```csharp
public sealed partial class Workflow1: SequentialWorkflowActivity
{
    private int _stokMiktari;

    public int StokMiktari
    {
        get { return _stokMiktari; }
        set { _stokMiktari = value; }
    }

    public Workflow1()
    {
        InitializeComponent();
    }
}
```

Artık tasarım tarafına dönülerek ilk IfElseBranchActivity için gerekli koşul aşağıdaki ekran görüntüsündeki gibi oluşturulabilir.

![mk237_9.gif](images/mk237_9.gif)

Buna göre StokMiktari özelliğinin (Property) değerinin 10 ile 50 arasındaki olması halinde, bu IfElseBranchActitiy bileşenin arkasından gelecek olan aktivite çalıştırılacaktır. Bu noktada ne yapılmak istendiği önemlidir. Söz gelimi bu koşulun sağlanması halinde üreticiye yeni ürün talepleri için mail veya Sms gönderilmesi gibi işlemler yaptırılabilir. Yine çok basit olarak düşünerek hareket edelim ve mail gönderme işleminin yapılacağı kod bloğunu işaret edecek bir CodeActivity bileşenini aşağıdaki ekran görüntüsünde olduğu gibi tasarım ortamına atalım.

![mk237_10.gif](images/mk237_10.gif)

CodeActivity bileşeninin en önemli üyesi ExecuteCode özelliğidir. Bu özelliğe atanacak olan isim, bu adımda çalıştırılacak olan metodun adıdır. Bu olay metodunun otomatik olarak oluşuturulması sağlanabilir. Bunun için ExecuteCode özelliğine bir metod adı yazılması ve enter'a basılması yeterlidir. Örnekteki ExecuteCode özelliğine MailGonder ismini yazıp enter tuşuna bastığımızda Workflow1 sınıfına aşağıdaki metodun otomatik olarak eklendiği görülecektir.

```csharp
private void MailGonder(object sender, EventArgs e)
{
}
```

Görüldüğü gibi üretilen metod stadart bir olay metodu yapısındadır. Geriye değer döndürmemekte ve iki adet parametre almaktadır. İş mantığında bu adımda işletilmesi gereken kodlar nelerse bu metod içerisine yazılmalıdır. Söz gelimi bu adımda mail gönderme işlemi yaptırılabilir.

IfElseActivity içerisinde sağ tarafta kalan ikinci bir IfElseBranchActivity bileşeni daha vardır. Şu anki senaryoda bu bileşen else olma durumunda ne olacağını belirtmektedir. Diğer taraftan örnek senaryoda başka IfElseBranchActivity bileşenlerinin eklenmeside mümkündür. Örneğin Stok miktarının herhangibir nedenle 0 ve altında olması hali ve Stok Miktarının 50' nin üzerinde olması hali gibi. Bu durumların her biri için birer IfElseBranchActivity kontrolü eklenip gerekli aksiyonların gerçekleştirilmesi sağlanabilir.

> IfElseActivity bileşenleri içerisine IfElseBranchActivity bileşenlerini eklemek için sağ tıklayıp Add Branch demek yeterlidir.
> ![mk237_11.gif](images/mk237_11.gif)

Bu amaçla örneğimize aşağıdaki şekildede görüldüğü gibi başka IfElseBranchActivity bileşenleri daha eklediğimizi düşünelim.

![mk237_13.gif](images/mk237_13.gif)

Burada ikinci IfElseBranchActivity içerisinde StokMiktari özelliğinin değerinin 0' ın altında ve eşit olduğu durumda çalıştırılacak bir kod aktivitesi yer almaktadır. 3ncü IfElseBranchActivity içerisinde StokMiktari özelliğinin değerinin 50 ile 500 arasında olduğu durumda çalışacak bir aktivite bulunur. Son IfElseBranchActivity parçasında ise var olan koşulların dışındaki durum ele alınmaktadır. Genellikle birden fazla IfElseBranchActivity içeren durumlarda tüm ihtimallerin dışında kalabilecek bir seçeneğide ele almak için Condition özelliği herhangibir şekilde atanmamış boş bir IfElseBranchActivity parçası kullanmakta yarar vardır. Örnekte bu tarz bir durum için yine kod aktivitesi yürütülmektedir. Elbette her CodeActivity bileşenin ExecuteCode özelliğine ilgili değerlerin atanması ve oluşan olay metodlarının kodlanması gerekmektedir. Şimdilik bu kısım bizim açımızdan önemli değildir. Nitekim örneğe son olarak yapılan eklemelerde kavranması gereken birden fazla IfElseBranchActivity bileşenin bir arada ele alınabilmesidir.

> IfElseBranchActivity işlemlerinde karar mekanizması olarak Declarative Rule Condition kullanılması halinde kuralların rules uzantılı bir XML dosyası içerisine yazıldığı görülür. Aşağıdaki ekran görüntüsünde örnekte kullanılan IfElseBranchActivity'ler için oluşturulan Workflow1.rules dosyasının sadece bir kısmı görülmektedir.
> ![mk237_14.gif](images/mk237_14.gif)

Şimdi iş akışı için daha fazla önem arz eden bir konu üzerinde durulmalıdır. İş akışına ilgili parametreler nasıl aktarılacaktır? Bu amaçla Main metodu içerisinde yer alan kod parçalarında bazı değişiklikler yapılması gerekmektedir. Daha öncedende belirtildiği gibi WorkflowRuntime sınıfına ait CreateInstance metodunun ikinci parametresi iş akışlarına değer göndermek için kullanılmaktadır. İş akışları herhangibir.Net CLR tipini parametre olarak aldığından ve dış ortamdan iş akışına gönderilen değerin hangi özelliğe aktarıldığının bilinmesi gerektiğinden generic Dictionary koleksiyonu kullanılmaktadır. Böylece ilgili iş akışının hangi özelliğine, hangi değerin aktarılacağı belirtilebilir. Bu aynı zamanda iş akışına birden fazla parametre değeri gönderilebilmesi anlamınada gelmektedir.

Örnekte kullanılan StokMiktari özelliğinin (Property) değeri iş akışına dış ortamdan, örneğin Host uygulama içerisinden gelmektedir. Bu nedenle Main metodu içerisinde aşağıdaki değişikliklerin yapılması gerekir.

```csharp
Dictionary<string, object> parametreler = new Dictionary<string, object>();
parametreler.Add("StokMiktari", 45);

WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof(MerhabaWWF.Workflow1),parametreler);
```

İlk olarak parametre veya parametreleri taşıyacak olan Dictionary koleksiyonu örneklenir. Bu generic koleksiyonun anahtarları (Keys) string, değerleri (Values) ise object türünden olmalıdır. Daha sonra Add metodu ile parametre ekleme işlemi gerçekleştirilir. Örnekte StokMiktari isimi özellik için 45 değerinin verileceği belirtilmektedir. Son olarak WorkflowInstance örneği oluşturulurken CreateWorkflow metodunun ikinci parametresine, koleksiyona ait nesne örneği atanır. Burada dikkat edilmesi gereken bazı noktalarda vardır. Herşeyden önce Dictionary koleksiyonuna eklenen parametre adının, iş akışı (Workflow) sınıfı içerisinde tanımlanan özellik adı (Property Name) ile bire bir uygun olması gerekmektedir. Söz gelimi Add metodunda StokMiktari yerine stokmiktari yazılırsa aşağıdaki ekran görüntüsünde olduğu gibi çalışma zamanında ArgumentException istisnası (Exception) alınır.

![mk237_15.gif](images/mk237_15.gif)

Bu noktadan sonra iş akışı başarılı bir şekilde çalışacaktır. Örnek olarak tasarlanan iş akışının herhangibir amacı ve başarısı yoktur ancak temel kavramların nasıl uygulanacağını göstermektedir.

Makalemize IfElseBranchActivity bileşenlerinde Condition özelliğinde Code Condition seçeneğini nasıl kullanacağımızı inceleyerek devam edelim. Bu amaçla örnek olarak herhangibir IfElseBranchActivity'nin Condition özelliğine Code Condition değerini atmamız yeterli olacaktır.

![mk237_16.gif](images/mk237_16.gif)

Burada Condition altında yer alan Condition özelliğinede bir metod adı verilmesi gerekmektedir. Örneğin Stok10ile50Arasindami ismini verip enter tuşuna bastığımızı düşünelim. Bunun sonucu olarak workflow1.cs içerisine aşağıdaki metodun eklendiği görülecektir.

```csharp
private void Stok10ile50Arasindami(object sender, ConditionalEventArgs e)
{

}
```

Metodun bool tipinde bir değerlendirme yapması gerekmektedir. Buna karşın dikkat edileceği üzere void tipinde oluşturulmuştur. İşte bu noktada koşulun sonucunu geriye döndürmek için ConditionalEventArgs tipinden olan parametrenin, Result özelliğinden yararlanılır. Result özelliği bool tipindendir ve koşula göre true veya false değerini almalıdır. Buna göre kod aşağıdaki şekilde düzenlenebilir.

```csharp
private void Stok10ile50Arasindami(object sender, ConditionalEventArgs e)
{
    if (StokMiktari >= 10 && StokMiktari <= 50)
        e.Result = true;
    else
        e.Result = false;
}
```

Eğer StokMiktari özelliğinin değeri 10 ile 50 arasında ise bu koşul sağlanmış demektir. Bu durumda Result özelliğine true değeri atanmalıdır. Eğer true değeri atanırsa IfElseBranchActivity'den sonra gelen aktivitenin çalıştırılması da sağlanmış olur. Elbetteki buradaki kod parçasında, koşulun sağlanmış olma veya olmama haline göre sıradaki aktiviteye geçilmeden önce farklı işlemler yapıtırılmasıda sağlanabilir.

Makalemizin son bölümünde iş akışını (Workflow) bir sınıf kütüphanesi (Class Library) olarak nasıl tasarlayabileceğimizi ve bunu örneğin bir windows uygulamasında nasıl host edebileceğimizi incelemeye çalışacağız. Bu sefer proje şablonu olarak Sequenatial Workflow Library modelini seçmemiz gerekiyor. Oluşan sınıf kütüphanesi (Class Library) içerisinde yine standart olarak Workflow1 isimli bir iş akışı nesnesi bulunur. Örnek olarak bir metin dosyasının içerisinde parametre olarak verilen bir kelimenin bulunması veya bulunmaması halinde yapılabilecek bazı işlemler olduğunu ve bunun bir iş süreci olarak göz önüne alındığını düşünelim.

Söz konusu sürecin birden fazla.Net uygulaması içerisinde ele alınabileceğini düşünürsek iş akışının sınıf kütüphanesi olarak tasarlanması son derece mantıklıdır. İş akışının bu anlamda dışarıdan alması gereken iki adet parametre bulunmaktadır. Bunlardan birisi dosya adresini, diğeri ise aranacak bilgiyi tutmalıdır. Diğer taraftan iş akışından Host eden uygulamayada bilgi gönderilmesi istenebilir. Söz gelimi arama sonuçlarına dair bir string bilgi söz konusu olabilir. Doğal olarak 3ncü bir özelliğe daha gerek vardır. Bu nedenle workflow1 sınıfı içerisine aşağıdaki kod parçasında yer alan özelliklerin (Properties) eklenmesi gerekmektedir.

```csharp
public sealed partial class Workflow1: SequentialWorkflowActivity
{
    private string _dosyaAdresi;
    private string _arananKelime;
     private string _aramaSonucu;

    public string AramaSonucu
    {
        get { return _aramaSonucu; }
        set { _aramaSonucu = value; }
    }

    public string ArananKelime
    {
        get { return _arananKelime; }
        set { _arananKelime = value; }
    }

    public string DosyaAdresi
    {
        get { return _dosyaAdresi; }
        set { _dosyaAdresi = value; }
    }

    public Workflow1()
    {
        InitializeComponent();
    }
}
```

İlk örneğimizdeki gibi bir IfElseActivity ve iki adet IfElseBranchActivity ekliyoruz. Burada dosya içerisinde bir bilgi arama işlemi yapılmak istendiğinden koşulun kod yardımıyla kontrol edilmesi, bu nedenle Code Condition seçeneğinin kullanılması daha mantıklıdır. Hatalara çok fazla takılmamak adına sadece txt uzantılı dosyaları ele aldığımızı düşünelim. IfElseBranchActivity1 bileşeninin Condition özelliğinin değerini Code Condition olarak ayarladıktan sonra alt Condition özelliğinede ArananKelimeVarmi bilgisini yazalim. Bu IfElseBranchActivity1 için çalışacak koşul kontrol metodunun adı olacaktır. IfElseBranchActivity2 bileşeni için herhangibir Condition ataması yapılmasına gerek yoktur. Nitekim otomatik olarak else durumunun değerlendirileceği yerdir. Her iki aktivitenin arkasından çalıştırılmak istenen kodların yer aldığı CodeActivity bileşenlerinide ekleyelim. Sonuçta iş akışının tasarım zamanındaki görüntüsü aşağıdaki gibi olacaktır.

![mk237_17.gif](images/mk237_17.gif)

CodeActivity1 için ArananBilgiVar isimli bir metod, CodeActivity2 bileşeni içinde ArananBilgiYok isimli bir metod devreye girecektir. Buna göre Workflow1 sınıfının başlangıçtaki içeriği aşağıdaki gibidir.

```csharp
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.IO;

namespace StokAkislari
{
    public sealed partial class Workflow1: SequentialWorkflowActivity
    {
        private string _dosyaAdresi;
        private string _arananKelime;
        private string _aramaSonucu;

        public string AramaSonucu
        {
            get { return _aramaSonucu; }
            set { _aramaSonucu = value; }
        }

        public string ArananKelime
        {
            get { return _arananKelime; }
            set { _arananKelime = value; }
        }

        public string DosyaAdresi
        {
            get { return _dosyaAdresi; }
            set { _dosyaAdresi = value; }
        }

        public Workflow1()
        {
            InitializeComponent();
        }

        // IfElseBranchActivity1 bileşenine ait koşul kontrol metodu
        private void ArananKelimeVarmi(object sender, ConditionalEventArgs e)
        {
            if (File.Exists(DosyaAdresi))
            {
                StreamReader reader = new StreamReader(DosyaAdresi); 
                e.Result = reader.ReadToEnd().Contains(ArananKelime);
            }
            else 
                e.Result = false;
        }

        // CodeActivity1 için çalışacak olay metodu
        private void ArananBilgiVar(object sender, EventArgs e)
        {
            // Aranan bilgi bulunduğunda yapılacak işlemler
            AramaSonucu = ArananKelime + " " + DosyaAdresi + " içinde bulunmuştur";
        }

        // CodeActivity2 için çalışacak olay metodu
        private void ArananBilgiYok(object sender, EventArgs e)
        {
            // Aranan bilgi bulunamadığında yapılacak işlemler
            AramaSonucu = ArananKelime + " " + DosyaAdresi + " içinde bulunamamıştır";
        }
    }
}
```

Amacımız her zamanki gibi konuyu basitçe kavramak olduğundan CodeActivity bileşenleri ile ilişkili kod parçaları şimdilik göz ardı edilmiştir. Artık host uygulamayı yazarak devam edebiliriz. Bu amaçla basit bir Windows uygulaması tasarlayacağız. Uygulamamız text tabanlı dosyaların seçimine ve aranacak kelimenin girilmesine izin verecek bir arabirime sahip olacaktır. Buradaki asıl hedefimiz ise bir.Net uygulaması içerisinden herhangibir iş akışının (Workflow) nasıl çalıştırılabileceğini görmektir. Windows uygulamasında tahim edileceği gibi bazı Workflow assembly'larının ve iş akışlarını taşıyan kütüphanenin eklenmiş olması gerekmektedir. Sonuç itibariyle windows uygulamasında aşağıdaki şekilde görülen referansların dahil edilmesiyle işe başlanmalıdır.

![mk237_19.gif](images/mk237_19.gif)

Bu noktadan sonra Windows uygulamasının aşağıdaki gibi tasarlandığını düşünebiliriz. Kullanıcı Dosya Seç başlıklı düğmeye bastığında açılacak iletişim kutusu ile txt uzantılı dosya seçebilecektir. Aranacak kelime bilgiside girildikten sonra sürecin başlatılması için tek yapılması gereken Süreci Başlat başlıklı düğmeye basmak olacaktır.

![mk237_18.gif](images/mk237_18.gif)

Dosya seçme işlemi için OpenFileDialog bileşeni kullanılamktadır. Sadece Text tabanlı dosyalar ele alınmak istendiğinden Filter özelliğine Text Files|.txt değeri atanmıştır. Uygulamanın kod içeriği ise aşağıdaki gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.Runtime;
using System.Threading;
using StokAkislari;

namespace Istemci
{
    public partial class Form1 : Form
    {
        private WorkflowRuntime _wfRunTime;
        private WorkflowInstance _wfInstance;
        // Workflow thread' lerinin yönetimi için AutoResetEvent nesnesi kullanılır
        private AutoResetEvent _arEvent = new AutoResetEvent(false);

        public Form1()
        {
            InitializeComponent();

            // Workflow çalışma zamanı nesnesi örneklenir
            _wfRunTime = new WorkflowRuntime();
        
            // Workflow tamamlandığında devreye girecek olay metodu yüklenir
            _wfRunTime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e)
                                                                    {
                                                                        MessageBox.Show(e.OutputParameters["AramaSonucu"].ToString());
                                                                        _arEvent.Set();
                                                                    };

            // Workflow' un çalışması sırasında bir istisna oluştuğunda devreye girecek olay metodu yüklenir.
            _wfRunTime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e)
                                                                    {
                                                                        MessageBox.Show(e.Exception.Message);
                                                                        _arEvent.Set();
                                                                    };
        }

        private void btnDosyaSec_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDosyaAdresi.Text = openFileDialog1.FileName;
            }
        }

        private void btnSureciBaslat_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtArananKelime.Text)
                        && !String.IsNullOrEmpty(txtDosyaAdresi.Text))
                {
                    // parametrelerin gönderilmesi için Dictionary koleksiyonu örneklenir
                    Dictionary<string, object> parametreler = new Dictionary<string, object>();

                    // Workflow1 içerisindeki özelliklerin alacağı değerler set edilir.
                    parametreler.Add("DosyaAdresi", txtDosyaAdresi.Text);
                    parametreler.Add("ArananKelime", txtArananKelime.Text);
    
                    // Workflow1 için bir örnek oluşturulur.
                    _wfInstance = _wfRunTime.CreateWorkflow(typeof(Workflow1), parametreler);
    
                    // Workflow1 başlatılır.
                    _wfInstance.Start();
    
                    _arEvent.WaitOne();
                }
                else
                    MessageBox.Show("Verilerde eksik var");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}
```

Geliştirilen örnekte form nesnesi üretilirken iş akışı çalışma ortamı (Workflow Run-Time) için gerekli ayarlar yapılmaktadır. Bu amaçla WorkflowRuntime nesnesi örneklenmekte, WorkflowCompleted, WorkflowTerminated gibi olaylar isimsiz metodlar (Anonymous Methods) aracılığıyla yüklenmektedir. Bir önceki örnekten farklı olarak dikkat edilmesi gereken nokta WorkflowCompleted olay metodu içerisinde WorkflowCompletedEventArgs sınıfına ait OutputParameters özelliğinin kullanılışıdır. Bu özellikte Dictionary tipinden generic bir koleksiyonu ele almaktadır. Özelliğin amacı, iş akışından uygulama ortamına değer aktarımını sağlamaktır. Bu amaçla indeksleyici (Indexer) operatörü içerisinde, iş akışında tanımlanan AramaSonucu özelliğinin adı verilmektedir.

Doğal olarak sürecin bir şekilde başlatılması gerekmektedir. Bu amaçla WorflowInstance örneği oluşturulduktan sonra, Start metodu çağırılmaktadır. Tüm bu başlatma işlemi ise Form üzerindeki bir Button kontrolüne ait Click olay metodu içerisinde gerçekleştirilmektedir. Örnek test edildiğinde bir döküman içerisinde aranan bilginin var olup olmadığına dair sonuçların alındığı görülecektir. (Tahmin edileceği üzere bu tarz bir ihtiyaç normal bir sınıf kütüphanesi içerisine alınacak bir tip ilede, iş akışlarına gerek olmadan tasarlanabilir. Ancak gerçek iş problemleri göz önüne alındığında tek başına yeterli bir çözüm olmayacaktır.)

Buraya kadar yazdıklarımız ile iş akışı (Workflow) kavramını, Windows Workflow Foundation yaklaşımını incelemeye çalıştık. Giriş niteliğindeki bu makalemizde, bir iş akışı için gerçek hayat senaryoları kullanmamış olsakta, WWF ile nasıl geliştirilebileceklerini, herhangibir.Net uygulamasından nasıl kullanılabileceklerini gördük. Bundan sonraki makalelerimizde WWF mimarisinin başka konularınıda incelemeye çalışıyor olacağız. Makalemize son vermeden önce Windows Workflow Foundation ile ilgili kaynak kitaplar hakkında bilgi vermek isterim. Söz konusu kitaplar ve bunlara ait özet bilgiler aşağıdaki tabloda yer aldığı gibidir.

Kitap
Özet Bilgi

![Wf_Book_1.jpg](images/Wf_Book_1.jpg)
[Microsoft Windows Workflow Foundation Step by Step](http://www.amazon.com/Microsoft-Windows-Workflow-Foundation-Developer/dp/073562335X/ref=pd_bbs_3?ie=UTF8&s=books&qid=1198592693&sr=8-3)

Mart 2007 tarihinde çıkan Microsoft Press'e ait bu kitap içerisindeki bilgiler ile WWF mimarisini adım adım öğrenmek mümkün. Aynen Microsoft Windows Communication Foundation Step by Step kitabında olduğu gibi oldukça iyi bir anlatıma sahip. Kısa sürede tamamlanabilecek bu kitap ile WWF mimarisini orta seviyede öğrenmek mümkün. Toplam 19 bölümden oluşan kitap içerisinde iş akışlarının SOA ile entegrasyonu, transaction desteği, paralel aktivitilerin (Activities) tasarlanması gibi ileri seviye konularda yer almakta.

![Wf_Book_2.jpg](images/Wf_Book_2.jpg)
[Pro WF: Windows Workflow in.NET 3.0 (Expert's Voice in.Net)](http://www.amazon.com/exec/obidos/tg/detail/-/1590597788/ref=ord_cart_shr?_encoding=UTF8&m=ATVPDKIKX0DER&v=glance)
Şubat 2007 tarihinde APress tarafından yayınlanan bu kitap 744 sayfalık bir içeriği 17 bölüm altında toplamakta. Workflow konusunda yazılmış kitaplar arasında temelden ileri seviyeye doğru giden ve en anlaşılır olanlarından bir tanesi. İleri seviye sayılabilecek bölümlerde, iş akışlarının dinamik güncellenmesi, iş akışlarında serileştirme, iş akışlarının izlenmesi gibi konu başlıklarıda yer almaktadır.

![Wf_Book_3.jpg](images/Wf_Book_3.jpg)
[Professional Windows Workflow Foundation](http://www.amazon.com/exec/obidos/tg/detail/-/0470053860/ref=ord_cart_shr?_encoding=UTF8&m=ATVPDKIKX0DER&v=glance)
Wrox yayınlarından Mart 2007 tarihinde çıkartılan bu kitap 410 sayfalık mütevazi bir içeriğe sahip. Lakin bu içerik sayesinde çok kısa zamanda Windows Workflow Foundation mimarisini anlamak ve etkin bir şekilde kullanabilmek mümkün. Üstelik kitapta best practices, ileri seviyede aktivite (Activity) tasarlanması, dinamik güncelleştirme, ofis (Office) sistemleri ile entegrasyon gibi enteresan kısımlarda yer almakta.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/WWFGiris.rar)
