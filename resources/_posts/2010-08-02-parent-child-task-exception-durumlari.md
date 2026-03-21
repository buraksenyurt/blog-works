---
layout: post
title: "Parent-Child Task Exception Durumları"
date: 2010-08-02 21:10:00 +0300
categories:
  - csharp-4-0
  - parallel-programming
  - tpl
tags:
  - task-parallel-library
  - parallel-programming
  - parallel-computing
---
Daha orta okul sıralarındayken havacılığa karşı müthiş bir ilgim vardı. Hiç unutmuyorum o yıllarda Uçan Türk dergisinin sıkı bir fanatiğiydim. Pek çok savaş uçağının teknik özelliklerini ezbere bilirdim ve hatta onları arşivlediğim bir not defterim dahi vardı. Uçmaktan korkan birisi olmama rağmen bunu yeneceğimi düşünerekten Lise yıllarında Hava Harp Okuluna girebilmek için özel bir çalışma programı bile uygulamıştım. Düzenli olarak spor yapıyor, kondisyon arttırmaya çalışıyor, günde değil 3, 5 kere dişlerimi fırçalıyor, gözlerimi yormamak için uykuma özen gösteriyordum.

![blg174_Giris.jpg](/assets/images/2010/blg174_Giris.jpg)

Tabi öğrenciliğim çok parlak olmadığı için ÖSS sınavında aşmam gereken 150' lik puan barajı konusunda tereddütler yaşıyordum. Nitekim barajı da geçemedim. Hayallerim yıkılmış mıydı? Elbette hayır. Heleki o yıllardaki çalışma azmimim bana kazandırdığı önemli avantajlar olduğu düşünüldüğünde. Bunlardan birisi de derin detaylara inebilmek için gerekli eforu, gayreti gösterme isteğidir. Neden böyle bir giriş yaptığıma gelince...Bu seferki konumuz Paralel Programlamada, ilişkisel Task örneklerinin Exception yönetimi hakkındadır. Konu sıkıcı ve bir o kadarda detaylıdır. Ama neyseki araştırıp, sıkılmadan derinlerine inmek ve analiz etmek için gerekli gayret mevcut

![Wink](/assets/images/2010/smiley-wink.gif)

Hatırlayacağınız üzere [Parent-Child Tasks Kavramı](https://www.buraksenyurt.com/admin/app/editor/post/Parent-Child-Tasks-Kavrami) başlıklı yazımızda.Net Framework 4.0 tarafında paralel programlamada önemli bir yere sahip olan Task örnekleri arasındaki Parent, Child ilişkiyi incelemeye çalışmıştık. Parent-Task nesne örnekleri arasındaki ilişkilerde bilinmesi gereken konulardan birisi de, istisnaların nasıl ele alındığıdır (Exception Handling). Aslında konuya hızlı bir giriş yaparak ilerlememiz şu aşamada avantajımız olacaktır. Bu nedenle Visual Studio 2010 Ultimate ortamında geliştireceğimiz Console uygulamasında aşağıdaki kod içeriğinin yer aldığını göz önüne alalım.

```csharp
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ParentChildTasksExceptionHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Case 1

            Task parent = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent task...");

                Task child1 = Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Child Task 1 başladı...");
                        Thread.Sleep(5000);
                        Console.WriteLine("Child Task 1 bitti...");
                    }
                , TaskCreationOptions.AttachedToParent);
                Task child2 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Child Task 2 başladı...");
                    FileStream stream=File.OpenRead("OlmayanDosya.txt");
                    Console.WriteLine("Child Task 2 bitti...");
                }
                , TaskCreationOptions.AttachedToParent);

                Task child3 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Child Task 3 başladı...");
                    Thread.Sleep(3000);
                    Console.WriteLine("Child Task 3 bitti...");
                }
                , TaskCreationOptions.AttachedToParent);

                Task child4 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Child Task 4 başladı...");
                    Thread.Sleep(10000);
                    SqlConnection conn = new SqlConnection();
                    conn.Open();
                    Console.WriteLine("Child Task 4 bitti...");
                }
                , TaskCreationOptions.AttachedToParent);
            }
            );

            try
            {
                Console.WriteLine("İşlemler yürütülüyor");
                parent.Wait();
                Console.WriteLine("İşlemler tamamlandı");
            }
            catch(AggregateException excp)
            {
                Console.WriteLine("Parent task durumu {0}",parent.Status);
                Console.WriteLine(excp.Message);
                foreach (var innerExcp in excp.InnerExceptions)
                {
                    Console.WriteLine(innerExcp.InnerException.Message);
                }
            }

            #endregion
        }
    }
}
```

Buradaki kod parçasında bir Parent Task ve bunun içerisinde çalışacak şekilde planlanan 4 farklı Child Task örneği yer almaktadır. Bu örneklerden birisi, olmayan bir dosyayı açmaya çalışmaktadır. Dolayısıyla FileNotFoundException tipinden bir istisna nesnesi üreteceği garantidir. Diğer bir Task ise parametresiz bir SqlConnection nesnesi oluşturmakta ve belli olmayan bir yere doğru SQL bağlantısı açmaya çalışmaktadır ki bu da InvalidOperationException türünden bir istisna nesnesinin fırlatılmasına neden olacaktır.

Daha önceki yazımızdan hatırlayacağınız üzere Child Task örneklerinin başlattığı metod gövdeleri içerisinde oluşabilecek olan istisnalar (Exception), Parent Task örneğinin Final State durumunu da doğrudan etkilemektedir. Ayrıca Child Task'lerde bir istisna oluşsa bile, diğer Task örnekleri çalışmalarını devam ettirmektedir. Örnek kod parçamızda Parent Task tarafına çıkartılan Exception örneklerinin yakalanabilmesi amacıyla try...catch bloğuna başvurulduğu görülmektedir.

Burada dikkat edilmesi gereken iki nokta vardır. Bunlardan birisi try bloğu içerisinde Parent Task nesne örneği üzerinden bilinçli olarak Wait metodunu kullanılmış olmasıdır. Bu şekilde uygulamanın ana Thread bloğu sonlanmadan Parent Task ve içeriğinin işlerinin tamamlanmasının beklenmesi garanti edilmiş olmaktadır. Diğer yandan ikinci önemli nokta catch bloğu içerisinde, Parent Task örneği altında çalışan Child Task'ler tarafından fırlatılan Exception nesnelerinin nasıl yakalandığıdır. Burada AggregateException sınıfının InnerExceptions isimli koleksiyonu üzerinden hareket edildiğine dikkat edilmelidir. Uygulama kodu Debug modda çalıştırıldığında aşağıdaki sonuçların elde edildiği görülecektir.

![blg174_DebugException.gif](/assets/images/2010/blg174_DebugException.gif)

Görüldüğü gibi AggregateException nesnesi Child Task'lerde oluşan istisnaları bir koleksiyon dahilinde saklamaktadır. Count değerinin iki dönmesinin sebebi tahmin edeceğiniz üzere iki Child Task'in Exception fırlatmış olmasıdır. Uygulamamızın çalışma zamanı görüntüsü ise aşağıdaki gibi olacaktır.

![blg174_RuntimeCase1.gif](/assets/images/2010/blg174_RuntimeCase1.gif)

Dikkat edileceği üzere Parent Task örneğinin Final State durumundaki karşılığı Faulted olmuştur ki bu son derece doğaldır. Çünkü Child Task örneklerinden ikisi Exception üretimi bildirmiştir. Bu istisna bildirimlerinin Child Task'lerden, Parent Task'e bildirildiğini ve AggregateException içerisinde toplandıklarını da unutmamak gerekir.

Exception yönetimi ile ilişkili tek durum bu değildir. Şimdi de aşağıdaki kod örneğini göz önüne alalım.

```csharp
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ParentChildTasksExceptionHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Case 2

            Task parentTask = null;
            Task childTask = null;
            ManualResetEvent mre = new ManualResetEvent(false);

            parentTask = Task.Factory.StartNew(() =>
                {
                    childTask = Task.Factory.StartNew(() =>
                        {
                            throw new Exception("Child Task için Exception");
                        }, TaskCreationOptions.AttachedToParent);
                    mre.Set();
                    throw new Exception("Parent Task için Exception");
                }
             );

            mre.WaitOne();

            try
            {
                //Task.WaitAll(parentTask, childTask);
                parentTask.Wait();
            }
            catch (AggregateException excp)
            {
                Console.WriteLine("{0} adet Exception söz konusudur",excp.InnerExceptions.Count);
            }

            #endregion
        }
    }
}
```

Bu kod parçasında iki adet Task örneği bulunmaktadır ve aralarında Parent-Child ilişki söz konusudur. Dikkat edilmesi gereken nokta ise, kodun bu haliyle debug edilmesi veya çalıştırılması sonrasında AggregateException nesne örneği üzerinden yaklanan dahili istisna örnekleri ve toplam sayısıdır. parentTask.Wait (); metod çağrısı kullanıldığında debug modda iken aşağıdaki sonuçlar ulaşılabildiğim gözlemlenecektir.

![blg174_RuntimeCase2_2.gif](/assets/images/2010/blg174_RuntimeCase2_2.gif)

Aslında beklediğimiz gibi bir sonuç söz konusudur. parentTask nesne örneği üzerinden Wait metodu kullanıldığı için AggregateException nesne örneğinin InnerExceptions koleksiyonunun ilk elemanı Parent Task örneğinin çalıştırdığı metod içerisinden fırlatılan istisna bilgisini içermektedir. InnerExceptions özelliğinin 1 numaralı indis değeri içinse, Child Task içerisinden üretilen Exception söz konusudur. Ancak hem Parent Task hemde içerdiği Child Task nesne örnekleri için Wait işlemi gerçekleştirilirse?

![Wink](/assets/images/2010/smiley-wink.gif)

Yani yorum satırı açılıp Task.WaitAll (parentTask,childTask) kullanılırsa, bu durumda Debug modda aşağıdaki sonuçlar ile karşılaşırız.

![blg174_RuntimeCase2New.gif](/assets/images/2010/blg174_RuntimeCase2New.gif)

Dikkat edileceği üzere AggregateException, 3 adet InnerException içerdiğini bildirmektedir. Her ne kadar throw new satırlarının sayısı iki olsa da 3 sonuç döndürülmüştür. Yakından bakıldığında ise durum aslında şu şekilde özetlenebilir; AggregateException, WaitAll çağrısı nedeniyle ne kadar Task örneği varsa bunların tamamının ürettiği Exception bilgilerini (parentTask'in ürettiği ve yukarıya gönderdiği Exception dahil) InnerExceptions altında toplamıştır.

Bunlara ilaveten Parent Task örneği içerisindeki Child Task örneğinden fırlatılan Exception, ayrıca InnerExceptions içerisindeki 2 numaralı indise atanmıştır. Dolayısıyla birden fazla Task örneğinin WaitAll ile beklenmesi halinde AggregateException nesnesinin istisna toplama yaklaşımı, sadece Parent Task örneğine uygulanan Wait metodu söz konusu olduğundakinden farklıdır. Bu çok tabi olarak üst tarafta Exception örneklerinin yakalanıp değerlendirildiği konumlarda önem kazanan küçük bir farktır.

Şu ana kadar yaptıklarımızı değerlendirdiğimizde Child Task tamamlandığında eğer bir Exception içeriyorsa bunu Parent Task'e bildirdiği yönündedir.

Exception nesnelerinin ele alınması ile ilişkili bir diğer yaklaşımı ise aşağıdaki kod parçasından devam ederek değerlendirebiliriz.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParentChildTasksExceptionHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Case 3

            Task parentTask = null;
            Task childTask = null;

            parentTask = Task.Factory.StartNew(() =>
            {
                childTask = Task.Factory.StartNew(() =>
                {
                    throw new Exception("Child Task için Exception");
                }, TaskCreationOptions.AttachedToParent);

                try
                {
                    childTask.Wait();
                }
                catch
                {
                }
            }
             );

            try
            {
                parentTask.Wait();
                Console.WriteLine("Parent Task Status : {0}",parentTask.Status);
            }
            catch (AggregateException excp)
            {
                Console.WriteLine("{0} adet Exception söz konusudur", excp.InnerExceptions.Count);
            }

            #endregion
        }
    }
}
```

Bu sefer Parent Task için açılan kod içerisinde Child Task örneği için Wait metodu çağrısı yapılmış ve söz konusu çağrı sırasında bir Exception oluşrsa, boş bir catch bloğunda yakalanmıştır. Dikkat çekici nokta da zaten burasıdır. Boş catch bloğu içerisinde herhangibir şekilde Exception nesne örneği fırlatılmadığından, Child Task tarafından üretilen istisna, Parent Task örneğine bildirilmemektedir. Bir başka deyişle Child Task örneğinin ürettiği istisna kendi içerisinde ele alınarak süpürülmüştür. Dolayısıyla bu örnek kod parçasının çalışma zamanı çıktısı aşağıdaki gibi olacak, bir başka deyişle, Parent Task nesne örneğinin Final State durumu RanToCompletion olarak belirlenecektir.

![blg174_RuntimeCase3.gif](/assets/images/2010/blg174_RuntimeCase3.gif)

Şimdi burada durup önemli bir noktayı vurgulamak gerektiği düşüncesindeyim. WaitAll metodunu kullandığımız örnekte Child Task ve Parent Task örneklerinin durumları Faulted olarak set edilmektedir. Ancak son senaryoda Parent Task örneğine Child Task içerisinde fırlatılan Exception bildirilmediğinden sadece Child Task, Faulted durumuna düşecek ve Parent Task, RanToCompletion modunda olacaktır. Bunun belirgin olan sebebi az öncede belirttiğimiz üzere, Child Task içerisinden fırlatılan istisnanın Parent Task'e çıkartılmadan bir try...catch bloğu ile kontrol altına alınmasıdır. Ancak yine son senaryoda aşağıdaki gibi bir kullanım ile Parent Task'e Child Task'ten fırlatılan Exception durumunun bildirilmesi sağlanabilir.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParentChildTasksExceptionHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Case 4

            Task parentTask = null;
            Task childTask = null;

            parentTask = Task.Factory.StartNew(() =>
            {
                childTask = Task.Factory.StartNew(() =>
                {
                    throw new Exception("Child Task için Exception");
                }, TaskCreationOptions.AttachedToParent);

                childTask.ContinueWith(_ =>
                {
                    try
                    {
                        childTask.Wait();
                    }
                    catch(AggregateException excp)
                    {
                    }
                }
                );
            }
            );

            try
            {
                parentTask.Wait();
                Console.WriteLine("Parent Task Status : {0}", parentTask.Status);
            }
            catch (AggregateException excp)
            {
                Console.WriteLine("{0} adet Exception söz konusudur\nParent Task Durumu : {1}", excp.InnerExceptions.Count,parentTask.Status);
            }

            #endregion
        }
    }
}
```

Bu kod parçasında Child Task nesne örneği üzerinden Continue metodu kullanılmış ve try...catch bloğu ile alt task örneğinin ürettiği istisnanın yine aynı blok içerisinde ele alınması sağlanmıştır. Bu durumda Parent Task yine Child Task içerisinden üretilen Exception nesnesi için bilgilendirilecektir, üstelik Continue bloğu içerisindeki catch bloğundan dışarıya doğru bir Exception fırlatımı açık bir şekilde yapılmasa bile. Tabi bu durumda Parent Task nesne örneğinin Final State durumu yine Faulted olacaktır. İşte çalışma zamanı çıktısı.

![blg174_RuntimeCase4.gif](/assets/images/2010/blg174_RuntimeCase4.gif)

Görüldüğü gibi Parent-Child Task diyerek geçmemek lazım

![Wink](/assets/images/2010/smiley-wink.gif)

Ele alınması gereken bir kaç durum söz konusu ki bunlardan birisi de iptal işlemleri (Cancellation). Bu konuyu da bir sonraki yazımızda ele almaya çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ParentChildTasksExceptionHandling.rar (25,96 kb)](/assets/files/2010/ParentChildTasksExceptionHandling.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde geliştirilmiş ve test edilmiştir]
