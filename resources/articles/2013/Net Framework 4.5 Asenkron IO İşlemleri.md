---
title: ".Net Framework 4.5 Asenkron IO İşlemleri"
pubDate: 2013-01-25 02:27:00
categories:
  - .Net Framework 4.5
tags:
  - .Net Framework 4.5
  - C#
  - .NET
  - ASP.NET
  - LINQ
  - SQL Server
  - WPF
  - Windows Forms
  - Async/Await
  - Task Parallel Library
  - Threading
  - Concurrency
  - Performance
  - Generics
---

# .Net Framework 4.5 Asenkron IO İşlemleri
[![WAR2](images/WAR2_thumb.jpg)](images/WAR2.jpg) Merhaba Arkadaşlar,

Geçtiğimiz gün özlem duyduğum bilgisayar oyunlarından birisi olan Warcraft II'nin ses efektlerini arar halde buldum kendimi. Olay tabi ses efektlerinin mükemmelliğinden çıktı oyuncu karakterlerine kadar geldi. Gerek Orc'lar da gerek Human'lar da süper kahramanlar vardı. Büyücüler, okçular, işçiler ve daha niceleri. Pek çoğumuzun bu oyun başında saatler harcadığından ve sabah ezanına kadar kaldığından eminim.

Nedense söz konusu karakterleri bir araştırma konusunda kullanma ihtiyacı da hissettim. Tesadüfe bakın ki aynı anda.Net tarafında da bir konuyu araştırmaktaydım. Sonuç olarak aşağıdaki konu için onları kendi bakış açımdan değerlendirmeye karar verdim. Bakalım bu günkü yazımızda nasıl bir maceraya dalıyor olacağız

![Winking smile](images/wlEmoticon-winkingsmile_117.png)

Haydi buyrun öyleyse başlayalım.

Asenkron çalışma çok uzun zamandır hayatımızda. Ancak yazılım geliştiriciler için halen daha tam anlamıyla mükemmel değil. Özellikle.Net Framework açısından olaya baktığımızda. Yine de her.Net Framework sürümünde, User Experience'ın yoğun ve cevap verebilirliği yüksek ekranların tasarlanmaya çalışıldığı senaryolarda, geliştiricilerin ellerinde daha güçlü kozlar oluşmakta. Burada en büyük yardımcı tabiki Framework içerisine dahil edilen Built-In operasyonlar/fonksiyonlar.

Bu alanlardan birisi de tahmin edileceği üzere dosya giriş çıkış işlemleri (IO operasyonları). Dosyalama işlemleri özellikle.Net Framework 4.0 sürümüne kadar senkron (Synchronous) olarak işletilebilen operasyonlardan ibaretti. Diğer yandan.Net Framework 4.0 sürümü ile birlikte StreamReader ve StreamWriter gibi sınıflara BeginRead, BeginWrite gibi temel asenkron çalışabilme metodları ilave edildi.

> Bilindiği üzere en basit mana da bir işlevi başlatıp anında koda dönebilmek için IAsyncResult arayüzünün (Interface) kullanıldığın BeginX, EndX gibi dahili metodlardan yararlanılır. Bu, olay tabanlı asenkron programlamaya (Event Based Asynchronous Programming) da olanak tanımaktadır.

Bu yeni metodlar sayesinden dosyalara asenkron olarak yazmak veya kaynakta yine asenkron olarak okuma yapmak mümkün. Yine de ortada geliştiriciler için bir sıkıntı var. O da kodun karmaşıklığı ve yazım zorluğu.

IO işlemlerinde neden asenkron çalışmaya ihtiyaç duyarız?

Bu soruyu da cevaplamak önemli. İlk etapta kullanıcıların ele aldıkları ekranların cevap verebilirliği (Responsivable) yer almakta. Kullanıcılar sabırsız kişilikte insanlar. O nedenle özellikle bir IO işlemi sırasına ekranın kitlenmesini (kitlenmiş gibi gözükmesini) istemezler. Diğer yandan ikinci bir sebep te tamamen performans ile alakalıdır. Bazı hallerde IO işlemi gerçekten çok zaman alan ve sistem kaynaklarını önemli ölçüde tüketen hareketliliklere gebedir. Dolayısıya bu yükü asenkron olarak dağıtabilmek önemlidir.

Pek tabi.Net Framework 4.5 ile birlikte özellikle async ve await anahtar kelimelerinin de devreye girmesi ile IO tarafı için de bazı iyileştirmeler yapıldı. Bu iyileştirmeler sayesinde IO işlemleri sırasında cevap verebilirliği yüksek olan ekranların tasarlanması daha kolay hale gelmekte. Buna ek olarak CLR (Common Language Runtime) takımı bu yeni fonksiyonellikler için eş zamanlı (Concurrent) çalışmanın performansını da arttırıcı iyileştirmeler yapmış. Ancak bana göre belki de en önemli avatajımız, geliştirici açısından bakıldığında, ihtiyaçların daha kolay yazılabiliyor olması.

> Gerçek hayattan;
> daha önceden çalıştığım bir banka da, sistemler arasında veri taşınması veya dışarıdan gelen bazı verilerin database ortamlarına alınması, büyük boyutlu ve genellikle text tabanlı olan dosyalar üzerinden yapılmaktaydı. Çoğunlukla otomatik olarak devreye giren ve bir SQL Job ile ilişkilendirilmiş olan SSIS (Sql Server Integration Services) paketleri söz konusuydu.
> Ancak SSIS'de ayrı bir uzmanlık alanı gerektirmekte ve bazı durumlarda söz konusu dosya aktarım işlemleri, Form tabanlı uygulamalar içerisinden yapılmak zorundaydı.
> Hal böyle olunca bazı vakalarda, boyutu 650 megabyte'ın üzerinde olan dosyalar için uygulama ekranının kilitlenmesi de son derece doğaldı. Hele de bu bir banka olunca süreci asenkron olarak çalıştırmak şarttır. İşte size Asenkron IO işlemleri için kocaman bir sebep
>
> ![Smile](images/wlEmoticon-smile_49.png)

Yeni Fonksiyonlar

Biz bu makalemizde özellikle StreamReader ve StreamWriter sınıflarına gelen ve asenkron IO işlemlerini yapmamızı sağlayan yeni metodları incelemeye çalışıyor olacağız. Konsept olarak sadece kullanım şekillerini incelemek niyetindeyiz. Bu sebepten senaryolarımızı bir Console uygulaması üzerinden ele alacağız. StreamReader ve StreamWriter gibi IO sınfılarına ilave edilen asenkron metodları bulmamız aslında son derece kolay. İsimlendirme standardı olarak sonu async kelimesi ile biten metodlar aradığımız fonksiyonlar olacaktır

![Winking smile](images/wlEmoticon-winkingsmile_117.png)

Dilerseniz işe ilk olarak asenkron yazma işlemleri ile başlayalım ki elimizde büyükçe bir test dosyası da oluşsun

![Smile](images/wlEmoticon-smile_49.png)

Bu amaçla basit ama işi zevkli hale getirecek de bir ön hazırlık yapacağız. Senaryomuzda en az 10milyon elementten oluşan bir oyun sahası bulunacak. Bu saha içerisinde okçu, şovalye, mancınık, kale gibi karekterlere yer vereceğiz. Bu karakterlere ait başka bilgileri de tutuabiliriz. Örneğin bulundukları kıta, harita üzerindeki koordinat bilgisi, güç seviyleri, hangi taraftan oldukları (insan, bilgisayar, hayalet ![Winking smile](images/wlEmoticon-winkingsmile_117.png)) gibi.

E tabi bu karakterlerden oluşan kümeyi üretecek bir de yardımcı tipimizin söz konusu olduğunu düşünebiliriz. Lafı fazla uzatmadan ön hazırlık için kullanacağımız kod parçalarına bir göz atalım dilerseniz.

[![aio_1](images/aio_1_thumb.png)](images/aio_1.png)

GameElement.cs;

```csharp
using System;

namespace NewIOFunctions 
{ 
    public class GameElement 
    { 
        public int Id { get; set; } 
        public string Actor { get; set; } 
        public int Force { get; set; } 
        public string Type { get; set; } 
        public string MainLand { get; set; } 
        public int X { get; set; } 
        public int Y { get; set; }

        public override string ToString() 
        { 
            return String.Format("{0}|{1}|{2}|{3}|{4}|({5};{6})", Id.ToString(), Actor, Force, Type, MainLand,X.ToString(),Y.ToString()); 
        } 
    } 
}
```

GameCreator.cs;

```csharp
using System; 
using System.Collections.Generic;

namespace NewIOFunctions 
{ 
    public static class GameCreator 
    { 
        // Test verileri için örnek bir küme 
        // Savaş alanına yerleştireceğimiz aktörler 
        static string[] actors = { "Archer", "Footman", "Knight", "Castle", "Rider", "Catapult" }; 
        // Bu aktörlerin güçlerini temsil eden değerler. 
        static int[] forceValues = { 100, 200, 300, 400 }; 
        // Bu yerleştirilen aktörlerin hangi tipten olduğunu belirtecek taraf bilgileri(Ghost enteresan bir karakterdir. Yeri gelince Human yeri gelince Computer tarafında oluyor :) 
        static string[] types = { "Human", "Computer", "Ghost" }; 
        static string[] mainlands = { "Mordor", "Middle Earth", "Other Side", "Wrong Side", "Red Zone", "Deadly Land", "Kolburg" };

        public static List<GameElement> CreateRandomElements(int size) 
        { 
            List<GameElement> elements = new List<GameElement>(size); 
            Random randomizer = new Random(); 
            for (int i = 0; i < size-1; i++) 
            { 
                GameElement element = new GameElement(); 
                element.Id = i; 
                element.Actor = actors[randomizer.Next(0, actors.Length - 1)]; 
                element.Force=forceValues[randomizer.Next(0, forceValues.Length - 1)]; 
                element.Type = types[randomizer.Next(0, types.Length - 1)]; 
                element.MainLand = mainlands[randomizer.Next(0, mainlands.Length - 1)]; 
                element.X = randomizer.Next(-90, 90); 
                element.Y = randomizer.Next(-90, 90); 
                elements.Add(element); 
            } 
            return elements; 
        } 
    } 
}
```

static GameCreator sınıfının CreateRandomElements isimli metodu istenen boyuta göre rastgele veriler ile üretilen GameElement tipinden bir koleksiyon döndürecek şekilde tasarlanmıştır.

> Dilerseniz buradaki üretim işini Task Parallel Library’ yi ve Concurrent koleksiyonları da işin içerisine katarak asenkron doldurabilirsiniz. Ama dikkatli olun. Her üretiminizde 10milyonluk bir set elde edemessiniz. Tabi Thread’ leri en azından gereken yerlerde kilitleyip senkronize etmedikçe
>
> ![Winking smile](images/wlEmoticon-winkingsmile_117.png)

Şimdi asıl senaryolarımıza geçebiliriz. İlk etapta üretilen bu rastgele veri içeriğini fiziki bir dosyaya asenkron moda da nasıl yazdırabileceğimizi incelemeye çalışacağız. Bu nedenle Program.cs içeriğinde aşağıdaki kodlar ile işe başlayabiliriz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Diagnostics; 
using System.IO; 
using System.Threading.Tasks;

namespace NewIOFunctions 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            //10 milyonluk bir test kümesi üretiyoruz. Bunu asenkron geliştirmediğimizden biraz zaman alacaktır. 
            var gameZone = GameCreator.CreateRandomElements(10000000); 
            var taskResult=WriteFileAsync(gameZone, Path.Combine(Environment.CurrentDirectory, "GameZone.txt")); 
            Console.WriteLine("İşlemler devam ediyor..."); 
            // Dosya yazma ile ilişkili işlemlerin tamamlanması için uygulamayı bekletiyoruz. 
            taskResult.Wait(); 
            Console.WriteLine("{0} adet satır yazıldığı bilgisi geldi.", taskResult.Result.ToString()); 
        }

        // WriteFileAsync metodundan geriye bilinçli olarak bir Task tipi döndürdük. Böylece burayı çağırdığımız Main metodu sonuna gelindiğinde ilgili Task' ın tamamlanıp tamamlanmadığını kontrol edebiliriz. 
        private static async Task<int> WriteFileAsync(List<GameElement> elements, string filePath) 
        { 
            Console.WriteLine("Yazma işlemi başlatıldı.");

            Stopwatch watcher = new Stopwatch(); 
            watcher.Start();

            // StreamWriter tipini üretiyoruz ve parametre olarak veriyi yazacağımız dosya adresini belirtiyoruz 
            using (StreamWriter writer = new StreamWriter(filePath)) 
            { 
                foreach (var element in elements) 
                { 
                    // Awaitable olan WriteLineAsync metodunu çağırıyor ve içeriği yazdırıyoruz. 
                    await writer.WriteLineAsync(element.ToString()); 
                }    
            } 
            
            watcher.Stop(); 
            Console.WriteLine("Yazma işlemi tamamlandı. Toplam süre {0} milisaniye.",watcher.ElapsedMilliseconds.ToString());

            return elements.Count; 
        } 
    } 
}
```

Örneğimizi çalıştırdığımızda aşağıdakine benzer bir sonuç ile karşılaşırız.

[![aio_2](images/aio_2_thumb.png)](images/aio_2.png)

Üretilen dosya içeriğinin bir kısmı da aşağıdaki gibidir.

> Tabi bu dosyayı Notepad açmayı nasıl başardı anlayabilmiş değilim
>
> ![Smile](images/wlEmoticon-smile_49.png)
>
> Yine de siz kendi sistemlerinizde zorlanmamak adına, Notepad2' yi veya Notepad++' ı kullanmayı düşünebilirsiniz.

[![aio_3](images/aio_3_thumb.png)](images/aio_3.png)

Dikkat edileceği üzere WriteFileAsync'e yapılan çağrı içerisinde StreamWriter tipinin await edilebilir WriteLineAsync metodu kullanılmaktadır. WriteFileAsync metodu aslında geriye bir Task tipi döndürmek zorunda değildir. Bunu daha çok, Wait metodu kullanılarak başlatılan Task'ın işlemlerini bitirene kadar uygulamanın beklemesi ve işlenen satır sayısının bir değerinin ana Thread tarafından elde edilebilmesi için kullandık. Nitekim uygulamayı akışına bırakıp Wait çağrısını gerçekleştirmessek, tüm içerik yerine program sonlanana kadar yazılabilen kısım fiziki olarak aktarılacaktır.

> Örnek senaryonun işletilişi sırasında gözden kaçırılmaması gereken bir nokta da, aslında dosyaya yazma işleminin asenkron olarak gerçekleştirilmemesidir. Yani, GameElement tipindeki koleksiyon içeriğinin eş zamanlı (Concurrent) olarak yazılması gibi bir durum ortada yoktur. Dosya içeriğine baktığımızda, içeriğin sıralı olarak yazılmış olmasının nedeni de budur. Yanlış bir asenkron algı oluşmaması için bu hususu belirtmek istedim. Nitekim dosya içeriğini asenkron olarak doldurmak tamemen farklı bir vakadır.

Dilerseniz bir de dosyadan okuma işlemini basit bir senaryo ile ele almaya çalışalım. Yine satır satır okuma adımını gerçekleştirebiliriz. İşte kod içeriğimiz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Diagnostics; 
using System.IO; 
using System.Threading.Tasks;

namespace NewIOFunctions 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            var readTaskResult = ReadFileLineByLineAsync(Path.Combine(Environment.CurrentDirectory, "GameZone.txt")); 
            Console.WriteLine("İşlemler devam ediyor..."); 
            readTaskResult.Wait(); 
            Console.WriteLine("Sonuçlar"); 
            for (int i = 0; i < 10; i++) 
            { 
                Console.WriteLine(readTaskResult.Result[i].ToString()); 
            } 
        }

        private static async Task<List<GameElement>> ReadFileLineByLineAsync(string file) 
        { 
            Console.WriteLine("Okuma işlemi başlatıldı."); 
            List<GameElement> elements = new List<GameElement>(); 
            Stopwatch watcher = new Stopwatch(); 
            watcher.Start();

            using (StreamReader reader = new StreamReader(file)) 
            { 
                string line; 
                while (!String.IsNullOrEmpty(line = await reader.ReadLineAsync())) 
                { 
                    string[] columns=line.Split('|'); 
                    elements.Add(new GameElement 
                    { 
                        Id=Convert.ToInt32(columns[0]), 
                        Actor=columns[1], 
                        Force=Convert.ToInt32(columns[2]), 
                        Type=columns[3], 
                        MainLand=columns[4], 
                        X=Convert.ToInt32(columns[5].Split(';')[0].TrimStart('(')), 
                        Y = Convert.ToInt32(columns[5].Split(';')[1].TrimEnd(')')) 
                    } 
                    ); 
                } 
            }

            watcher.Stop(); 
            Console.WriteLine("Okuma işlemi tamamlandı. Toplam süre {0} milisaniye.", watcher.ElapsedMilliseconds.ToString());

            return elements; 
        }        
    } 
}
```

Okuma işleminin tamamlanmasını takiben,üretilen GameElement içerikli generic List koleksiyonunun metoddan neden bu şekilde bir Task tipi ile döndürüldüğü noktasında kafalarda bir soru işaret olabilir. Neden normal bir liste döndürmedik? Ya da neden out veya ref işaretli metod parametrelerine başvurmadık? Cevap async kullanmış olmamızdır. Bu şekilde işaretlenmiş metodlar void, Task ya da Task dönüşüne sahip olabilir. Ayrıca ref ve out kullanımına izin verilmez.

Örneğimizin çalışması tabiki biraz uzun sürebilir. Kolay değil yaklaşık 450 megabyte'lık bir içeriğin okunması söz konusu. İşte örnek ekran çıktımız.

[![aio_6](images/aio_6_thumb.png)](images/aio_6.png)

Her iki örnekte de önemli olan, dosyadan satır bazında okuma ve dosyaya satır bazında yazma işlemlerini içeren fonksiyonelliklerin ana uygulamadan bağımsız olarak asenkron çalışabiliyor olmalarıdır. Bir başka deyişle eş zamanlı (Concurrent) olarak bir dosya içerisine yazma veya okuma işlemi söz konusu değildir.

Async olarak kullanılabilecek başka IO metodları da mevcuttur. Bunları aşağıdaki şekilde sıralayabiliriz.

- ReadAsync
- ReadToEndAsync
- ReadBlockAsync
- WriteAsync
- WriteLineAsync
- FlushAsync

Parçalanmış Asenkronluk

Son bir senaryo ile yazımıza devam edelim dilerseniz

![Winking smile](images/wlEmoticon-winkingsmile_117.png)

Bu kez 10milyon element'ten oluşan sahımızı 10 parçaya bölüp her bir parça içerisinde ayrı bir Text dosyaya yazma işlemini, asenkron manada simüle etmeye çalışıyor olacağız. İşte senaryoya istinaden ele alabileceğimiz kod parçaları.

```csharp
using System; 
using System.Collections.Generic; 
using System.Diagnostics; 
using System.IO; 
using System.Linq; 
using System.Threading.Tasks;

namespace NewIOFunctions 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            //10 milyonluk bir test kümesi üretiyoruz. Bunu asenkron geliştirmediğimizden biraz zaman alacaktır. 
            var gameZone = GameCreator.CreateRandomElements(10000000); 
            var r=WriteTo10File(gameZone); 
            Console.WriteLine("İşlemler devam ediyor...Lütfen bekleyiniz"); 
            r.Wait(); 
            Console.WriteLine("Program sonlandırılıyor"); 
        }

        private static async Task WriteTo10File(List<GameElement> elements) 
        { 
            for (int i = 1; i <= 10; i++) 
            { 
                string fileName = Path.Combine(Environment.CurrentDirectory, "GameZone_" + i.ToString() + ".txt"); 
                await WriteFileAsyncV2(elements.Skip((i-1)*1000000).Take(1000000).ToList(), fileName); 
            } 
            Console.WriteLine("Tüm dosya yazma işlemleri tamamlandı."); 
        }

        private static async Task WriteFileAsyncV2(List<GameElement> elements, string filePath) 
        { 
            using (StreamWriter writer = new StreamWriter(filePath)) 
            { 
                foreach (var element in elements) 
                { 
                    // Awaitable olan WriteLineAsync metodunu çağırıyor ve içeriği yazdırıyoruz. 
                    await writer.WriteLineAsync(element.ToString()); 
                } 
                Console.WriteLine("{0} için işlemler tamamlandı.",filePath); 
            } 
        } 
    } 
}
```

Hem WriteTo10File hem de WriteFileAsyncV2 metodları async olarak işaretlenmiş olup kendi içlerinde await edilebilir operasyon çağrılarına yer vermektedirler. İlk olarak 10milyonluk küme 10 eşit parça şeklinde ele alınmakta ve her bir alt kümenin asenkron bir metod ile çalışacak şekilde ilgili dosyalara yazdırılması işlemi gerçekleştirilmektedir.

Dosyaya satır bazlı yazma işlemi için yine WriteLineAsync fonksiyonunundan yararlanılmaktadır. WriteTo10File metoduda kendi içerisinde 1milyonluk kümelere ayırdığı element listelerini ayrı dosyalara yazmak için WriteFileAsyncV2 metodunu await anahtar kelimesi ile çağırmaktadır ki bu da gözden kaçırılmaması gereken bir noktadır. Kodun çalışma zamanı çıktısı aşağıda görüldüğü gibidir.

[![aio_7](images/aio_7_thumb.png)](images/aio_7.png)

Tabi klasör yapısına bakıldığında GameZone_1’ den GameZone_10’ a kadar 10 farklı dosyanın üretildiği ve içerisine de 10milyon kümenin 1milyon parçalarının yazıldığı görülebilir.

[![aio_8](images/aio_8_thumb.png)](images/aio_8.png)

Senaryolarımız ile Console üzerinden basit pratiklerimizi yapmış olduk. Şimdi bu pratikleri gerçek hayat senaryoları ile değerlendirmeye çalışmalıyız. Dolayısıyla işlemlerimizi artık görsel arabirimi olan ve gerçekten de cevap verilebilirliğe ihtiyaç duyan Windows Forms, WPF (Windows Presentation Foundation) ve belki de Asp.Net gibi uygulama çeşitlerinde ele almalıyız. Her zaman olduğu gibi bu kutsal görevi siz değerli meslektaşlarıma bırakıyorum

![Smile](images/wlEmoticon-smile_49.png)

Böylece geldik bir yazmızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[NewIOFunctions.zip (48,16 kb)](assets/NewIOFunctions.zip)
