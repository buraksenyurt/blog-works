---
layout: post
title: "TPL için Önemli Bir Kavram : Task [Beta 1]"
date: 2009-06-04 19:05:00 +0300
categories:
  - tpl
tags:
  - task-parallel-library
---
Bir önceki blog yazımda Task Parallel Library alt yapısının ne olduğunu sizlere aktarmaya çalışmıştım. Tabiki bu alt yapı üzerinde durulması gereken pek çok konu bulunmaktadır. Heyecanım çok, anlatmak içinde sabırsızlanıyorum. Ama her zamanki gibi adım adım ilerlemekte ve acele etmemekte yarar olduğu kansındayın. TPL ile ilişkili önemli konulardan birisi Task (yada Task) sınıfıdır. TPL esas itibariyle görev adı verilen küçük iş parçaları üzerine kurulu bir yapı olarak düşünülebilir. Bu nedenle Task sınıfı son derece önemlidir.

Nitekim görevlerin yönetimli kod tarafındaki ifadesidir. Bu sınıf yardımıyla, paralel çalışacak olan görevlerin başlatılması, iptal edilmesi, bekletilmesi, arka arkaya eklenerek bir süreç tesis edilmesi gibi pek çok işlem yapılabilir. Task sınıfı normal şartlarda geriye değer döndürmeyen fonksiyonelliklerin eş zamanlı olarak çalıştırılmasında ele alınmaktadır. Geriye değer döndüren metodlar söz konusu olduğunda ise, Task generic tipinden yararlanılabilir. Buradaki T, paralel çalışan metodun dönüş tipi olarak düşünülebilir. Aşağıdaki sınıf diagramında söz konusu tipler ve üyeleri yer almaktadır.

![blg27_1.gif](/assets/images/2009/blg27_1.gif)

Aslında Task ve Task sınıflarının static Factory özelliği üzerinden gidildiğinde StartNew metodu yardımıyla görevlerin başlatılması sağlanmaktadır. Diğer yandan Task sınıfının Result özelliği, geri dönüş tipini belirtmektedir. Ayrıca sınıf diagramındanda görüldüğü gibi Task sınıfı, Task sınıfından türemektedir. Factory özellikleri, TaskFactory veya TaskFactory tipinden referanslar barındırmaktadır. Bu tiplerin içeriği ise aşağıdaki şekilde görüldüğü gibidir.

![blg27_2.gif](/assets/images/2009/blg27_2.gif)

Tüm tiplerde pek çok önemli üye bulunmaktadır. Bunların hemen hepsini zaman içerisinde ele almaya gayret edeceğiz, hiç merak etmeyin. Şimdi gelin Task ve Task sınıflarını basit (ve her zamanki gibi tam anlamıyla gerçek hayat örneği olmayan

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

) bir örnek üzerinden ele almaya çalışalım. İlk olarak senaryomuzdan bahsedelim. Senaryomuza göre resim dosyalarına ait 3 farklı işlemin gerçekleştiği metodların eş zamanlı ve paralel olarak çalıştırılmasını sağlamayı hedefliyoruz. Buna göre bir klasörden,

- Resim dosyalarının toplam boyutunun bulunması,
- Resimler içerisinde bmp olanların kaç adet olduklarının tespit edilmesi,
- Resimler içerisinde bmp olanların farklı bir klasöre kopylanması,

işlemlerini gerçekleştiren fonksiyonelliklerimiz bulunmakta.

Normal şartlar altında herkesin burada durup biraz düşünmesi gerekiyor. Elimizde.Net Parallel Extensions olmadığını varsayalım. Bu durumda ya Multi-Thread mimarisini kullanacağız, yada delegate (temsilci) tiplerinden yararlanarak asenkron erişim modellerini (Polling, Callback, WaitHandle, Event-Based) ele alacağız. Bunu bir düşünün ve senaryoyu bu materyaller ile yazmayı bir deneyin.

![Wink](/assets/images/2009/smiley-wink.gif)

Tabi şunu biliyoruzki TPL alt yapısı, paralel işlemleri kolayca ele almamızı sağlayacak şekilde tasarlanmıştır. İlk etapta kodlarımızı aşağıdali gibi geliştirdiğimiz varsayalım.(Kodlarımızı Visual Studio 2010 Beta 1 üzerinde geliştirdiğimizi hatırlatayım)

```csharp
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HelloTasks
{
    class Program
    {
        static string imagesPath = ConfigurationManager.AppSettings["ImagesPath"];

        static void Main(string[] args)
        {
            long totalSize = GetTotalSize();
            int bmpCount = GetBmpCount();
            CopyBmp();
            Console.WriteLine("Toplam boyut {0} byte\nBmp sayısı {1}", totalSize.ToString(), bmpCount.ToString());

            Console.WriteLine("Devam etmek için bir tuşa basınız");
            Console.Read();
        }

        static long GetTotalSize()
        {
            Console.WriteLine("\t GetTotalSize metodu için Managed Thread Id {0}. Zaman {1}",Thread.CurrentThread.ManagedThreadId.ToString(),DateTime.Now.ToLongTimeString());
            string[] files=Directory.GetFiles(imagesPath);
            long totalSize = 0;
            foreach (string file in files)
            {
                FileInfo fInfo = new FileInfo(file);
                totalSize += fInfo.Length;
                Thread.Sleep(10); // işlemleri biraz geciktirmek için bilinçli olarak konulmuştur
            }
            return totalSize;
        }

        static int GetBmpCount()
        {
            Console.WriteLine("\t GetBmpCount metodu için Managed Thread Id {0} Zaman {1}", Thread.CurrentThread.ManagedThreadId.ToString(), DateTime.Now.ToLongTimeString());
            int result = 0;

            foreach (string file in Directory.GetFiles(imagesPath))
            {
                FileInfo fInfo = new FileInfo(file);
                Thread.Sleep(10); // işlemleri biraz geciktirmek için bilinçli olarak konulmuştur
                if (fInfo.Extension.Contains("bmp"))
                    result++;
            }

            return result;
        }

        static void CopyBmp()
        {
            Console.WriteLine("\t CopyBmp metodu için Managed Thread Id {0} Zaman {1}", Thread.CurrentThread.ManagedThreadId.ToString(), DateTime.Now.ToLongTimeString());
            foreach (string file in Directory.GetFiles(imagesPath))
            {
                FileInfo fInfo = new FileInfo(file);
                if (fInfo.Extension.Contains("bmp"))
                {
                    File.Copy(file, "C:\\Bitmaps\\"+ fInfo.Name,true);
                }
            }
            Console.WriteLine("Kopyalama işlemi tamamlandı");
        }
    }
}
```

Şunu hemen belirteyim; aslında Directory ve FileInfo sınıflarının söz konusu hesaplamalar için kolaylaştırıcı metodları zaten mevcut. Söz gelimi Directory sınıfının GetFiles metoduna filtre uygulayarak zaten bmp dosyalarını kolayca elde edebiliriz. Yada bmp dosyalarını ele alırken kopyalama işlemlerinide yapabiliriz. Ancak yazının başında da bahsettiğim üzere bu sadece örnek bir senaryo malzemesi. Önemli olan nokta GetTotalSize, GetBmpCount ve CopyBmp metodlarının paralel olarak çalıştırılmalarını sağlamak. Tabi şu andaki kod parçamız bu metodları ardışık (Sequential) olarak çalıştırmaktadır. Uygulamanın çalışma zamanı çıktısına baktığımızda ise aşağıdaki ekran görütüsündekine benzer sonuçları alırız.

![blg27_3.gif](/assets/images/2009/blg27_3.gif)

Sanıyorumki metodların başlangıç zamanları ve aralarındaki farklar dikkatinizi çekmiştir. Bu zaten beklediğimiz bir sonuçtur aslında. Nitekim metod görevlerinin paralel olarak ele alınması için hiç bir şey yapmadık. Kodu paralel programlama felsefesine taşımak için aşağıdaki gibi değiştirmemiz gerekmektedir.

```csharp
static void Main(string[] args)
{
	Task[] tasks =
	{
		Task<long>.Factory.StartNew(GetTotalSize),
		Task<int>.Factory.StartNew(GetBmpCount),
		Task.Factory.StartNew(CopyBmp)
	};

	/* tasks isimli dizi içerisindeki Task<T> tipleri aynı generic tip ile kullanılmadıklarında Task<T>[] gibi bir dizi üretilememiş bu nedenle 0 ve 1nci indislerdeki Task tiplerinin Result özelliklerine ulaşabilmek için bilinçli olarak Task<T> tiplerine dönüşüm yapılmıştır. */
	Console.WriteLine("Toplam boyut {0} byte\nBmp sayısı {1}"
		, ((Task<long>)tasks[0]).Result.ToString()
		, ((Task<int>)tasks[1]).Result.ToString()
		);

	Console.WriteLine("Devam etmek için bir tuşa basınız");
	Console.Read();
}
```

İlk olarak Task tipinden bir dizi ürettildiğini görmekteyiz ki bir dizi kullanmanın bir zorunluluk olmadığını biraz sonra göreceğiz. Nihayetinde elimizde birden fazla görev var. Bu ilk kod denememizde, görevlerin tamamı bir dizi içerisinde toplanmaktadır. Dizinin her bir elemanının oluşturulması sırasında Factory özelliği üzerinden StartNew metodunun çağırıldığına dikkat edelim. Bu noktada parametre olarak belirtilen metodların, Task'ler diziye eklenirken çalıştırıldığını söyleyebiliriz. Kodun devam eden kısmında ise, generic Task tiplerinin çalıştırdığı metodlardan gelen dönüş değerleri ele alınmak istenmektedir. Dönüş değerlerinin ele alınması sırasında bilinçli olarak tür dönüşümü yapıldığına dikkat edilmelidir. Nitekim, dönüş değerleri ancak Task generic sınıfının Result özellliği üzerinden ele alınabilmektedir.

> NOT: Tabiki bazı senaryolarda, tüm görevler aynı dönüş tipine sahip olabilirler. Bu durumda dizinin Task tipinden tasarlanmış olması halinde, dönüştürme işlemlerine gerek olmadan sonuçlar alınabilir. Nitekim dizi üzerinde hareket edecek basit bir for each döngüsünün ele alacağı her bir eleman Task tipinde olacağından, zaten Result özelliklerine otomatikman ulaşılabilecektir.

Bu noktada şunu vurgulamaktada yarar var; bazı durumlarda paralel çalışan metodların işlemlerini tamamlamadan kodun devam etmesi istenmeyebilir. Bu durumdada Task sınıfının static WaitAll veya WaitAny gibi metodlarını kullanarak gereken bekletmeleri yapabiliriz. Örneğimizde buna gerek kalmamıştır. Çünkü generic Task tiplerinin işaret ettiği metodlara ait dönüş tipleri alınmak istendiğinden, uygulama kodu zaten o anda sonuç gelmediyse mecburen beklemede kalacaktır. Peki örneği çalıştırdığımızda nasıl bir sonuç alırız.

![blg27_4.gif](/assets/images/2009/blg27_4.gif)

Mutlaka dikkatinizi çekmiştir; her metod için ayrı bir Managed Thread Id değeri üretilmektedir. Oysaki ardışık (Sequential) çalışan modelde tüm metodlar aynı Thread içerisinde ele alınmıştır. Bu, Thread bölünümünün de bir göstergesidir. Diğer taraftan, metodlar arası süre farklılıkları neredeyse sıfıra yakındır. Görüldüğü gibi gayet basit bir şekilde işlemleri paralel hale getirmeyi başardık. Kod ile ilişkili önemli bir noktayı daha vurgulamak isterim. Biraz önce bahsettiğimiz gibi, aynı dönüş tipine sahip metodların kullanıldığı senaryolarda Task dizilerini kullanmak daha mantıklıdır. Bu nedenle yukarıdaki senaryoda yer alan kodda dizi kullanımı şart değildir. Bir başka deyişle aynı amacı yerine getirden bir kod parçası, aşağıdaki şekilde olduğu gibi ele alınabilir.

```csharp
Task<long> task1=Task<long>.Factory.StartNew(GetTotalSize);
Task<int> task2=Task<int>.Factory.StartNew(GetBmpCount);
Task task3=Task.Factory.StartNew(CopyBmp);

Console.WriteLine("Toplam boyut {0} byte\nBmp sayısı {1}"
                    , task1.Result.ToString()
                    , task2.Result.ToString()
                    );
```

Bu sefer GetTotalSize ve GetBmpCount metodlarını kullanan Task tiplerine ait çalışma zamanı referansları, birer değişkene atanarak kullanılmışlardır. Bu durumda bir önceki örnekte yaptığımız gibi Result özelliğine erişmek için, cast işlemi yapılmasına da gerek kalmamaktadır ki bu oldukça doğru bir yoldur. Dolayısıyla kodu daha düzgün bir hale getirmiş bulunuyoruz. Sizlerde Task ve Task sınıflarını kullanarak bir kaç antrenman yapmayı deneyebilirsiniz.

> Visual Studio 2010 ile birlikte gelen Parallel Tasks ve Parallel Stacks debugger pencereleri yardımıyla çalışma zamanında, task ve thread'lerin durumunu daha net bir şekilde analiz edebilirsiniz. Bu konuyu bir görsel dersimizde ele almaya çalışacağım.

Tabiki konuyu daha derinlere genişletmek mümkündür. Örneğin bazı görevlerin, kendinden önceki görev (ler) tamamlandıktan sonra başlatılması istenebilir. İşte diğer blog yazımın konusunu şimdiden bulduk.

![Wink](/assets/images/2009/smiley-wink.gif)

Tabi başımıza dert açacak daha pek çok konuda var. Söz gelimi, TPL alt yapısını WinForms yada WPF gibi uygulamalarda ele aldığımızda neler olacaktır kimbilir

![Undecided](/assets/images/2009/smiley-undecided.gif)

. Malum WinForms yada WPF ekranlarında, Main Thread bencillik edip ekran üzerindeki kontrolleri başka Thread'ler ile paylaşmak istemez. Bu bencilliğe ortak olduğumuzda WinForms tarafında Illegal Cross Thread istisnalarına düştüğümüzü gayet iyi biliyoruz. Bu ve benzeri diğer konuları ilerleyen zamanlarda irdelemeye devam ediyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloTasks.rar (23,71 kb)](/assets/files/2009/HelloTasks.rar)