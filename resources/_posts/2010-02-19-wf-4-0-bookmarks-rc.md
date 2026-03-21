---
layout: post
title: "WF 4.0 - Bookmarks [RC]"
date: 2010-02-19 05:04:00 +0300
categories:
  - wf-4-0-rc
tags:
  - workflow-foundation
---
Çalışmakta olduğum yazılım şirketinin çok yakınında kocaman bir alışveriş merkezi bulunmakta. Bazen öğle yemekleri için alışveriş merkezinin tahsis ettiği servisler ile oraya gidiyoruz. Alışveriş merkezi olduğu için tehlikeli bir yer olduğunu da söyleyebiliriz.

![blg154_Giris.jpg](/assets/images/2010/blg154_Giris.jpg)

![Sealed](/assets/images/2010/smiley-sealed.gif)

Nitekim çok büyük bir yer ve A'dan Z'ye herşey bulunabilmekte. Arkadaşlarım ile sık uğradığım mekanlardan birisi de D&R kitap evi. Çoğunlukla aylık dergilerimi almak için uğramaktayım (Aslında Türkiye'de Amazon gibi bir kitap dağıtım evi olmadığı için çok şanslı olduğumu düşünüyorum. Her halde öyle bir yer açılsa kazancımın çok büyük bir kısmı meslek kitaplarına gider)

Geçen gün yine Bilim Teknik, NTV Bilim ve NG dergilerimi almak üzere oradaydım. Sırada beklerken kasada ücretsiz olarak verilen kitap ayraçlarını farkettim. Hep görürdüm ama bu gün biraz daha anlamlı geliyorlardı. Üzelerinde çeşitli reklamlar veya faydalı bilgiler bulunan bu ayraçlar yardımıyla (ki Bookmark diyebilir miyiz acaba? ![Wink](/assets/images/2010/smiley-wink.gif)), okuduğumuz kitabın neresinde kaldığımızı kolayca hatırlayabildiğimizi düşünmeye başladım. Derken evde uzun süredir el değdirip kaldığım yerden devam edemediğim kitaplarım aklıma geldi. Hüzünlendim...

![Tongue out](/assets/images/2010/smiley-tongue-out.gif)

Tesadüfe bakın ki bu kitapları okumak baya uzun sürmüştü. Zamanın neresinde okumaya başladığımı pek hatırlamamakla birlikte, neresinde kaldığımı da hatırlamadığım bir kaç kitap...Tesadüfe bakın ki bu uzun sürecin benzeri Workflow Uygulamalarında da söz konusu olabilmekteydi.

![Wink](/assets/images/2010/smiley-wink.gif)

Aslında.Net Framework 3.5 sürümünde Uzun Süreli İşemlerin (Long Running Process) için ExternalDataExchangeService veya WorkflowQueue tiplerinden yararlanılmaktadır. Ne varki Workflow Foundation 4.0 sürümünde, geliştiricilerin kulağına daha hoş gelen Bookmark kavramı ile karşılaşmaktayız. Peki Bookmark nedir? Ne işe yaramaktadır? Nasıl kullanılmaktadır? Konuyu anlamanın belki de en kolay yolu her zaman ki gibi basit bir örnek üzerinden ilerlemekle olacaktır. Bu nedenle Bookmark kavramının tanımlamasını yazımızın sonunda yapmaya çalışacağız.

Bookmark kullanımında önemli olan noktalardan birisi, geçici olarak duraksatılabilecek (Pause) Activity bileşeninin Workflow'un çalışma zamanı içeriğine ulaşabiliyor olmasıdır. Bu nedenle en uygun aktivite bileşenleri, NativeActivity (veya NativeActivity) türevli olanlardır. Bunu ilk gereksinimimiz olarak düşünebiliriz. Aşağıdaki diagramda.Net Framework 4.0 RC sürümü içerisinde yer alan NativeActivity tipleri ve üyeleri görülmektedir.

![blg154_ClassDiagram.gif](/assets/images/2010/blg154_ClassDiagram.gif)

Şimdi bu türetmeyi kullanarak aşağıdaki kod parçasında görülen aktivite bileşenini geliştirdiğimizi düşünelim. İlgili örneğin bir Workflow Console Application üzerinden geliştirebiliriz.

```csharp
using System;
using System.Activities;

namespace HelloBookmarks
{

    public sealed class ResizeImageActivity 
        : NativeActivity
    {
        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            Console.WriteLine("Resize Image bir takım işlemler yapıyor");
            // Bazı işlemler
            // İkinci parametre BookmarkCallback temsilcisi tarafından işaret edilen bir fonksiyondur.
            context.CreateBookmark("ResizeImageBookmark",                
                (nac, b, obj) =>
                {
                    Console.WriteLine("Resume edilen bookmark adı {0}",b.Name);                    
                }
            );
        }
    }
}
```

Bookmark işlemleri Idle konuma düşebilen Workflow aktivitelerinde işe yarayacak bir teknik olarak düşünülmelidir. Nitekim bir aktivite içerisinde herhangibir zamanda Pause etme ve sonraki bir anda Resume etme söz operasyonları konusudur. Bu sebepten CanIncludeIdle özelliğinin override edilmesi ve geriye true değer döndürmesi söz konusudur. Aksi durumda çalışma zamanında aşağıda görülen InvalidOperationException hata mesajı alınacaktır.

![blg154_Exception.gif](/assets/images/2010/blg154_Exception.gif)

CreateBookmark metodunun ikinci parametresi BookmarkCallback tipinden bir temsilcidir (Delegate). Bu temsilcinin yapısı ise aşağıdaki gibidir.

public delegate void BookmarkCallback (System.Activities.NativeActivityContext context, System.Activities.Bookmark bookmark, object value)

Buna göre örneğimizde yer alan isimsiz metodun (Anonymous Method) ilk parametresi ile Activity'nin çalışma zamanındaki çevresel içeriğine, ikinci parametre ilede Bookmark örneğine erişilebilir. Bu temsilci aslında bir geri bildirim metodunu (Callback Method) işaret etmektedir. Bir başka deyişle, Idle konumda kalan Activity örneğinin tekrar Resume edilmesi halinde devreye girecek olan metod olarak düşünülebilir. Dolayısıyla geri bildirim metodu içerisinde CreateBookmark tarafında saklanan bazı varlıkların tekrardan yüklenmesi, hazırlanması gibi operasyonlar ele alınabilir. CreateBookmark metodunun aslında 8 aşırı yüklenmiş (Overload) versiyonu bulunmaktadır.

Diğer versiyonlar göz önüne alındığında dikkat çeken parametrelerden birisi BookmarkOptions Enum sabitidir. Bu enum sabiti MultipleResume, NonBlocking ve None değerlerinden birisini almaktadır. Varsayılan değer None'dır. MultipleResume olması halinde bir den fazla Resume işlemi yapılabileceği belirtilir. NonBlocking değerine göre ilgili aktivite bileşeni Resume edilmemiş olsa dahi WF'in çalışacağını belirtilir. Nitekim normal şartlar altında bir aktivite içerisinde oluşturulan Bookmark'ların tamamı Resume edilmediği sürece WF'in tamamlanması söz konusu değildir. Dilerseniz MultipleResume ve NonBlocking değerlerini bir arada kullanabilirsiniz. Gelelim aktivitenin nasıl kullanılacağına. Örneğimizdeki amacımız sadece Bookmark kullanımını görmek olduğundan aşağıdaki şekilde görülen Workflow Activity içeriğini değerlendirebiliriz.

![blg154_Workflow1.gif](/assets/images/2010/blg154_Workflow1.gif)

Bookmark kullanımı yazımızın başında da belirttiğimiz üzere uzun süreli işlemler (Long Running Process) için anlamlıdır. Bu sebepten WorkflowApplication tipinin kullanılması gerekmektedir. Workflow Console Application tipinden olan uygulamamızda, çalışma zamanındaki Idle durumları irdelememiz aslında son derece kolaydır. Console.ReadLine metodu burada çok işe yarayacaktır.

![Wink](/assets/images/2010/smiley-wink.gif)

İşte kod içeriğimiz;

```csharp
using System;
using System.Activities;
using System.Threading;

namespace HelloBookmarks
{

    class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent rE = new AutoResetEvent(false);
            
            // Workflow örneği oluşturlur
            Workflow1 wf1 = new Workflow1();
            // Workflow Application örneği oluşturulur
            WorkflowApplication wfApp = new WorkflowApplication(wf1);
            // Workflow' un tamamlanması sonrası devreye girecek Completed olay metodu
            wfApp.Completed = (e) => { rE.Set(); }; // işlemlerin bittiğine dair bilgilendirme için AutoResetEvent örneğinin Set metodu çağırılır.
            // Workflow çalışma zamanı başlatılır dolayısıyla Workflow1 örneği yürütülür
            wfApp.Run();            
            Console.WriteLine("Bir süre bekleyin...");
            Console.ReadLine(); // Bu noktada Workflow1 örneğinin Idle konuma geçmesi söz konusudur.
            
            /* Kullanıcı devam etmek istediğinde Bookmark' lanan Workflow1 örneğine tekrardan hayata geçirilir. ResumeBookmark metodunun ilk parametresi dikkat edileceği üzere ResizeImageActivity içerisinde kullanılan Bookmark adıdır. Bu adın aslında aktivite bileşeni içerisinden çalışma zamanı ortamına verilmesi(örneğin bir OutArgument) ile faydalı olabilir. İkinci parametre ise hangi Workflow örneğinin Resume edileceğidir. Buna göre Workflow1 içerisinde ResizeImageBookmark isimli Bookmark' ın yer aldığı aktivite bileşeninin Resume edilmesi söz konusudur. */
            BookmarkResumptionResult result = wfApp.ResumeBookmark("ResizeImageBookmark", wf1);
            // ResumeBookmark metodunun sonucu olan Enum sabitinin değerine göre bir işlem yapılabilir
            switch (result)
            {
                case BookmarkResumptionResult.NotFound:
                    Console.WriteLine("Not Found");
                    break;
                case BookmarkResumptionResult.NotReady:
                    Console.WriteLine("Not Ready");
                    break;
                case BookmarkResumptionResult.Success:
                    Console.WriteLine("Success");
                    break;
                default:
                    break;
            }
            
            // Eğer Workflow Application' ın beklediği çalışan örnekler var ise bunların tamamlanması beklenir
            rE.WaitOne();
        }
    }
}
```

Aslında örnek kodumuz Workflow1 tipinden bir nesne örneğini çalıştırmakta ve yaşamı içerisinde kullanıcından belirli süreliğine tuşa basmasını beklemektedir. Tuşa basmayı beklediği sırada ise Idle olabilen bileşenlerin bu konuma geçmesi söz konusudur. ResumeBookmark çağrısından sonra ise Bookmark ile Pause konumda duran aktivitenin ilgili geri bildirim metodunun çağırılması ve dolayısıyla yürütülmeye devam edilmesi sağlanır. İşte örnek program kodumuzun çalışma zamanı çıktısı.

![blg154_Runtime.gif](/assets/images/2010/blg154_Runtime.gif)

Bu arada çalışma zamanında aktif olan Bookmark listesini de WorkflowApplication nesne örneğine ait GetBookmarks metodu üzerinden alabileceğinizi belirtmek isterim. Aşağıdaki kod parçasında bu durum örneklenmektedir.

```csharp
using System;
using System.Activities;
using System.Threading;
using System.Activities.Hosting;

namespace HelloBookmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent rE = new AutoResetEvent(false);                        
            Workflow1 wf1 = new Workflow1();
            WorkflowApplication wfApp = new WorkflowApplication(wf1);
            wfApp.Completed = (e) => { rE.Set(); }; // işlemlerin bittiğine dair bilgilendirme için AutoResetEvent örneğinin Set metodu çağırılır.
            wfApp.Run();            
            Console.WriteLine("Bir süre bekleyin...");
            Console.ReadLine(); 
            foreach (BookmarkInfo bm in wfApp.GetBookmarks())
            {
                Console.WriteLine(bm.BookmarkName);
            }            
...
```

Şimdi örneğimizi biraz daha ilginç bir hale getirelim. Öncelikli olarak aşağıdaki kod içeriğine sahip Custom Activity sınıfını oluşturarak projemize ilave edelim.

```csharp
using System;
using System.Activities;

namespace HelloBookmarks
{

    public sealed class SendImageActivity 
        : NativeActivity
    {        
        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            Console.WriteLine("Send Image bir takım işlemler yapıyor");
            context.CreateBookmark("SendImageBookmark",                
                (nac, b, obj) =>
                {
                    Console.WriteLine("Resume edilen bookmark adı {0}",b.Name);                    
                }
            );
        }
    }
}
```

ResizeImageActivity tipinin tıpkısının aynısı olan bu bileşeni de ele alarak Workflow1 içeriğini aşağıdaki şekilde görüldüğü gibi değiştirelim.

![blg154_Workflow1Last.gif](/assets/images/2010/blg154_Workflow1Last.gif)

Bu sefer aynı anda birden fazla aktivitenin çalıştırılmasına izin veren Parallel bileşeninden yararlanmaktayız. İncelemek istediğimiz nokta ise şu; her iki aktivite de Execute metodları içerisinde birer Bookmark oluşturmaktadır. Buna göre program kodumuzda birden fazla Bookmark'ın nasıl ele alınacağına bakmak istiyoruz. Söz konusu vakayı analiz etmek için, Main metoduna ait kod içeriğini aşağıdaki gibi değiştirmemiz yeterli olacaktır.

```csharp
using System;
using System.Activities;
using System.Threading;
using System.Activities.Hosting;

namespace HelloBookmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent rE = new AutoResetEvent(false);            
            Workflow1 wf1 = new Workflow1();
            WorkflowApplication wfApp = new WorkflowApplication(wf1);
            wfApp.Completed = (e) => { rE.Set(); };
            wfApp.Run();            
            Console.WriteLine("Bir süre bekleyin...");
            Console.ReadLine();
            Console.WriteLine("***Etkin Bookmark Listesi***");
            foreach (BookmarkInfo bm in wfApp.GetBookmarks())
            {
                Console.WriteLine("Bookmark Name:{0}, Owner:{1}",bm.BookmarkName,bm.OwnerDisplayName.ToString());
            }
            Console.WriteLine();
            
            BookmarkResumptionResult result1 = wfApp.ResumeBookmark("ResizeImageBookmark", wf1);
            BookmarkResumptionResult result2 = wfApp.ResumeBookmark("SendImageBookmark", wf1);

            Console.WriteLine("ResizeImageBookmark için Resume Result : {0}",result1.ToString());
            Console.WriteLine("SendImageBookmark için Resume Result : {0}", result2.ToString());

            rE.WaitOne();
            Console.ReadLine();
        }
    }
}
```

Ve işte çalışma zamanı sonuçları;

![blg154_Runtime2.gif](/assets/images/2010/blg154_Runtime2.gif)

Çıktımıza göre her iki akitivite bileşeni, eş zamanlı olarak Execute metodlarını icra etmiş ve birer Bookmark oluşturmuştur. Sonrasında kullanıcının tuşa basması ile devam eden süreçte ilk olarak yüklü olan Bookmark'lar listelenmiş ve ardından ResumeBookmark çağrıları nedeniyle Pause edilmiş olan aktivitelerin geri bildirim operasyonları devreye girmiştir. Her iki Bookmark'ında Resume edilmesinin sonucu olarak Workflow1 örneğinin işlemlerini tamamladığı anlaşılmaktadır. Program kodu da buna göre sonlanır.

Görüldüğü gibi bir aktivitenin Bookmark'lanması aslında isimlendirilmiş duraksama noktalarına (Named Pause Points) sahip olması anlamına gelmektedir. Öyleki, ResumeBookmark metodu sayesinde Pause edilen noktadan tekrar yürütülmeleri sağlanabilir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloBookmarks.rar (54,52 kb)](/assets/files/2010/HelloBookmarks.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir]
