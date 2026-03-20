---
layout: post
title: "Paralel Programlamada İstisna Yönetimi"
date: 2011-01-13 13:30:00 +0300
categories:
  - parallel-programming
tags:
  - parallel-programming
  - csharp
  - linq
  - task-parallel-library
  - threading
  - delegates
  - visual-studio
---
Balık tutmak farklı bir hobidir. Özellikle olta ile balık yakalamaktan büyük keyif alanlar vardır. (Hatta laf aramızda, şirkette yanımda oturan çalışma arkadaşımın çekmecesinde, bir olta takımı var)

[![blg208_Giris](/assets/images/2011/blg208_Giris_thumb.jpg)](/assets/images/2011/blg208_Giris.jpg)


Çoğu zaman Haliç köprüsü gibi alanlarda yandaki resimde olduğu gibi bu işin sevdalılarını görebiliriz. Kimisi sabahın erken saatlerinde gelip, akşamın geç saatlerine kadar burada olta sallar ve “Rastgele” der.

Benim ne yazık ki bu tip bir hobim yok. Hatta bu günkü yazımızda sizlere balık yakalamayı da anlatacak değilim. Onun yerine Task Parallel Library için istisna yakalama vakaları üzerinde duracağım.

Task örneklerinin kullanıldığı senaryolarda, bloklar içerisinde yer alan işlevselliklerin doğurabileceği çalışma zamanı istisnalarını ele almak, son derece önemlidir. Nitekim paralel çalışmakta olan blokların beklenmedik bir şekilde sonlandırılması söz konusudur. İşte bu yazımızda Task örnekleri içerisinde oluşabilecek istisnaların nasıl ele alınabileceğini incemelye çalışıyor olacağız.

## Wait, WaitAll, WaitAny Tetikleyicileri

Bir veya daha fazla Task örneği tarafından başlatılan paralel işleyişlerde, bekletme metodlarının çağırılması halinde, ortama fırlayabilecek Exception örneklerinin yakalanması mümkündür. Aşağıdaki kod parçasında 3 farklı Task bloğu için bir istisna senaryosu ele alınmaya çalışılmıştır.

```csharp
using System; 
using System.IO; 
using System.Linq; 
using System.Threading.Tasks;

namespace HandlingExcpetions 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Task task1 = new Task(() => 
                { 
                    File.Open("C:\\OlmayanDosya.txt", FileMode.Open); 
                } 
            ); 
            Task task2 = new Task(() => 
                { 
                    string number = "oniki"; 
                    double point = Convert.ToDouble(number); 
                } 
            ); 
            Task task3 = new Task(() => 
                { 
                    for (int i = 0; i < 100; i++) 
                    { 
                        i++; 
                        i--; 
                        i *= 1; 
                        Console.Write("."); 
                    } 
                } 
            );

            task3.Start(); 
            task2.Start(); 
            task1.Start();

            try 
            { 
                Task.WaitAll(task1, task2, task3); 
            } 
            catch (AggregateException agrExcp) 
            { 
                Console.WriteLine("\nOluşan istisnalar"); 
                var excpInfos = from e in agrExcp.InnerExceptions 
                                select new 
                                {                        
                                    e.TargetSite, 
                                    e.Message, 
                                    InnerException=e.InnerException!=null?e.InnerException.Message:"InnerException bilgisi yok" 
                                }; 
                foreach (var excp in excpInfos) 
                { 
                    Console.WriteLine("\n{0}\n{1}\n{2}",excp.Message,excp.InnerException,excp.TargetSite); 
                } 
            } 
        } 
    } 
}
```

Örnek kod parçasında 3 farklı Task örneği oluşturulduğu görülmektedir. Task1 içerisinde sistemde olmadığı düşünülen bir dosya açılmaya çalışılmaktadır. Task 2 ile alakalı blok içerisinde ise, metinsel bir ifadenin sayısal dönüştürülmesi söz konusudur. Task 3 ile ilişkili kod bloğunda ise herhangibir istisna durumu söz konusu değildir. Örnek uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan çalışma zamanı çıktısı ile karşılaşırız.

[![blg208_Runtime1](/assets/images/2011/blg208_Runtime1_thumb.gif)](/assets/images/2011/blg208_Runtime1.gif)

Burada dikkat edilmesi gereken noktalardan birisi, Task 1 ve Task 2 içerisinde oluşan istisnaların Task 3’ ün başlattığı bloğun çalışmasını etkilememesidir. Diğer yandan oluşan istisnaların tamamı, AggregateException tipinin InnerExceptions özelliği içerisinde toplanmaktadır.

Aslında try…catch bloğu içerisine yer alan Task.WaitAll metodu çağrısı ile, tüm Task örneklerinin işleyişleri bitene kadar, bu işleyişlerin sahibi olan Main Thread’ in duraksatılması sağlanmaktadır. WaitAll metodunun try…catch bloğu içerisine olması nedeniyle de, tamamlanması beklenen Task bloklarında oluşan istisnalar, AggregateException tarafından toplanmatadır. Eğer catch bloğu içerisinde breakpoint konularak durulur ve yerel değişkenlere bakılırsa, aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaşılır.

[![blg208_Debug1](/assets/images/2011/blg208_Debug1_thumb.gif)](/assets/images/2011/blg208_Debug1.gif)

Burada dikkat edilmesi gereken en önemli noktalardan birisi de, Task örneklerinin Status özelliklerinin değerleridir. Dikkat edileceği üzere task1 ve task2 isimli örnekler Faulted durumunda kalmışlardır. Bu değerler, uygulamalarda olup biten herşeyi tutan log mekanizmaları için veya iş sürecinin akan diğer kısımları için önemlidir. Çok doğal olarak task3 örneğinin durumu RanToCompletion olarak kalmıştır. Yani başarılı bir şekilde işleyişini tamamlamıştır. Gelelim diğer bir mevzuya…

## AggregateException Nesne Örneğine Üzerinden Handle Metodunun Kullanılması

AggregateException nesne örneği üzerinden erişilebilen Handle metodu ile, Task örneklerine ait bloklar içerisinde oluşacak istisnalar arasında dolaşılabilmektedir. Özellikle n sayıda Task bloğunun takip edildiği durumlarda, beklenen bir istisnanın oluşması halinde nasıl hareket edileceğine karar vermek için kullanılabilecek yollardan birisidir.

Bu metod parametre olarak Func tipinden bir Temsilci (Delegate) almaktadır. Buna göre hangi istisnanın ele alınmak istediği ilk parametre ile belirtilmektedir. Diğer yandan söz konusu temsilci geriye bool değer döndürecek şekilde ayarlanmıştır. Bu değer spesifik olarak belirlenen istisnanın/istisnaların ele alınması halinde true olmalıdır. Handle kullanımını daha iyi kavrayabilmek adına aşağıdaki örnek kod parçasını göz önüne alalım.

```csharp
using System; 
using System.IO; 
using System.Linq; 
using System.Threading.Tasks;

namespace HandlingExcpetions 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Task task1 = new Task(() => 
            { 
                File.Open("C:\\OlmayanDosya.txt", FileMode.Open); 
            } 
            ); 
            Task task2 = new Task(() => 
            { 
                string number = "oniki"; 
                double point = Convert.ToDouble(number); 
            } 
            ); 
            Task task3 = new Task(() => 
            { 
                for (int i = 0; i < 100; i++) 
                { 
                    i++; 
                    i--; 
                    i *= 1; 
                    Console.Write("."); 
                } 
            } 
            );

            task3.Start(); 
            task2.Start(); 
            task1.Start();

            try 
            { 
                Task.WaitAll(task1, task2, task3); 
            } 
            catch (AggregateException agrExcp) 
            { 
                agrExcp.Handle(e => 
                    { 
                        if (e is FileNotFoundException) 
                        { 
                            Console.WriteLine("Dosya bulunamadı"); 
                            return true; 
                        } 
                        else if (e is FormatException) 
                        { 
                            Console.WriteLine("Dönüştürme hatası"); 
                            return true; 
                        } 
                        else 
                           return false; 
                    } 
                ); 
        } 
    } 
}
```

Bir önceki örnekte yer alan senaryonun aynısı söz konusudur. Ancak bu kez catch bloğu içerisinde Handle metodu kullanılmıştır. Handle ile işaret edilen blokta, e ile temsil edilen referansın olası değerleri kontrol edilmektedir. FileNotFoundException veya FormatException olması halleri ele alınmıştır. true döndürdüğümüz yerlerde, oluşan istisnai durumların geliştirici tarafından ele alındığını ifade edilmektedir. Program kodunun çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

[![blg208_Runtime2](/assets/images/2011/blg208_Runtime2_thumb.gif)](/assets/images/2011/blg208_Runtime2.gif)

Tabi burada dikkat edilmesi gereken ayrı bir durum daha vardır. Olaya 4ncü bir Task örneğini daha kattığımızı düşünelim ve kodumuzu buna göre aşağıdaki gibi düzenleyelim.

```csharp
using System; 
using System.IO; 
using System.Linq; 
using System.Threading.Tasks; 
using System.Data.SqlClient;

namespace HandlingExcpetions 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Task task1 = new Task(() => 
            { 
                File.Open("C:\\OlmayanDosya.txt", FileMode.Open); 
            } 
            ); 
            Task task2 = new Task(() => 
            { 
                string number = "oniki"; 
                double point = Convert.ToDouble(number); 
            } 
            ); 
            Task task3 = new Task(() => 
            { 
                for (int i = 0; i < 100; i++) 
                { 
                    i++; 
                    i--; 
                    i *= 1; 
                    Console.Write("."); 
                } 
            } 
            ); 
            Task task4 = new Task(() => 
                { 
                    SqlConnection conn = new SqlConnection("data source=.;database=Maybe;integrated security=sspi"); 
                    conn.Open(); 
                } 
            );

            task3.Start(); 
            task2.Start(); 
            task1.Start(); 
            task4.Start();

            try 
            { 
                Task.WaitAll(task1, task2, task3,task4); 
            } 
            catch (AggregateException agrExcp) 
            { 
                agrExcp.Handle(e => 
                    { 
                        if (e is FileNotFoundException) 
                        { 
                            Console.WriteLine("Dosya bulunamadı"); 
                            return true; 
                        } 
                        else if (e is FormatException) 
                        { 
                            Console.WriteLine("Dönüştürme hatası"); 
                            return true; 
                        } 
                        else 
                            return false; 
                    } 
                ); 
            } 
        } 
    } 
}
```

Örneği çalıştırdığımızda aşağıdaki gibi bir sonuçla karşılaşırız.

[![blg208_Exception](/assets/images/2011/blg208_Exception_thumb.gif)](/assets/images/2011/blg208_Exception.gif)

Dikkat edileceği üzere uygulama istem dışı bir şekilde sonlanmıştır. Task 4 nesne örneğine ait kod bloğunda, var olmayan bir veritabanına doğru bağlantı oluşturulmaya çalışılmaktadır. Bunun doğal sonucu bir SqlException örneğidir. Ancak catch bloğu içerisinde kullandığımız Handle metodunda söz konusu istisna ele alınmadığı için kod doğrudan else bloğuna girmiştir. Bir başka deyişle ele alınmamış bir istisna söz konusudur. Özellike Handle metodunda bu duruma dikkat edilmesi gerekir.

## IsFaulted Özelliğine Bakmak

Bazı durumlarda istisnaların catch bloğu içerisinde, AggregateException üzerinden ele alınması yerine, Task örneklerine ait IsFaulted özelliklerine bakılaraktan da ilerlenebilir. Task örnekleri üzerinden erişilebilen IsFaulted, IsCompleted, IsCanceled gibi özellikler sayesinde, Task’ in çalışma sonucu ile ilişkili bilgiler yakalanabilmektedir. IsFaulted özelliği de istisna senaryolarında değerlendirilebilir. Aşağıdaki kod parçasında bu durum ele alınmaktadır.

```csharp
using System; 
using System.IO; 
using System.Linq; 
using System.Threading.Tasks; 
using System.Data.SqlClient;

namespace HandlingExcpetions 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Task[] tasks={ 
            new Task(() => 
            { 
                File.Open("C:\\OlmayanDosya.txt", FileMode.Open); 
            } 
            ), 
           new Task(() => 
            { 
                string number = "oniki"; 
                double point = Convert.ToDouble(number); 
            } 
            ), 
            new Task(() => 
            { 
                for (int i = 0; i < 100; i++) 
                { 
                    i++; 
                    i--; 
                    i *= 1; 
                    Console.Write("."); 
                } 
            } 
            ) 
            };

            foreach (Task t in tasks) 
            { 
                t.Start(); 
            }

            try 
            { 
                Task.WaitAll(tasks); 
            } 
            catch (AggregateException agrExcp) 
            { 
            }

           foreach (Task t in tasks) 
            { 
                Console.WriteLine("\nTask Id : {0} IsFaulted : {1} IsCompleted : {2} IsCanceled : {3}",t.Id,t.IsFaulted,t.IsCompleted,t.IsCanceled); 
                if (t.IsFaulted) 
                { 
                    foreach (Exception excp in t.Exception.InnerExceptions) 
                    { 
                        Console.WriteLine("{0}",excp.Message); 
                    } 
                } 
            } 
        } 
    } 
}
```

Aynı senaryoyu bu kez Task tipinden örneklerden oluşan bir Array üzerinde ele almaktayız. Dikkat edileceği üzere Task örneklerinin her birinin IsFaulted özelliği kontrol edilmekte ve eğer true ise InnerExceptions özelliği ile belirtilen koleksiyona gidilerek, oluşan istisnai durumlara ait mesaj bilgilendirmeleri yapılmaktadır. Örnekte catch bloğunda herhangibir işlem yapılmadığına dikkat edilmelidir. Uygulamayı çalıştırdığımızda aşağıdaki sonuçlar ile karşılaştığımızı görürüz.

[![blg208_Runtime3](/assets/images/2011/blg208_Runtime3_thumb.gif)](/assets/images/2011/blg208_Runtime3.gif)

## Escalation Policy

Peki ya trigger görevini üstlenen Wait… metodlarından herhangibirini kullanmadığımızda ne olur?

Her ne kadar CLR ortamı, fırlatılan istisnaları yakalıyor olsa da, trigger metodlarının kullanılmadığı durumlarda bunun ne zaman olacağı kestirilemeyebilir. Dahası Wait metodlarının kullanılmak istenmediği durumlarda olabilir. İşte bu durumda TaskScheduler tipinin UnobservedTaskException olay metodunu kullanarak, söz konusu durumlar için devreye giren standart ilke (Policy) değiştirilebilir. Bir başka deyişle Exception yakalanma işleminde kendi yazdığımız olay metodunun devreye girmesini sağlayabiliriz. Ancak bu konuyu işleyebilmemiz için öncelikle TaskScheduler kavramını öğrenmemiz, kavramımız gerekmektedir. Bu konuyu ilerleyen yazılarımızda irdelemeye çalışıyor olacağız.

## Elimizde Olta da, Misina da, Kurşun da Yok

Son olarak hiç bir Exception kontrolü yapmadığımızda ne olduğuna bir bakalım. Bu amaçla aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System; 
using System.IO; 
using System.Linq; 
using System.Threading.Tasks; 
using System.Data.SqlClient;

namespace HandlingExcpetions 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Task[] tasks ={ 
             new Task(() => 
             { 
                 File.Open("C:\\OlmayanDosya.txt", FileMode.Open); 
             } 
             ), 
            new Task(() => 
             { 
                 string number = "oniki"; 
                 double point = Convert.ToDouble(number); 
             } 
             ), 
             new Task(() => 
             { 
                 for (int i = 0; i < 100; i++) 
                 { 
                     i++; 
                     i--; 
                     i *= 1; 
                     Console.Write("."); 
                 } 
             } 
             ) 
             };

            foreach (Task t in tasks) 
            { 
                t.Start(); 
            } 
            Task.WaitAll(tasks);

            Console.WriteLine("Program sonu"); 
        } 
    } 
}
```

Burada herhangibir try…catch bloğu kullanılmamıştır. Ancak senaryomuza göre Task’ lerden ortama fırlayan iki Exception söz konusudur. Çalışma zamanına baktığımızda ise sadece bir tanesinin CLR (Common Language Runtime) tarafından yakalandığını ve uygulamanın da istem dışı sonlandırıldığını görürüz. Üstelik sadece bir Exception mesajı yakalanmış ve uygulamanın son satırı bile çalıştırılmamıştır. Açıkçası uygulamanın bu şekilde istem dışı sonlanması zaten CLR’ dan beklenen bir davranıştır.

[![blg208_Exception2](/assets/images/2011/blg208_Exception2_thumb.gif)](/assets/images/2011/blg208_Exception2.gif)

[![Exclamation](/assets/images/2011/Exclamation_thumb_2.gif)](/assets/images/2011/Exclamation_2.gif) Yapılan örneklerde gözden kaçırılmaması gereken bir husus daha vardır. Task bloklarında Exception oluşan noktalarda, kod bir sonraki satıra geçmeyecektir. Dolayısıyla, ardışıl olarak oluşan istisna durumları olsa bile, aynı Task örneği için sadece ilk Exception içeriğinin yakalanması söz konusudur.

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda Task Parallel Library için Exception kontrol senaryolarını incelemeye çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HandlingExcpetions.rar (22,91 kb)](/assets/files/2011/HandlingExcpetions.rar) [Örnek Visual Studio 2010 Ultimate Sürümünde Geliştirilmiş ve Test Edilmiştir]