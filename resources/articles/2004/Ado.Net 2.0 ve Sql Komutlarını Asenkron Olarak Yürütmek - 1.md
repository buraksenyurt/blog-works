---
title: "Ado.Net 2.0 ve Sql Komutlarını Asenkron Olarak Yürütmek - 1"
pubDate: 2004-09-23 12:00:00
categories:
  - Ado.net 2.0
tags:
  - Ado.net 2.0
  - Bash
  - C#
  - .NET
  - ADO.NET
  - Concurrency
  - Generics
  - Visual Studio
---

# Ado.Net 2.0 ve Sql Komutlarını Asenkron Olarak Yürütmek - 1
Değerli Okurlarım Merhabalar,

Bir önceki makalemizde MARS etkisini incelemiş ve aynı bağlantı üzerinden birden fazla sayıda sonuç kümesine nasıl erişebileceğimizi görmüştük. Her ne kadar, aynı anda birden fazla sonuç kümesine erişebilsekte, halen daha MARS modeli, sql komutları ile eş zamanlı çalışan kodlar ve asenkron yürütülebilen diğer sql komutları için yeterli değildir. Ado.Net 2.0 ile, sql komutlarını asenkron olarak yürütebileceğimiz bir takım yeni üyeler gelmektedir. Bu üyeler sayesinde, sql komutlarını asenkron olarakçalıştırabilir ve hatta, diğer kod satırlarınında eş zamanlı olarak işleyebilmesini sağlayabiliriz. Bu işleri gerçekleştirebilecek üyeler şu an için sadece SqlClient sınıfında yer almaktadır. Ancak.Net Framework 2.0' ın final sürümünde bu üyelerin, diğer Ado.Net isim alanlarınada yerleştirileceklerini düşünüyorum.

Asenkron komut yürütümenin mantığı, asenkron metodların çalıştırılmasına çok benzer. Nitekim burada da, başlatılan bir prosesi kontrol etmemize imkan sağlayan IAsyncResult arayüzününe ait bir nesne örneği anahtar görevini oynamaktadır. Asenkron sql komutlarını yürütmek için, SqlCommand sınıfına aşağıdaki tabloda yer alan altı adet yeni metod ilave edilmiştir.

SqlCommand sınıfı için Asenkron komut yürütme metodları

BeginExecuteNonQuery

BeginExecuteReader

BeginExecuteXmlReader

EndExecuteNonQuery

EndExecuteReader

EndExecuteXmlReader

Begin ile başlayan tüm metodlar, geriye IAsyncResult arayüzü tipinden bir nesne örneği döndürürler. Programın çalışması esnasında, bu tipe ait özellikler kullanılarak çalışan sql komutlarına ait proseslerin durumları kontrol edilebilir ve tamamlanıp tamamlanmadıkları öğrenilir. Elbetteki, çalışması tamamlanan bir sql komut metodunun geriye sonuçları döndürebilmesi için, End ile başlayan uygun metodun çalıştırılması ve bu metoda ilgili prosese ait IAsyncResult nesne örneğinin parametre olarak gönderilmesi gerekmektedir. Temel olarak bu, Asenkron Sql Komutlarının çalıştırılmasının ana mantığıdır. Bununla birlikte Ado.Net 2.0, asenkron sql komutlarının yürütülebilmesi için 3 değişik model sunmaktadır.

![mk96_1.gif](images/mk96_1.gif)

Şekil 1. Asenkron Yürütme Modelleri.

Biz bugünkü makalemizde, Pooling Modelini incelemeye çalışacağız. Bu modelde genellikle, IAsyncResult tipinden nesne örneği ile sahip olunan prosesin tamamlanıp tamamlanmadığı sürekli olarak kontrol edilir. Yani, Begin ile başlayan herhangibir metoddan sonra bu metodun içerdiği Sql komutu yürütülürken, elde edilen IAsyncResult tipi nesne örneğine ait IsCompleted özelliğine bakılarak prosesin tamamlanıp tamamlanmadığı araştırılır. Bu araştırma işlemi sürekli tekrar ettiğinden çalıştırılan sql komutları tamamlanıncaya kadar, bu aralıklarda başka işlemler gerçekleştirilebilir veya başka sql komutları yürütülebilir. Diğer taraftan bu kontrol işlemi zorunlu değildir. Nitekim biz bu kontrolü yapmasakta, Begin ve End komutları arasındaki blokların çalıştırılması sağlanabilir. Ancak burada ana fikir, uzun sürebilecek sql komutlarının asenkron olarak çalışmaları halinde zaman kaybının önüne geçebilmek, uygulamanın işleyişinin kesilmesini önlemek ve bu zaman aralığında başka kodları ve başka kullanıcı aktivitelerini başarılı bir şekilde icra edebilmektir. Şimdi dilerseniz Pooling modelinin en basit haliyle uygulanışını incelemek amacıyla, Visual Studio.Net 2005 ortamında aşağıdaki kodlara sahip basit bir Console uygulaması geliştirelim.

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

#endregion

namespace PoolingModeli
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;Asynchronous Processing=true");
                SqlCommand cmd = new SqlCommand("UPDATE Production.Product SET ListPrice=ListPrice+100", con);
                con.Open();
                IAsyncResult res = cmd.BeginExecuteNonQuery();

                int i = 0;
                while (!res.IsCompleted)
                {
                    Console.WriteLine("İŞLEM DEVAM EDİYOR "+i);
                    i += 1;
                }

                int sonuc = cmd.EndExecuteNonQuery(res);
                Console.WriteLine(sonuc + " SATIR GÜNCELLENDİ...");
                Console.ReadLine();
                con.Close();
            }
            catch (Exception hata)
            {
                Console.WriteLine(hata.Message);
                Console.ReadLine();
            }
        }
    }
}
```

Şimdi kodlarımızda ne yaptığımıza yakında bakalım. İlk olarak, aşağıdaki satır ile yeni bir SqlConnection nesnesi yaratıyoruz.

```csharp
SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;Asynchronous Processing=true");
```

Burada, standart Sql Connection ifademizdekilerinin aksine, ekstradan bir terim kullandık.Asynchronous Processing terimi, açılan bağlantının asenkron erişimlere izin verip vermeyeceğini belirler. Varsayılan değeri false'tur ve Asenkron komut yürütmelerine izin vermez. Bu nedenle burada true değerini açıkça atıyoruz. Eğer bu bağlantı özelliğini belirtmessek, çalışma zamanında aşağıdaki istisnayı alırız.

![mk96_4.gif](images/mk96_4.gif)

Şekil 2. Bağlantının Asenkron yürütmelere izin vermemesi halinde.

Daha sonra ise, SqlCommand nesnemizi oluşturuyoruz. SqlCommand nesnemiz, AdventureWorks veritabanında yer alan Product isimli tablodaki ListPrice değerini 100 birim arttıran bir Update sorgusuna sahip. Bu tablo Yukon kurulduğunda 504 satır veri içermektedir. Haliyle biraz zaman alabilecek bir güncelleme işlemi söz konusu. Ancak biz istiyoruz ki, bu güncelleme komutu yürütülürken uygulama ortamında başka şeylerde yapabilelim. En azından uygulama bloke olmadan basit bir takım işlemleri gerçekeştirebilme amacındayız. Bu amaçla ilk olarak sql komutumuzu BeginExecuteNonQuery metodu ile çalıştırıyoruz ve hemen ortama bir IAsyncResult tipi nesnenin dönmesini sağlıyoruz.

```csharp
IAsyncResult res = cmd.BeginExecuteNonQuery();
```

Şu aşamada, çalışan komuta ait proses res isimli IAsyncResult tipinden nesne örneğinin sorumluluğu altında. Biz bu örneğimizde pooling modelini kullandığımız için, eş zamanlı olarak çalıştırmak istediğimiz kodlarımızı bir while döngüsü içerisine alabiliriz. Bu while döngüsü her bir iterasyonunda, IAsyncResult nesnesinin o an sahip olduğu prosesin tamamlanıp tamamlanmadığını kontrol ediyor olacaktır. Bunun içinde, IsCompleted özelliğinin değerine bakılıyor. Eğer dönen değer false ise, işlemin tamamlanmadığı anlaşılıyor ve döngü içindeki kodlar yürütülüyor.

```csharp
while (!res.IsCompleted)
{
      Console.WriteLine("İŞLEM DEVAM EDİYOR "+i);
      i += 1;
}
```

Sql komutunun yürütülmesi tamamlandığında, IAsyncResult nesnemizin IsCompleted özelliği true değerini döndürecektir. Dolayısıyla döngüden çıkılmış olunacak ve bir sonraki satıra gelinecektir. Gerçekleştirdiğimiz asenkron işleyişi aşağıdaki şekil ile kafamızda daha kolay canlandırabileceğimizi düşünüyorum.

![mk96_3.gif](images/mk96_3.gif)

Şekil 3. Pooling Modelinde İşleyiş Tarzı.

Tabiki, döngü içerisinde yer alan işlemlerin daha uzun sürede gerçekleşmesi gibi bir durum ilede karşılaşabiliriz. Böyle bir durumda, IAsyncResult nesnesine ait proses tamamlanmış olsa bile, diğer işlemler devam ettiği için bu işlemler sonlanana kadar uygulama donacaktır.

```csharp
int sonuc = cmd.EndExecuteNonQuery(res);
```

Bu satırda ise, EndExecuteNonQuery metoduna yütülen sql komutuna ait prosesi temsil eden IAsyncResult nesnemiz parametre olarak gönderiliyor. Böylece, tamamlanan proses sonucu elde edilen değer (ExecuteNonQuery'nin integer bir değer döndürdüğünü, yani sql sorusundan etkilenen satır sayısını verdiğini belirtelim.) uygulama ortamına aktarılıyor. Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk96_2.gif](images/mk96_2.gif)

Şekil 4. Uygulamanın çalışması sonucu.

Elbetteki, bu örneğimizde olduğu gibi bir while döngüsünü kullanmak zorunda değiliz. Sonuç olarak, IAsyncResult nesnesi, End metodları ile ortama iade edilinceye kadar yer alan tüm satırlar çalışacaktır. Diğer taraftan, aynı anda birden fazla sql komutunun asenkron olarak çalıştırılmasınıda sağlayabiliriz. Dilerseniz pooling modelinin, birden fazla Sql komutunun yürütülebilmesi için nasıl kullanılabileceğini bir örnek ile inceleyelim. Bu amaçla, Console uygulamamızdaki kodlarımızı aşağıdaki hale getirelim.

```csharp
try
{
    /* Bağlantımızı oluştururken Asynchronous Processing özelliğine true değerini vermeyi ihmal etmiyoruz.*/
    SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;MultipleActiveResultSets=true;Asynchronous Processing=true");

    /* SqlCommand nesnelerimizi oluşturuyoruz. İlki bir tablo üzerinde güncelleme yaparken, ikincisi bir View nesnesinde veri çekiyor.*/
    SqlCommand cmd = new SqlCommand("UPDATE Production.Product SET ListPrice=ListPrice+10", con);
    SqlCommand cmd2 = new SqlCommand("SELECT * FROM Sales.vStore", con);
    con.Open(); // Bağlantımızı açıyoruz.
    IAsyncResult res = cmd.BeginExecuteNonQuery();
     IAsyncResult res2 = cmd2.BeginExecuteReader();

    //Bu aralıktaki satırlar bloke olmadan çalışır.
    /* Döngü her iki sql komutuda tamamlanıncaya kadar çalışacaktır. Eğer bunlardan her hangibir önce biterseki öyle olacaktır, diğeride sonlanıncaya kadar döngü içerisindeki kodlar işletilmeye devam eder.*/
    while ((!res2.IsCompleted)||(!res.IsCompleted)) 
    {
        /* Eğer Update komutu tamamlanmış ise, o anki milisaniye ile birlikte ekrana RES BITTI yazdırır. Eğer işlem tamamlanmamışsa, RES ISLEMINE DEVAM EDIYOR tekstini ve o anki milisaniyeyi yazar.*/
        if (res.IsCompleted)
        {
            Console.WriteLine("RES BITTI ");
            Console.WriteLine(DateTime.Now.TimeOfDay.Milliseconds.ToString());
        }
        else
        {
            Console.WriteLine("RES ISLEMINE DEVAM EDIYOR " + DateTime.Now.TimeOfDay.Milliseconds.ToString());
        }
         /* Eğer Select komutu tamamlanmış ise, o anki milisaniye ile birlikte ekrana RES2 BITTI yazdırır. Eğer işlem tamamlanmamışsa, RES2 ISLEMINE DEVAM EDIYOR tekstini ve o anki milisaniyeyi yazar.*/
        if (res2.IsCompleted)
        {
            Console.WriteLine("RES2 BITTI ");
            Console.WriteLine(DateTime.Now.TimeOfDay.Milliseconds.ToString());
        }
        else
        {
            Console.WriteLine("RES2 ISLEMINE DEVAM EDIYOR " + DateTime.Now.TimeOfDay.Milliseconds.ToString());
        }
    }
    /* Update sorgusunun sonuçlarını ilgil IAsyncResult nesnesini parametre olarak vererek integer bir değişkene atıyoruz. Burada EndExecuteNonQuery metodu aynen ExecuteNonQuery metodunda olduğu gibi geriye , sorgudan etkilenen satır sayısını döndürecektir. */
    int sonuc = cmd.EndExecuteNonQuery(res);
    Console.WriteLine(sonuc + " SATIR GÜNCELLENDİ...");

    /* Select sorgusu ile elde edilen kayıt kümesini ortama almak için, EndExecuteReader metoduna res2 isimli IAsyncResult nesne örneğini parametre olarak gönderiyoruz ve bu kümedeki ilk satıra ait 0ncı ve 1nci alanların değerlerini ekrana yazdırıyoruz.*/
    SqlDataReader dr = cmd2.EndExecuteReader(res2);
    dr.Read();
    Console.WriteLine(dr[0] + " " + dr[1]);
    dr.Close(); // SqlDataReader nesnemizi kapatıyoruz.
    Console.ReadLine();
    con.Close(); // Bağlantımızı kapatıyoruz.
}
catch (Exception hata)
{
    Console.WriteLine(hata.Message);
    Console.ReadLine();
}
```

Bu örnekte, iki sql komutunu asenkron olarak çalıştırmaktayız. Yürütülecek her iki komut içinde birer IAsyncResult tipinden nesne örneği oluşturuyoruz. Bu andan itibaren, uygulamada farklı işlemler yapabiliriz. Biz burada, while döngüsünü her iki IAsyncResult nesne örneğinin temsil ettiği proseslerde çalışan sql komutları sonlanıncaya kadar çalıştırmaktayız. Yürütülen komutlar işleyişlerini bitirdiklerinde, kullandıkları IAsyncResult nesne örnekleri yardımıyla asıl sonuçları ortama alıyoruz. Sonuç itibariyle, çalışan sql komutları asenkron olarak yürütülmekte ve çalışmaları esnasında programın bloke olması önlenmektedir. Aşağıdaki ekran görüntüsünden de dikkat edeceğiniz gibi, daha kısa süren UPDATE işlemi tamamlandıktan sonra, diğer komut bitene dek döngü devam etmiştir.

![mk96_5.gif](images/mk96_5.gif)

Şekil 5. Uygulamanın çalışması sonucu.

Bu makalemizde Asenkron Sql Komutların Yürütülmesi konusuna kısaca değinerek kullanabileceğimiz tekniklerden birisi olan Pooling Modelini inceledik. Pooling modeli aslında, çok büyük boyutlu sorgular içeren sql komutlarının asenkron olarak yürütülmesinde çok faydalı bir teknik değildir. Bunun yerine daha gelişmiş olan diğer modellerden CallBack Modeli veya Wait Modeli yararlanılabilir. Bir sonraki makalemizde, CallBack modelini incelemeye çalışacağız. Tekrar görüşünceye dek hepinize mutlu günler dilerim.
