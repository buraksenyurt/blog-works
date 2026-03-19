---
layout: post
title: "Thread'leri Belli Süreler Boyunca Uyutmak ve Yoketmek"
date: 2004-01-02 12:00:00 +0300
description: "Bir önceki makalemizde hatırlayacak olursanız, iş parçacıkları haline getirdiğimiz metodlarımızda işlemeleri yavaşlatmak amacı ile bazı döngüler kullanmıştık. Gerçek hayatta çoğu zaman, iş parçacıklarının belirli süreler boyunca beklemesini ve süre sona erdiğinde tekrardan işlemelerine kaldığı yerden devam etmesini istediğimiz durumlar olabilir. Önceki makalemizde kullandığımız Suspend metodu ile ilgili iş parçacığını durdurabiliyorduk. Bu ilgili iş parçacıklarını geçici süre ile bekletmenin yollarından birisidir. Ancak böyle bir durumda bekletilen iş parçacığını tekrar hareketlendirmek kullanıcının Resume metodunu çalıştırması ile olabilir. Oysaki biz"
categories:
  - csharp
tags:
  - csharp
  - threading
---
# Thread'leri Belli Süreler Boyunca Uyutmak ve Yoketmek
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde iş parçacıklarının belli süreler boyunca nasıl durgunlaştırılabileceğini yani etkisizleştirebilieceğimizi işlemey çalışıcaz. Ayrıca iş parçacıklarının henüz sonlanmadan önce nasıl yokedildiklerini göreceğiz.

Bir önceki makalemizde hatırlayacak olursanız, iş parçacıkları haline getirdiğimiz metodlarımızda işlemeleri yavaşlatmak amacı ile bazı döngüler kullanmıştık. Gerçek hayatta çoğu zaman, iş parçacıklarının belirli süreler boyunca beklemesini ve süre sona erdiğinde tekrardan işlemelerine kaldığı yerden devam etmesini istediğimiz durumlar olabilir. Önceki makalemizde kullandığımız Suspend metodu ile ilgili iş parçacığını durdurabiliyorduk. Bu ilgili iş parçacıklarını geçici süre ile bekletmenin yollarından birisidir. Ancak böyle bir durumda bekletilen iş parçacığını tekrar hareketlendirmek kullanıcının Resume metodunu çalıştırması ile olabilir. Oysaki biz, iş parçacığımızın belli bir süre boyunca beklemsini isteyebiliriz. İşte böyle bir durumda Sleep metodunu kullanırız. Bu metodun iki adet overload edilmiş versiyonu vardır.

public static void Sleep (int);

public static void Sleep (TimeSpan);

Biz bugünkü uygulamamızda ilk versiyonu kullanacağız. Bu versiyonda metodumuz parametre olarak int tipinde bir değer almaktadır. Bu değer milisaniye cinsinden süreyi bildirir. Metodun Static bir metod olduğu dikkatinizi çekmiş olmalıdır. Static bir metod olması nedeni ile, Sınıf adı ile birlikte çağırılmak zorundadır. Yani herhangibir thread nesnesinin ardından Sellp metodunu yazamassınız. Peki o halde bekleme süresinin hangi iş parçacığı için geçerli olacağını nereden bileceğiz. Bu nedenle, bu metod iş parçacığı olarak tanımlanan metod blokları içerisinde kullanılır. Konuyu örnek üzerinden inceleyince daha iyi anlayacağız. Metod çalıştırıldığında parametresinde belirtilen süre boyunca geçerli iş parçacığını bekletir. Bu bekleme diğer parçacıkların çalışmasını engellemez. Süre sona erince, iş parçacığımız çalışmasına devam edicektir. Şimdi dilerseniz ötnek bir uygulama geliştirelim ve konuya açıklık getirmeye çalışalım.

Formumuzda bu kez üç adet ProgressBar kontrolümüz var. Baslat başlıklı düğmeye bastığımızda iş parçacıklarımız çalışıyor ve tüm ProgressBar'lar aynı anda değişik süreler ile ilerliyor. Burada iş parçacıkları olarak belirlediğimiz metodlarda kullandığımız Sleep metodlarına dikkat edelim. Tabi kodlarımızı yazmadan önce System.Threading isim uzayını eklemeyi unutmayalım.

![mk34_1.gif](/assets/images/2004/mk34_1.gif)

Şekil 1. Form Tasarımımız.

```csharp
public void pb1Ileri()
    {
        for (int i = 1; i < 100; ++i)
        {
            pb1.Value += 1;
            Thread.Sleep(800);
        }
    }
    public void pb2Ileri()
    {
        for (int i = 1; i < 100; ++i)
        {
            pb2.Value += 1;
            Thread.Sleep(500);  /* Metodumuz iş parçacığı olarak başladıktan sonra döngü içince her bir artımdan sonra 500 milisaniye bekler. */
        }
    }

    public void pb3Ileri()
    {
        for (int i = 1; i < 100; ++i)
        {
            pb3.Value += 1;
            Thread.Sleep(300);
        }
    }

    /* ThreadStart temsilcilerimiz ve Thread nesnelerimizi tanımlıyoruz. */
    ThreadStart ts1;
    ThreadStart ts2;
    ThreadStart ts3;
    Thread t1;
    Thread t2;
    Thread t3;

    private void btnBaslat_Click(object sender, System.EventArgs e)
    {
        /* ThreadStart temsilcilerimizi ve Thread nesnelerimizi oluşturuyoruz. */
        ts1 = new ThreadStart(pb1Ileri);
        ts2 = new ThreadStart(pb2Ileri);
        ts3 = new ThreadStart(pb3Ileri);
        t1 = new Thread(ts1);
        t2 = new Thread(ts2);
        t3 = new Thread(ts3);

        /* Thread nesnelerimizi start metodu ile başlatıyoruz. */

        t1.Start();
        t2.Start();
        t3.Start();
    }
```

Uygulamamızı çalıştıralım. Her iş parçacığı Sleep metodu ile belirtilen süre kadar beklemeler ile çalışmasına devam eder. Örneğin pb3Ileri metodunda iş parçacığımız ProgressBar'ın Value değerini her bir arttırdıktan sonra 300 milisaniye bekler ve döngü bir sonraki değerden itibaren devam eder. Sleep metodu ile Suspend metodları arasında önemli bir bağ daha vardır. Bildiğiniz gibi Suspend metodu ilede bir iş parçacığını durdurabilmekteyiz. Ancak bu iş parçacığını tekrar devam ettirmek için Resume metodunu kullanmamız gerekiyor. Bu iki yöntem arasındaki fark idi. Diğeri önemli olgu ise şudur; bir iş parçacığı metodu içinde, Sleep metodunu kullanmış olsak bile, programın herhangibir yerinden bu iş parçacığı ile ilgili Thread nesnesinin Suspend metodunu çağırdığımızda, bu iş parçacığı yine duracaktır. Bu andan itibaren Sleep metodu geçerliliğini, bu iş parçacığı için tekrardan Resume metodu çağırılıncaya kadar kaybedecektir. Resume çağrısından sonra ise Sleep metodları yine işlemeye devam eder.

![mk34_2.gif](/assets/images/2004/mk34_2.gif)

Şekil 2. Sleep Metodunun Çalışması

Şimdi gelelim diğer konumuz olan bir iş parçacığının nasıl yok edileceğine. Bir iş parçacığını yoketmek amacı ile Abort metodunu kullanabiliriz. Bu metod çalıştırıldığında derleyici aslında bir ThreadAbortException istisnası üretir ve iş parçacığını yoketmeye zorlar. Abort yöntemi çağırıldığında, ilgili iş parçacığını tekrar resume gibi bir komutla başlatamayız. Diğer yandan Abort metodu iş parçacığı ile ilgili metod için ThreadAbortException istisnasını fırlattığında (throw), bu metod içinde bir try..catch..finally korumalı bloğunda bu istisnayı yakalayabiliriz veya Catch bloğunda hiç bir şey yazmas isek program kodumuz kesilmeden çalışmasına devam edicektir.

Abort metodu ile bir iş parçacığı sonlandırıldığında, bu iş parçacığını Start metodu ile tekrar çalıştırmak istersek, "ThreadStateException'Additional information: Thread is running or terminated; it can not restart." hatasını alırız. Yani iş parçacığımızı tekrar baştan başlatmak gibi bir şansımız yoktur. Şimdi bu metodu inceleyeceğimiz bir kod yazalım. Yukarıdaki uygulamamızı aşağıdaki şekilde geliştirelim.

![mk34_3.gif](/assets/images/2004/mk34_3.gif)

Şekil 3. Form Tasarımımız.

Kodlarımıza geçelim.

```csharp
public void pb1Ileri()
    {
        try
        {
            for (int i = 1; i < 100; ++i)
            {
                pb1.Value += 1;
                Thread.Sleep(800);
            }
        }
        catch (ThreadAbortException hata)
        {
        }
        finally
        {
        }

    }

    public void pb2Ileri()
    {
        for (int i = 1; i < 100; ++i)
        {
            pb2.Value += 1;
            Thread.Sleep(500);
            /* Metodumuz iş parçacığı olarak başladıktan sonra döngü içince her bir artımdan sonra 500 milisaniye bekler. */
        }
    }

    public void pb3Ileri()
    {
        for (int i = 1; i < 100; ++i)
        {
            pb3.Value += 1;
            Thread.Sleep(300);
        }
    }

    /* ThreadStart temsilcilerimiz ve Thread nesnelerimizi tanımlıyoruz. */
    ThreadStart ts1;
    ThreadStart ts2;
    ThreadStart ts3;
    Thread t1;
    Thread t2;
    Thread t3;

    private void btnBaslat_Click(object sender, System.EventArgs e)
    {
        /* ThreadStart temsilcilerimizi ve Thread nesnelerimizi oluşturuyoruz. */
        ts1 = new ThreadStart(pb1Ileri);
        ts2 = new ThreadStart(pb2Ileri);
        ts3 = new ThreadStart(pb3Ileri);

        t1 = new Thread(ts1);
        t2 = new Thread(ts2);
        t3 = new Thread(ts3);

        /* Thread nesnelerimizi start metodu ile başlatıyoruz. */

        t1.Start();
        t2.Start();
        t3.Start();

        btnBaslat.Enabled = false;
        btnDurdur.Enabled = true;
        btnDevam.Enabled = false;
    }

    private void btnDurdur_Click(object sender, System.EventArgs e)
    {
        t1.Abort();
        /* t1 isimli Thread'imizi yokediyoruz. Dolayısıyla pb1Ileri isimli metodumuzunda çalışmasını sonlandırmış oluyoruz. */
        /* Diğer iki iş parçacığını uyutuyoruz. */
        t2.Suspend();
        t3.Suspend();
        btnDurdur.Enabled = false;
        btnDevam.Enabled = true;
    }

    private void btnDevam_Click(object sender, System.EventArgs e)
    {

        /* İş parçacıklarını tekrar kaldıkları yerden çalıştırıyoruz. İşte burada t1 thread nesnesini Resume metodu ile tekrar kaldığı yerden çalıştıramayız. 
        * Bu durumda programımız hataya düşecektir. Nitekim Abort metodu ile Thread'imiz sonlandırılmıştır. Aynı zamanda Start metodu ile Thread'imizi baştanda başlatamayız.*/
        t2.Resume();
        t3.Resume();
        btnDurdur.Enabled = true;
        btnDevam.Enabled = false;
    }
```

Uygulamamızı deneyelim.

![mk34_4.gif](/assets/images/2004/mk34_4.gif)

Şekil 4. Uygulamanın çalışması sonucu.

Değerli okurlarım geldik bir makalemizin daha sonuna. Bir sonraki makalemizde Threading konusunu işlemeye devam edeceğiz. Hepinize mutlu günler dilerim.