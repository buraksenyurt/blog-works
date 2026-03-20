---
layout: post
title: "Ado.Net 2.0 ve Sql Komutlarını Asenkron Olarak Yürütmek - 3"
date: 2004-10-22 12:00:00 +0300
categories:
  - ado-net-2-0
tags:
  - ado-net-2-0
  - bash
  - csharp
  - dotnet
  - ado-net
  - async-await
  - threading
  - concurrency
  - generics
---
Hatılayacağınız gibi, Asenkron erişim teknikleri ile ilgili önceki makalelerimizde Polling ve Callback modellerini incelemiştik. Bu makalemizde ise, Wait modelini incelemeye çalışacağız. Wait modeli, diğer asenkron sql komutu yürütme tekniklerine göre biraz daha farklı bir işleyişe sahiptir. Bu model, bazı durumlarda asenkron olarak çalışan sql komutları tamamlanıncaya kadar uygulamayı bekletmek istediğimiz durumlarda kullanılmaktadır. WaitHandle modeli aslında birden fazla sunucu üzerinde çalışacak farklı sorgular söz konusu olduğunda işe yarayacak etkili bir tekniktir. Diğer taraftan eş zamanlı çalışan sorgu sonuçlarının uygulamamın kalanında etkili olduğu durumlarda da tercih edilmelidir. Wait modeli şu an için 3 teknik ile gerçekleştirilmektedir. Dilerseniz bu tekniklerin ne olduklarını ve nasıl uygulandıklarını kısaca inceleyelim.

![mk104_1.gif](/assets/images/2004/mk104_1.gif)

Şekil 1. Wait Modelleri.

Wait Modellerini en kolay haliyle anlayabilmek için işe, WaitOne modeli ile başlamakta fayda vardır. WaitOne modeli, sadece tek bir sql komutu için uygulamanın bekletilmesini sağlar. Ne kastettiğimi anlamak için aşağıdaki örneğimizi dikkatle inceleyelim.

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Threading;

#endregion

namespace WaitModeli
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;async=true");
            SqlCommand cmd = new SqlCommand("WAITFOR DELAY '0:0:5';UPDATE Production.Product SET ListPrice=ListPrice*1.15", con);

            con.Open();
            IAsyncResult res1 = cmd.BeginExecuteNonQuery(); //Komutu asenkron olarak çalıştır.

            WaitHandle wh1 = res1.AsyncWaitHandle; // WaitHandle nesnesini al.
            Console.WriteLine("Herhangibir islem"); // Bu aralıkta eş zamanlı kodlar çalışır.

            wh1.WaitOne(); // Eğer komut tamamlanmamışsa bekle.
            int sonucUpdate = cmd.EndExecuteNonQuery(res1); // Komut işleyişini tamamladı sonuçları al.
            con.Close();
            Console.WriteLine(sonucUpdate.ToString() + " SATIR GUNCELLENDI");

            Console.ReadLine();
        }
    }
}
```

Bu örnek console uygulamasında, Yukon üzerinde yer alan AdventureWorks veritabanındaki Product isimli tabloda güncelleme işlemi gerçekleştiren bir komut söz konusudur. Senaryonun daha gerçekçi olması için sql üzerinde WaitFor ile işlemi 5 saniye geç başlatıyoruz. Biz komutu asenkron olarak yürütmek istiyoruz. Lakin komutun sonuçlarını almadan önce bir takım kod satırlarınında çalıştırılmasını istiyoruz. Buraya kadar her şey normal. Ancak eş zamanlı yürüyen kod satırlarından sonrada, eğer işlenen komut halen daha tamamlanmamışsa, o komut tamamlanıncaya kadar güncel uygulamanın duraksamasını ve başka bir işlem yapmamasını istiyoruz. İşte bu gibi tek komutlara wait modelini uygulamak istediğimizde, WaitHandle sınıfının statik metodlarından olan WaitOne'ı kullanmaktayız.

Uygulama kodlarımızı inceldiğimizde ilk olarak asenkron olarak çalışmasını istediğimiz komuta ait Begin metodlarından birisini kullanarak IAsyncResult arayüzü tipinden bir nesne örneğini elde ediyoruz.

```csharp
IAsyncResult res1 = cmd.BeginExecuteNonQuery();
```

Daha sonra ise, bu arayüz nesnesinin AsyncWaitHandle özelliği ile bir WaitHandle nesnesi oluşturuyoruz.

```csharp
WaitHandle wh1 = res1.AsyncWaitHandle;
```

Bu WaitHandle nesnesinin elde edilişi sırasında ve sonrasındaki uygulama satırları sql komutumuz ile eşzamanlı olarak yürütülmektedir. Biz WaitHandle nesnemizin WaitOne metodunu uyguladığımızda, çalışan sql komutunun tamamlandığına dair bir sinyal WaitHandle nesnesine gelinceye kadar aktif thread'in duraksatılmasını sağlamış oluyoruz.

```csharp
wh1.WaitOne();
```

Gelelim, WaitAll tekniğine. WaitAll ise, birden fazla sql komutunun asenkron olarak çalıştığı durumlarda devreye giren ve tüm komutlar başarılı bir şekilde tamamlanıncaya kadar aktif uygulamayı duraksatan bir tekniktir. Bu sefer, WaitHandle nesneleri her bir komut için ayrı ayrı tanımlanmalıdır. Dolayısıyla bize WaitHandle tipinden bir dizi gerekmektedir. Aşağıdaki örnek uygulama, WaitAll tekniğinin nasıl uygulandığını göstermektedir.

```csharp
SqlConnection con1 = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;async=true");
SqlConnection con2 = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;async=true");
SqlConnection con3 = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;async=true");
SqlCommand cmd1 = new SqlCommand("WAITFOR DELAY '0:0:4';UPDATE Production.Product SET ListPrice=ListPrice*1.15", con1);
SqlCommand cmd2 = new SqlCommand("WAITFOR DELAY '0:0:5';UPDATE Production.Product SET ListPrice=ListPrice*1.20", con2);
SqlCommand cmd3 = new SqlCommand("WAITFOR DELAY '0:0:2';UPDATE Production.Product SET ListPrice=ListPrice*1.45", con3);

con1.Open();
IAsyncResult res1 = cmd1.BeginExecuteNonQuery();
con2.Open();
IAsyncResult res2 = cmd2.BeginExecuteNonQuery();
con3.Open();
IAsyncResult res3 = cmd3.BeginExecuteNonQuery();

WaitHandle[] wh = new WaitHandle[3];
wh[0] = res1.AsyncWaitHandle;
wh[1] = res2.AsyncWaitHandle;
wh[2] = res3.AsyncWaitHandle;

Console.WriteLine("Burada bir şeyler yapılır...");
for (int i = 1; i < 100; i++)
{
    Console.Write(i.ToString()+" ");
}
Console.WriteLine();

WaitHandle.WaitAll(wh);
int sonucGuncel1 = cmd1.EndExecuteNonQuery(res1);
con1.Close();
int sonucGuncel2 = cmd2.EndExecuteNonQuery(res2);
con2.Close();
int sonucGuncel3 = cmd3.EndExecuteNonQuery(res3);
con3.Close();
Console.WriteLine(sonucGuncel1 + " SATIR GUNCELLENDI");
Console.WriteLine(sonucGuncel2 + " SATIR GUNCELLENDI");
Console.WriteLine(sonucGuncel3 + " SATIR GUNCELLENDI");
Console.ReadLine();
```

Burada görüldüğü gibi, üç adet sql komutumuz var. Her ne kadar anlamsız olsalarda sonuçta amacımız Wait modelinin nasıl işlediğini anlamaktır. WaitHandle dizisi içindeki her eleman, bu elemanlar ile ilişkili komutlara ait IAsyncResult nesnelerinin AsyncWaitHandle özellikleri yardımıyla oluşturulur. Bu esnada, sql komutları asenkron olarak yürütülmektedir. Dolayısıyla WaitHandle sınıfının WaitAll metodunu çalıştırdığımız satıra kadar olan kodlar, komutlar ile birlikte eş zamanlı olarak yürümektedir. WaitAll metodunun olduğu satıra gelindiğinde, uygulama çalışan komutların hepsi tamamlanmış olmak şartıyla yoluna devam eder. Bir başka deyişle, eğer bu satıra gelindiğinde halen daha tamamlanmamış komutlar varsa, varsayılan timeout süresi doluncaya kadar bu komutlarında tamamlanması beklenir.

Wait modelindeki son teknik ise, WaitAny yapısıdır. Bu teknik bir öncekilere nazaran biraz daha karmaşıktır. WaitAny tekniğinde, asenkron olarak yürütülen komutlar sırasıyla tamamlanıncaya kadar uygulama bekletilir. Burada sıralamayı belirten WaitAny metodudur. Nitekim, işleyişi önce tamamlanan metodun WaitHandle nesnesi diğerlerine göre daha öncelikli olarak sinyal alacak ve sonuçlar ortama aktarılacaktır. WaitAny tekniğinde işin ilginç ve bir o kadarda önemli olan yanı, WaitAny metodunun geriye integer olarak döndürdüğü değerin kullanılmasında yatmaktadır. Bu değer aslında, WaitHandle dizisi içindeki indeksi işaret etmektedir. Yani, WaitAny metodu ile aktif olarak alınan WaitHandle eğer tamamlanmışsa, metodun geriye döndüreceği değer bu komuta bağlı WaitHandle elemanının dizi içerisindeki indeksidir.

Buna göre, WaitAny tekniğinin, WaitHandle dizisi içindeki tüm elemanları işleyen bir döngü içerisinde kullanılması ve bu metodun döndürdüğü değerlerinde dizi indeksleri ile burada karşılaştırılması gerekmektedir. Her ne kadar karışık bir teknikmiş gibi görünsede, aşağıdaki kod bloğunu incelediğimizde olayı çok daha kolay anlayabileceğimize inanıyorum.

```csharp
SqlConnection con1 = new SqlConnection("data source=Manchester;initial catalog=AdventureWorks;integrated security=SSPI;async=true");
SqlConnection con2 = new SqlConnection("data source=Manchester;initial catalog=AdventureWorks;integrated security=SSPI;async=true");
SqlCommand cmd = new SqlCommand("WAITFOR DELAY '0:0:5';UPDATE Production.Product SET ListPrice=ListPrice*1.15", con1);
SqlCommand cmd2 = new SqlCommand("WAITFOR DELAY '0:0:3';UPDATE Production.Product SET ListPrice=ListPrice*1.20", con2);

con1.Open();
IAsyncResult res1 = cmd.BeginExecuteNonQuery();
con2.Open();
IAsyncResult res2 = cmd2.BeginExecuteNonQuery();

WaitHandle[] wh = new WaitHandle[2];
wh[0] = res1.AsyncWaitHandle;
wh[1] = res2.AsyncWaitHandle;
for (int i = 0; i < 2; i++)
{
    int iIndis = WaitHandle.WaitAny(wh);
    if (iIndis == 0)
    {
        int result=cmd.EndExecuteNonQuery(res1);
        con1.Close();
        Console.WriteLine(iIndis.ToString()+" tamamlandı." +result.ToString());
    }
    if (iIndis == 1)
    {
        int result = cmd2.EndExecuteNonQuery(res2);
        con2.Close();
        Console.WriteLine(iIndis.ToString() + " tamamlandı." + result.ToString());
    }
}
```

Dilerseniz bu kod parçasını inceleyelim. WaitHandle dizimizi, iki sql komut için gerekli olan WaitHandle nesnelerini tutmak üzere kullanılıyoruz. Bu dizi için gerekli WaitHandle nesnelerini elde edebilmek amacıylada, IAsyncResult arayüzüne ait AsyncWaitHandle özelliğini kullanmaktayız. Buraya kadar herşey anlaşılır. Asıl önemli olan açtığımız for döngüsüdür. Bu döngü, WaitHandle dizisinin eleman sayısı kadar iterasyon yapmaktadır. Döngü içerisinde,

```csharp
int iIndis = WaitHandle.WaitAny(wh);
```

satırı ile, dizi içindeki WaitHandle nesnelerinden her hangibirinin ilgili komutun bittiğine dair bir sinyal alıp almadığına bakılır. Eğer böyle ise, komut işleyişini tamamlamış demektir. Bu durumda WaitAny metodu geriye integer bir değer döndürür. Elbetteki işleyen sql komutlarından her hangibir tamamlanmamış ise, uygulama bu satırda bu komutlardan herhangibiri tamamlanıncaya ve ilgili WaitHandle nesnesine sinyal gönderilinceye kadar duraksar. Dönen integer değer, WaitAny metodunun yapısı gereği, WaitHandle dizisi içindeki ilgili WaitHandle nesnesinin indeks değerini işaret eder. Bu nedenle uygulama bir sonraki satıra geçtiğinde if koşuluna girer ve burada hangi komutun tamamlandığı WaitAny metodundan dönen integer değer yardımıyla belirlenir. Tabiki tamamlanan komuta uygun olan Begin metoduda bu if döngüsü içinde çalıştırılır.

Daha sonra ise, döngü ikinci iterasyondan işleyişine devam eder. Bu kez halen daha çalışmakta olan diğer sql komutlarına ait WaitHandle nesnelerinin bir sinyal alıp almadığına bakılır. Yine, ilk iterasyonda olduğu gibi önce tamamlanan komuta ait WaitHandle nesnesi gerekli sinyali alır ve geriye dizi içindeki indeks değerini döndürür. Ardından tekrar if koşulları uygulanır ve gerekli sonuçlar elde edilir. Bu işleyiş döngü içindeki tüm WaitHandle nesneleri ilişkili oldukları komutların tamamlandığına dair sinyaller alıncaya, dolayısıyla komutlar işleyişini bitirinceye kadar devam eder.

Görüldüğü gibi Wait modeli içerdiği teknikler itibariyle biraz karışıktır. Ancak ilerleyen zamanlarda, Ado.Net 2.0' ın son sürümünde bu tekniklerin çok daha kullanışlı hale geleceğine inanıyorum. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
