---
layout: post
title: "Transaction' larda DeadLock Kavramı"
date: 2004-07-07 12:00:00 +0300
description: "Bu makalemizde, eş zamanlı olarak çalışan Transaction' larda meydana gelebilecek DeadLock durumunu incelemeye çalışacağız. Öncelikle DeadLock teriminin ne olduğunu anlamaya çalışalım. DeadLock, aynı zamanlı çalışan Transaction' ların, belirlir satır(ları) kilitlemeleri sonucunda ortaya çıkabilecek bir durumdur."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - t-sql
  - threading
  - concurrency
  - transactions
---
# Transaction' larda DeadLock Kavramı
Değerli Okurlarım, Merhabalar.

Bu makalemizde, eş zamanlı olarak çalışan Transaction'larda meydana gelebilecek DeadLock durumunu incelemeye çalışacağız. Öncelikle DeadLock teriminin ne olduğunu anlamaya çalışalım. DeadLock, aynı zamanlı çalışan Transaction'ların, belirlir satır (ları) kilitlemeleri sonucunda ortaya çıkabilecek bir durumdur. DeadLock terimini kavrayabilmenin en iyi yolu aşağıdaki gibi gelişebilecek bir senaryoyu zihnimizde canlandırmakla mümkündür. Bu senaryoda söz konusu olan iki tablomuz mevcuttur. Bu tablolar Sql sunucusunda Northwind veritabanı altında oluşturulmuş olup Field (alan) yapıları aşağıdaki gibidir.

![mk77_1.gif](/assets/images/2004/mk77_1.gif)

Şekil 1. Musteriler Tablosu.

![mk77_2.gif](/assets/images/2004/mk77_2.gif)

Şekil 2. Personel Tablosu.

Şimdi senaryomuzu tasarlayalarak DeadLock kavramını anlamaya çalışalım. Her iki tabloyu ayrı ayrı kullanan ve eş zamanlı olarak çalışan iki Transaction'ımız olduğunu düşünelim. Burada önemli olan bu iki Transaction'ın aynı anda çalışıyor olmalarıdır.

DeadLock Senaryosu

1nci Adım
Transaction 1 başlatılır.

2nci Adım
Transaction 2 başlatılır.

3üncü Adım
Transaction 1 Personel tablosunda PersonelID değeri 78 olan satırı kitler ve günceller.

4üncü Adım
Transaction 2 Musterilre tablosunda MusteriID değeri 1000 olan satırı kitler ve günceller.

5inci Adım
Transaction 1 Musteriler tablosundaki MusteriID değeri 1000 olan satırı güncellemek ister. Ancak, Transaction 2 bu satırı kitlediğinden, varsayılan Lock TimeOut süresi kadar bu kilidin açılmasını bekler. Bu süre sonuna kadar Transaction 2' nin işlemlerini onaylaması veya geri alması beklenir.

6ncı Adım
Transaction 2 Personel tablosundaki PersonelID değeri 78 olan satırı güncellemek ister. Ancak bu durumda, Transaction 1 bu satırı kitlediğinden yine varsayılan Lock TimeOut süresi kadar bu kilidin açılmasını bekler. Bu süre sonuna kadar Transaction 1' in işlemlerini onaylaması veya geri alması beklenir.

Bizim için önemli olan adımlar, 5inci ve 6ncı adımlardır. Nitekim bu adımlar eş zamanlı olarak gerçekleştirildiklerinden, iki Transaction da birbirinin kilitlerinin açılmasını beklemek durumundadır. İşte bu noktada DeadLock oluşur. Nitekim süreler sona erinceye kadar herhangibir Transaciton sahip olduğu işlemleri ne onaylamış (Commit) nede geri almıştır (RollBack).Dolayısıyla Transaction'lardan birisi, varsayılan olarakta Sql sunucusuna göre en maliyetli olanı otomatik olarak RollBack edilecektir. Bu durumda bu Transaction'a ait kilitler ortadan kalktığından kalan Transaction'a ait güncelleme işlemi tamamlanır. Ancak.net ile yazılan uygulamalarda DeadLock oluştuğunda, Transaction'lardan birisi geri alınmakla kalmaz aynı zamanda ortama bir istisna fırlatılır. Dolayısıyla DeadLock durumunda bu istisnanında ele alınması gerekirki, DeadLock sonucu çalışmaya devam eden Transaction işlemleri onaylanabilsin.

DeadLock oluşması durumunda, birbirlerini bekleyen Transaction'larda, bekleme sürelerini ayarlayabilir ve hangi Transaction'ın daha önce RollBack edilmesi gerektiğine karar verebiliriz. Bunun için, T-Sql'in LOCK_TIMEOUT ve DEADLOCK_PRIORITY anahtar sözcükleri kullanılır. Bir Transaction'ın başka bir Transaction'da oluşan kilidi ne kadar süre ile beklemesi gerektiğini belirtmek için aşağıdaki gibi bir sql cümleciği kullanılır.

```text
SET LOCK_TIMEOUT 3000
```

Burada LOCK_TIMEOUT değeri 3 saniye (3000 milisaniye) olarak belirtilmiştir. Diğer yandan, bir Transaction için DeadLock önceliğini aşağıdaki gibi bir sql cümleciği ile belirtebiliriz.

```text
SET DEADLOCK_PRIORITY LOW
```

Bu sql cümleciğini kullanan komutun çalıştığı Transaction, DeadLock oluşması durumunda, ilk olarak RollBack edilecek Transaction olacaktır. DEADLOCK_PRIORITY anahtar sözcüğünün alabileceği diğer değerde NORMAL dir. Bu durumda, Transaction'lardan en maliyetli olanı RollBack edilir. DEADLOCK_PRIORITY için varsayılan değer NORMAL olarak belirlenmiştir. Şimdi dilerseniz DeadLock oluşmasını daha iyi izleyebileceğimiz bir örnek geliştirmeye çalışalım. Bu durumu simule edebilmek için, aynı anda çalışan Transaction iş parçalarına ihtiyacımız olacak. Yani DeadLock senaryosunda belirtmiş olduğumuz güncelleme işlemlerinin aynı zamanda çalışıyor olması gerekli. Bunu sağlayabilmek için bu güncelleme işlemlerini birer Thread içinde çalıştıracağız. Uygulamamız basit bir Console Application olacak. Şimdi aşağıdaki uygulama kodlarını oluşturalım.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace DeadLockTest
{
    class Deadlock
    {
        /* Iki ayri Transaction için iki ayri SqlConnection nesnesi olusturuyoruz. Bununla birlikte, iki Transaction içindeki komutlari icra edecek iki adet SqlCommand nesnesi ve Transactionlar içinde iki adet SqlTransaction nesnesi tanimliyoruz.*/
        public static SqlConnection conT1 =new SqlConnection("server=localhost;database=Northwind;integrated security=SSPI");
        public static SqlConnection conT2 =new SqlConnection("server=localhost;database=Northwind;uid=sa;integrated security=SSPI");
        public static SqlCommand cmdT1;
        public static SqlCommand cmdT2;
        public static SqlTransaction T1;
        public static SqlTransaction T2;

        /* Bu metod, Personel tablosunda güncelleme islemini yapiyor. Komut, conT1 SqlConnection' i üzerinden, T1 isimli SqlTransaction' da çalisacak sekilde olusturuluyor. Sonra bu komut yürütülüyor. Metod deadlock senaryomuzun 3ncü adimini gerçeklestiriyor.*/
        public static void GuncellePersonelT1()
        {
            Console.WriteLine("PersonelID=78 olan satirdaki PersonelAd alaninin degeri DENEME yapiliyor...");
            cmdT1=new SqlCommand("UPDATE Personel SET PersonelAd = 'DENEME' WHERE PersonelID = 78", conT1,T1);
            int sonuc = cmdT1.ExecuteNonQuery();
            Console.WriteLine(sonuc+" guncellendi. PersonelAd = DENEME yapildi.");
        }

        /* Bu metod ile DeadLock senaryomuzun 4ncü adimi gerçeklestiriliyor.*/
        public static void GuncelleMusterilerT2()
        {
            Console.WriteLine("MusteriID=1000 olan satirdaki MusteriMail alaninin degeri DENEME@DENEME.COM yapiliyor...");
            cmdT2=new SqlCommand("UPDATE Musteriler SET MusteriMail = 'DENEME@DENEME.COM' WHERE MusteriID = 1000",conT2,T2);
            int sonuc = cmdT2.ExecuteNonQuery();
            Console.WriteLine(sonuc+" guncellendi. MusteriMail = DENEME@DENEME.COM yapildi.");
        }

        /* Bu metod ile DeadLock senaryomuzun 5nci adimi gerçeklestiriliyor.*/
        public static void GuncelleMusterilerT1()
        {
            Console.WriteLine("MusteriID=1000 olan satirdaki MusteriMail alaninin degeri MAIL@MAIL.COM yapiliyor...");
            cmdT1 =new SqlCommand("UPDATE Musteriler SET MusteriMail = 'MAIL@MAIL.COM' WHERE MusteriID = 1000",conT1,T1);
            int sonuc = cmdT1.ExecuteNonQuery();
            Console.WriteLine(sonuc+" guncellendi. MusteriMail = MAIL@MAIL.COM yapildi.");
        }

        /* Bu metod ilede DeadLock senaryomuzun 6nci adimi gerçeklestiriliyor.*/
        public static void GuncellePersonelT2()
        {
            Console.WriteLine("PersonelID=78 olan satirdaki PersonelAd alaninin degeri ISIM yapiliyor...");
            cmdT2=new SqlCommand("UPDATE Personel SET PersonelAd = 'ISIM' WHERE PersonelID = 78", conT2,T2);
            int sonuc = cmdT2.ExecuteNonQuery();
            Console.WriteLine(sonuc+" guncellendi. PersonelAd = ISIM yapildi.");
        }

        public static void Main()
        {
            /* Baglantimiz açiliyor ve ilk transaction baslatiliyor. Ardinan bu Transaction için LOCK_TIMEOUT degeri belirlenerek, kilitlerin ne kadar süre ile beklenecegini belirten sql komutu çalistiriliyor. Süre olarak 3 saniye veriliyor.*/
            conT1.Open();
            T1 = conT1.BeginTransaction();
            cmdT1 = conT1.CreateCommand();
            cmdT1.Transaction = T1;
            cmdT1.CommandText = "SET LOCK_TIMEOUT 3000";
            cmdT1.ExecuteNonQuery();

            /* Ikinci transaction için gerekli baglanti açiliyor, transaction baslatiliyor ve LOCK_TIMEOUT süresi 3 saniye olarak belirleniyor.*/
            conT2.Open();
            T2 = conT2.BeginTransaction();
            cmdT2 = conT2.CreateCommand();
            cmdT2.Transaction = T2;
            cmdT2.CommandText = "SET LOCK_TIMEOUT 3000";
            cmdT2.ExecuteNonQuery();

            /* Izleyen sql cümlecigi ile, DeadLock_Priority degeri LOW olarak belirleniyor. Yani, bir deadLock olusmasi durumunda, cmdT2 nin içinde çalistigi Transaction RollBack edilecektir.*/
            cmdT2.CommandText = "SET DEADLOCK_PRIORITY LOW";
            cmdT2.ExecuteNonQuery();

            /*DeadLock senaryomuzdaki update işlemelerini gerçekleştirecek olan metodlarımız için Thread nesneleri oluşturuluyor ve daha sonra bu Thread'ler başlatılıyor.*/
            Thread Thread1 = new Thread(new ThreadStart(GuncellePersonelT1));
            Thread Thread2 = new Thread(new ThreadStart(GuncelleMusterilerT2));
            Thread Thread3 = new Thread(new ThreadStart(GuncelleMusterilerT1));
            Thread Thread4 = new Thread(new ThreadStart(GuncellePersonelT2));

            Thread1.Start();
            Thread2.Start();
            Thread3.Start();
            Thread4.Start();

            Console.ReadLine();
        }
    }
}
```

![mk77_3.gif](/assets/images/2004/mk77_3.gif)

Şekil 3. Programın Çalışmasının Sonucu.

Uygulamayı çalıştırdığımızda, ilk Thread ve ikinci Thread'ler içinde yürütülen update işlemlerinin gerçekleştirildiğini görürüz. Daha sonra üçüncü Thread için güncelleme işlemi yapılırken, bu Thread'in çalıştığı Transaction 3 saniye süreyle diğer Transaction'daki kilidin açılmasını bekleyecektir. Süre sonunda kilit halen daha açılmamış olacağından (ki kilidin açılması Commit veya RollBack işlemini gerektirir.) DEADLOCK_PRIORITY değeri LOW olarak belirlenen ikinci Transaction RollBack edilecektir. Bununla birlikte SqlException türünden bir istisna da ortama fırlatılır. Bu istisnada bir DeadLock oluştuğu ve prosesler içinde çalışan Transaction'lardan birisininde kurban edileceği belirtilir. Elbette burada istisnayı kontrol etmediğimiz için her iki Transaction içindeki işlemler RollBack edilecektir. Ana amacımız, DeadLock senaryosunun ne zaman gerçekleşeceği ve neler olacağıdır. Böylece DeadLock durumunun nasıl oluştuğunu ve nelere yol açtığını çok kısa ve basit olarak incelemeye çalıştık. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.