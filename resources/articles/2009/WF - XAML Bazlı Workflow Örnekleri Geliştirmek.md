---
title: "WF - XAML Bazlı Workflow Örnekleri Geliştirmek"
pubDate: 2009-05-06 11:23:00
categories:
  - WF
tags:
  - WF
  - C#
  - XML
  - Workflow Foundation
  - XAML
  - HTTP
  - Authentication
  - Delegates
  - Visual Studio
---

# WF - XAML Bazlı Workflow Örnekleri Geliştirmek
Merhaba Arkadaşlar,

Geçtiğimiz günlerde Workflow 4.0 ile ilişkili araştırmalarıma devam ederken, özellikle dekleratif olarak tanımlanabilen WF servislerindeki önemli bir noktayı farkettim. Bu, aynı zamanda WF 4.0 ile birlikte gelen en önemli yenilikler arasındaydı. (Hatta WF motorunun-Engine- değişmesi veya temel aktivite kütüphanesinde (Base Activity Library), ata tip olarak WorkflowElement isimli yeni bir sınıfın getirilmesi kadar önemliydi) Bir workflow örneğinin sadece XAML içeriğinden oluşacak şekilde koda ihtiyaç duymadan tasarlanabilmesi (design), derlenebilmesi (Compile) ve gerektiğinde çalışma zamanında basit bir notepad uygulaması ile değiştirilerek güncellenebilmesi...Burada özellikle derleme konusu son derece dikkat çekici. Nedeni mi?

Nedeni araştırmak için elbette basit bir senaryo üzerinden ilerlemem gerekiyordu. Bu yüzden dün gece biraz geç bir vakittede olsa üşenmeden kodlamaya başladım. Senaryoya göre,.Net 3.5 açısından olaya bakıp, sadece XAML içeriğinden oluşacak bir Workflow örneğini oluşturmak ve çalıştırmak istiyordum. Bu sebeple öncelikle, örnek bir aktivite tipi geliştirmeye karar verdim. ProductOrderActivity isimli aktivite bileşenini, ayrı bir Workflow Activity Library projesi içerisinde aşağıdaki sınıf diagramında olduğu gibi tasarladım.

![blg13_1.gif](images/blg13_1.gif)

Kod içeriği

```csharp
using System;
using System.ComponentModel;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;

namespace NorthwindActivities
{
 public class ProductOrderActivity
        : SequenceActivity
 {
        public static DependencyProperty ProductNumberProperty = DependencyProperty.Register("ProductNumber", typeof(string), typeof(ProductOrderActivity));
        public static DependencyProperty PartCountProperty = DependencyProperty.Register("PartCount", typeof(int), typeof(ProductOrderActivity));
        public static DependencyProperty OrderDateProperty = DependencyProperty.Register("OrderDate", typeof(DateTime), typeof(ProductOrderActivity));

        [Description("Ürün Numarası")]
        [Category("Sipariş Parametreleri")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ProductNumber
        {
            get
            {
                return ((string)(base.GetValue(ProductOrderActivity.ProductNumberProperty)));
            }
            set
            {
                base.SetValue(ProductOrderActivity.ProductNumberProperty, value);
            }
        }

        [Description("Parça sayısı")]
        [Category("Sipariş Parametreleri")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int PartCount
        {
            get
            {
                return ((int)(base.GetValue(ProductOrderActivity.PartCountProperty)));
            }
            set
            {
                base.SetValue(ProductOrderActivity.PartCountProperty, value);
            }
        }

        [Description("Sipariş Tarihi")]
        [Category("Sipariş Parametreleri")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTime OrderDate
        {
            get
            {
                return Convert.ToDateTime(base.GetValue(ProductOrderActivity.OrderDateProperty));
            }
            set
            {
                base.SetValue(ProductOrderActivity.OrderDateProperty, value);
            }
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            Console.WriteLine("{0} numaralı üründen {1} adet sipariş işlemi...",ProductNumber,PartCount.ToString());
            Console.WriteLine("{0}  tarihine kadar sipariş edilmelidir.",OrderDate.ToShortDateString());
            return base.Execute(executionContext);
        }
 }
}
```

Aktivite aslında basit olarak bir ürünün belirli bir tarihe kadar, istenen miktarda sipariş edilmesi adımını yürüten bir modele sahipti. Tabiki sembolik olarak. Bu sebepten ProductNumber, PartCount ve OrderDate isimli özellikleri bulunmaktaydı. Bu özellikler,, aktivitenin başka aktivitiler içermesi veya başka aktivitelere bağlanması (Binding) gibi ihtiyaçlara sahip olabileceğinden DependencyProperty tipi ile ilişkilendirilmiştim. Özellikler, tasarım zamanında Visual Studio IDE'si tarafından değerlendirileceğinden, Description, Category, Browsable ve DesignerSerializationVisibility gibi niteliklerlede sahipti. Aktivite icra edildiğinde ise ezilen (override) Exeute metodu içeriği çalıştırılmaktadır. Bu kısımda işin modeline göre bir takım işlemler yapılması gerekmekte. Ben sembolik olarak sadece ekrana bazı bilgiler yazdırmayı hedefledim.

Şimdi gelelim bu aktiviteyi kullanacağımız örnek Workflow uygulamasına. Bu amaçla testleri kolayca yapabileceğim bir Sequential Workflow Console Application projesi oluşturdum. Projenin, ProductOrderActivity aktivitesini kullanabilmesi içinde, tanımlandığı NorthwindActivities kütüphanesini referans ettim.

![blg13_2.gif](images/blg13_2.gif)

Artık ön hazırlıklar tamamlanmıştı. Sırada XAML bazlı Sequential Activity öğesinin eklenmesi vardı. Yanlız burada yapacağımız seçimin önemli olduğunu vurgulamak isterim. Nitekim amacımız kod içermeyen ve XAML içeriğine sahip bir Workflow örneği geliştirmek olduğundan, proje öğelerinden Sequential Workflow (with no code) tipini seçmemiz gerekiyor. Tabi eğer State Machine Workflow tipinden bir proje söz konusuysa, State Machine Workflow (with no code) öğesinin seçmemiz gerekiyor.

![blg13_3.gif](images/blg13_3.gif)

Bunun sonucunda projeye aşağıdaki şekilde görülen ProductOrderFlow isimli XOML uzantılı bir öğenin eklendiği görülür.

![blg13_4.gif](images/blg13_4.gif)

Ancak görüldüğü gibi bu oluşum sırasında xoml uzantılı içerik dışında cs uzantılı bir kod içeriğide üretilmektedir. Şunu hemen hatırlatayım. Amacımız kesin olarak kod dosyasından bağımsız bir Workflow örneği oluşturmaktır. Peki bunun için ne yapmalıyız? Aslında şu an için çözüm son derece basit. cs uzantılı dosya silinir

![Laughing](images/smiley-laughing.gif)

Bende aynen böyle yaptım. Tabi şu anda Workflow içerisinde herhangibir aktivite kullanılmamakta.

Ancak dikkat edilmesi gereken önemli bir nokta daha var. Bu Workflow için bir kod bloğu olmadığından, içeride kullanacağımız aktivitelerin Codebehind dosyası içerisine kod atmayacak şekilde kullanılmaları gerekmekte. Söz gelimi bir CodeActivity bileşenini kullanmak istediğimizde, bu bileşenin çalıştırılması sonucu devreye girecek metodun, cs kod dosyası içerisinde yer alması gerekmektedir. Oysaki şu anki teorimize göre böyle bir dosya bulunmamaktadır (olmamalıdır). Buda bizi, özel aktivite tiplerinin yazılmasına itmektedir. Ama elbetteki kod dosyasına ihtiyaç duymayan bazı aktivite bileşenleri burada ele alınabilir. Örneğin DelayActivity aktivitesi. Bu bilgilerden yola çıkarak ProductOrderFlow.xoml içeriğini tasarım zamanında aşağıdaki gibi oluşturdum.

![blg13_5.gif](images/blg13_5.gif)

Şekildende görüldüğü gibi, Workflow içerisinde önce delayActivity bileşeni ve peşinden yazdığım productOrderActivity bileşeni çalıştırılmakta. productOrderActivity1 bileşeninin OrderDate, PartCount ve ProductNumber isimli özelliklerine ise sembolik değerler aktarılmış durumdadır. Şimdi xoml içeriğini XML Editor yardımıyla açarsak, aşağıdaki içeriğin oluşturulduğunu görürüz.

```xml
<SequentialWorkflowActivity x:Class="NorthwindActivities.ProductOrderFlow" x:Name="ProductOrderFlow" xmlns:ns0="clr-namespace:NorthwindActivities;Assembly=NorthwindActivities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/workflow">
 <DelayActivity TimeoutDuration="00:00:05" x:Name="delayActivity1" />
 <ns0:ProductOrderActivity x:Name="productOrderActivity1" ProductNumber="PART-1001" PartCount="100" OrderDate="2009-05-07T00:00:00.0000000" />
</SequentialWorkflowActivity>
```

Görüldüğü gibi Workflow içerisinde aktivitilerin tamamı, XAML içeriği olarak oluşturulmuştur. Bu, zihinlerde yeni ufuklar açacak kadar önemli bir ayrıntır. Çünkü, istenirse bu içerikte yer alan elementlerin yerleri değiştirilerek akışın şekline müdahele edilebilir (Koda girmeye gerek kalmadan). Yada başka elementler basit bir notepad programı yardımıyla içeriğe dahil edilip akışa yeni adımların eklenmesi sağlanabilir. Hatta bu içerik belki bir depolama ortamında saklanarak farklı görsel uygulamaların bu akışları ele alabilmesi, değiştirebilmesi sağlanabilir (Oslo, Quadrant kavramına gitmeye çalıştığımı sanıyorumki anlamışsınızdır)

Sonrasında aşırı heyecan yapmaya gerek olmadığını farkedip devam etmeye karar verdim. Bu nedenle projeyi derleyerek yoluma devam etmek istedim. Ancak oldukça ilginç bir durumla karşılaştım. Proje içerisinde birden ProductOrderFlow.xoml.cs isimli kod dosyası ortaya çıktı.

![Sealed](images/smiley-sealed.gif)

Gecenin karanlığında sanki bir korku filminde yaşanan gerilimi hissetmiştim. Ensemden soğuk bir ter damlası ilerlerken, bu hortlağın nereden çıktığını düşünüyordum. Aslında bu son derece doğaldı. Nitekim proje derlendiğinde, xoml dosyası da hesaba katıldığından, cs dosyası otomatik olarak üretilmekteydi. Bu tabiki istediğim bir durum değildi. Bu nedenle ProductOrderFlow.xoml dosyasının Build Action özelliğinin değerini None olarak belirlemek yeterliydi. Tabiki sonrasında (öncesinde) cs dosyasını silmeyi unutmamak da gerekiyordu.

![blg13_6.gif](images/blg13_6.gif)

Evettt...Herşey hazır gibi. Mi acaba? Aslında unuttuğum önemli bir nokta var. Söz konusu Workflow nasıl çalıştırılacak? Nitekim, build işlemi sırasında ProductOrderFlow.xoml içeriğini devre dışı bıraktığımızdan, bunun çalışma zamanında bir şekilde yükleniyor olması gerekiyor. Ancak derlenmiş kod içerisinde bu Workflow'a ait bir tip tanımlamasıda yer almadığından (çünkü Build Action=None olarak belirlendi) çalışma zamanında xoml dosyasının içeriğinin ele alınması gerekmekte. Bunu sağlamak için tek yapılması gereken çalışma zamanı kodlamasını aşağıdaki gibi değiştirmek.

```csharp
using System;
using System.Threading;
using System.Workflow.Runtime;
using System.Xml;

namespace NorthwindActivities
{
    class Program
    {
        static void Main(string[] args)
        {
            using(WorkflowRuntime workflowRuntime = new WorkflowRuntime())
            {
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                workflowRuntime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) {
                    waitHandle.Set();
                    Console.WriteLine("İşlemler tamamlandı");
                };
                workflowRuntime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e)
                {
                    Console.WriteLine(e.Exception.Message);
                    waitHandle.Set();
                };

                XmlReader reader = XmlReader.Create("..\\..\\ProductOrderFlow.xoml");
                WorkflowInstance instance = workflowRuntime.CreateWorkflow(
                    reader
                    );
                instance.Start();

                waitHandle.WaitOne();
            }
        }
    }
}
```

Koddanda görüldüğü gibi yapılması gereken, xoml içeriğini XmlReader nesnesi yardımıyla ortama almak ve WorkflowInstance örneğinin oluşturulması sırasında CreateWorkflow metoduna parametre olarak vermektir. Uygulamayı bu haliyle çalıştırdığımda aşağıdaki sonucu elde ettim.

![blg13_7.gif](images/blg13_7.gif)

Sanıyorumki ben dahil herkes, uygulamanın çalışmasını bekliyordu. Ancak yukarıda görüldüğü gibi bir istisna (Exception) aldım. Karabasan devam ediyordu sanki. Çözümü bulmam biraz zamanımı aldı. Aslında problem, xoml içeriğinde yer alan

x:Class="NorthwindActivities.ProductOrderFlow"

bildirimiydi. Çalışma zamanının kızması son derece doğaldı. Hak vermem gerekiyordu. Hata mesajındanda anlaşılacağı üzere bir doğrulama (Validation) sorunu vardı. Bunun kaynadğında ise ProductOrderFlow tipi yer almakta. Derken tepemde bir ampül yanıverdi.

![Wink](images/smiley-wink.gif)

cs dosyasını çıkarmış ve xoml içeriğini uygulamaya dahil etmemiştim. Dolayısıyla söz konusu tip zaten yoktu ve çalışma zamanı, akışı doğrulamaya çalışırken tam bu noktada çatlıyordu. Neden böyle olmuştu peki? Tabiki Visual Studio ortamında sadece XAML içeriğinden oluşan bir akış geliştirme desteği bulunmamaktaydı ve ben kod parçalı oluşturulan akışın üzerinde değişiklikler yapıyordum. Yani varsayılan modele karşı gelmiştim. Haliyle xoml içeriğini aşağıdaki gibi değiştirmem gerekti.

```csharp
<SequentialWorkflowActivity x:Name="ProductOrderFlow" xmlns:ns0="clr-namespace:NorthwindActivities;Assembly=NorthwindActivities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/workflow">
 <DelayActivity TimeoutDuration="00:00:05" x:Name="delayActivity1" />
 <ns0:ProductOrderActivity x:Name="productOrderActivity1" ProductNumber="PART-1001" PartCount="100" OrderDate="2009-05-07T00:00:00.0000000" />
</SequentialWorkflowActivity>
```

Artık tekrar testi yapabilirdim. Programı çalıştırdığımda aşağıdaki sonuçla karşılaştım.

![blg13_8.gif](images/blg13_8.gif)

Nihayet

![Smile](images/smiley-smile.gif)

Artık xoml içeriği ile biraz oynayabilirdim. Bu amaçla xoml dosyasını notepad ile açtım ve aşağıdaki hale getirdim.

![blg13_10.gif](images/blg13_10.gif)

Görüldüğü gibi productOrderActivity2 isimli yeni bir bileşeni akış içerisine dahil edip özelliklerine sembolik değerler atadım. Bundan sonra program kodunu derlemeden çalıştırdığımdaysa aşağıdaki ekran görüntüsü ile karşılaştım.

![blg13_11.gif](images/blg13_11.gif)

Görüldüğü gibi yeni eklenen bileşende başarılı bir şekilde çalıştırıldı. Sonuç olarak; bir workflow örneğinin koddan bağımsız olacak şekilde tasarlanabilmesi, XAML içeriğinin basit bir editor yardımıyla değiştirilip akışın güncellenebilmesi sağlanabilmektedir. Burada önemli olan noktalardan birisi, söz konusu xoml dosyalarının, çalışma zamanında değerlendirilip yürütülmeleridir.

Şimdi şöyle bir senaryoyu göz önüne alalım. Buradaki gibi tamamen XAML bazlı akışların bir depoda saklandığını düşünelim. Örneğin SQL sunucusu üzerinde veya bir x veri depolama sisteminde. Sonrasında ise, bu akışları kullanan (içeren) süreçler ve programların görsel bir araç yardımıyla tasarlanabildiğini ve değiştirilebildiğini göz önüne alalım. Hatta bu akışların istenirse export edilip farklı uygulama alanlarına import edilebildiklerini farz edelim...Derken zaten Microsoft'un gitmek istediği noktada yer alan bir kaç temel ihtiyaçtan bir kısmını özetlemiş oluyoruz.

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[UsingXAML.rar (43,63 kb)](assets/UsingXAML.rar)
