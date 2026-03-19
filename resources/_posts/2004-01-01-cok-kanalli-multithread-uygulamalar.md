---
layout: post
title: "Çok Kanallı(Multithread) Uygulamalar"
date: 2004-01-01 12:00:00 +0300
description: "Bugünkü makelemiz ile birlikte threading kavramını en basit haliyle tanımaya çalışacağız. Sonraki makalelerimizde de threading kavramını daha üst seviyede işlemeye çalışacağız. Bugün hepimiz bilgisayar başındayaken aynı anda pek çok uygulamanın sorunsuz bir şekilde çalıştığını görürüz. Bir belge yazarken, aynı zamanda müzik dinleyebilir, internet üzerinden program indirebilir ve sistemimizin kaynaklarının elverdiği ölçüde uygulamayla eşzamanlı olarak çalışabiliriz. Bu bize, günümüz işlemcilerinin ve üzerlerinde çalışan işletim sistemlerinin ne kadar yetenekli oluğunu gösterir. Gösterir mi acaba?"
categories:
  - csharp
tags:
  - csharp
  - threading
  - concurrency
  - delegates
---
# Çok Kanallı(Multithread) Uygulamalar
Değerli Okurlarım, Merhabalar.

Bugünkü makelemiz ile birlikte threading kavramını en basit haliyle tanımaya çalışacağız. Sonraki makalelerimizde de threading kavramını daha üst seviyede işlemeye çalışacağız. Bugün hepimiz bilgisayar başındayaken aynı anda pek çok uygulamanın sorunsuz bir şekilde çalıştığını görürüz. Bir belge yazarken, aynı zamanda müzik dinleyebilir, internet üzerinden program indirebilir ve sistemimizin kaynaklarının elverdiği ölçüde uygulamayla eşzamanlı olarak çalışabiliriz. Bu bize, günümüz işlemcilerinin ve üzerlerinde çalışan işletim sistemlerinin ne kadar yetenekli oluğunu gösterir. Gösterir mi acaba?

Aslında tek işlemcili makineler günümüzün modern sihirbazları gibidirler. Gerçekte çalışan uygulamaların tüm işlemleri aynı anda gerçekleşmemektedir. Fakat işlemciler öylesine büyük saat hızlarına sahiptirlerki. İşlemcinin yaptığı, çalıştırılan uygulamaya ait işlemleri iş parçacacıkları (thread) halinde ele almaktır. Her bir iş parçacağı bir işlemin birden fazla parçaya bölünmesinden oluşur. İşlemciler her iş parçacığı için bir zaman dilimi belirler. T zaman diliminde bir işlem parçacığı yürütülür ve bu zaman dilim bittiğinde işlem parçacığı geçici bir süre için durur. Ardından kuyrukta bekleyen diğer iş parçacağı başka bir zaman dilimi içinde çalıştırılır. Bu böylece devam ederken, işlemcimiz her iş parçacığına geri döner ve tüm iş parçacıkları sıra sıra çalıştırılır. Dedik ya, işlemciler bu işlemleri çok yüksek saat ve frekans hızında gerçekleştiri. İşte bu yüksek hız nedeniyle tüm bu olaylar saniyenin milyon sürelerinde gerçekleşir ve sanki tüm bu uygulamalar aynı anda çalışıyor hissi verir.

Gerçektende uygulamaları birbirleriyle paralel olarak ve eş zamanlı çalıştırmak aslında birden fazla işlemciye sahip sistemler için gerçeklenir.

Bugünkü uygulamamız ile, bahsetmiş olduğumuz threadin kavramına basit bir giriş yapıcağız. Nitekim threading kavramı ve teknikleri, uygulamalarda profesyonel olarak kod yazmayı gerektirir. Daha açık şekilde söylemek gerekirse bir uygulama içinde yazdığımız kodlara uygulayacağımı thread'ler her zaman avantaj sağlamaz. Bazı durumlarda dezavantaja dönüşüp programların daha yavaş çalışmasına neden olabilir. Nitekim thread'lerin çalışma mantığını iyi kavramak ve uygulamalarda titiz davranmak gerekir.

Örneğin thread'lerin zaman dilimlerine bölündüklerinde sistemin nasıl bir önceki veya daha önceki thread'i çalıştırabildiğini düşünelim. İşlemci zaman dilimini dolduran bir thread için donanımda bir kesme işareti bırakır, bunun ardında thread'e ait bir takım bilgiler belleğe yazılır ve sonra bu bellek bölgesinde Context adı verilen bir veri yapısına depolanır. Sistem bu thread'e döneceği zaman Context'te yer alan bilgilere bakar ve hangi donanımın kesme sinyali verdiğini bulur. Ardından bu sinyal açılır ve işlemin bir sonraki işlem parçacığının çalışacağı zaman dilimine girilir. Eğer thread işlemini çok fazla kullanırsanız bu durumda bellek kaynaklarınıda fazlası ile tüketmiş olursunuz. Bu thread'leri neden titiz bir şekilde programlamamız gerektiğini anlatan nedenlerden sadece birisidir. Öyleki yanlış yapılan thread programlamaları sistemlerin kilitlenmesine de yol açacaktır.

Threading gördüğünüz gibi çok basit olmayan bir kavramdır. Bu nedenle olayı daha iyi açıklayabileceğimi düşündüğüm örneklerime geçmek istiyorum. Uygulamamızın formu aşağıdaki şekildeki gibi olucak.

![mk33_1.gif](/assets/images/2004/mk33_1.gif)

Şekil 1. Form Tasarımımız.

Şimdi kodlarımızı yazalım.

```csharp
public void z1() 
{ 
    for (int i = 1; i < 60; ++i) 
    { 
        zaman1.Value += 1; 
        for (int j = 1; j < 10000000; ++j) 
        { 
            j += 1; 
        } 
    } 
}
public void z2() 
{ 
    for (int k = 1; k < 100; ++k) 
    { 
        zaman2.Value += 1; 
        for (int j = 1; j < 25000000; ++j) 
        { 
            j += 1; 
        } 
    } 
}
private void btnBaslat_Click(object sender, System.EventArgs e) 
{ 
    z1(); 
    z2(); 
}
```

Program kodlarımızı kısaca açıklayalım. Z1 ve z2 isimli metodlarımız progressBar kontrolllerimizin değerlerini belirli zaman aralıklarıyla arttırıyorlar. Bu işlemleri geçekleştirmek için, Başlat başlıklı butonumuza tıklıyoruz. Burada önce z1 daha sonrada z2 isimli metodumuz çalıştırılıyor. Bunun sonucu olarak önce zaman1 isimli progressBar kontrolümüz doluyor ve dolması bittikten sonra zaman2 isimli progressBar kontrolümüzün value değeri arttırılarak dolduruluyor.

Şimdi bu programın şöyle çalışmasını istediğimizi düşünelim. Her iki progressBar'da aynı anda dolmaya başlasınlar. İstediğimiz zaman z1 ve z2 metodlarının çalışmasını durduralım ve tekrar başlatabilelim. Tekrar başlattığımızda ise progressBar'lar kaldıkları yerden dolmaya devam etsinler. Söz ettiğimiz aslında her iki metodunda aynı anda çalışmasıdır. İşte bu işi başarmak için bu metodları sisteme birer iş parçacağı (thread) olarak tanıtmalı ve bu thread'leri yönetmeliyiz.

.Net ortamında thread'ler için System.Threading isim uzayını kullanırız. Öncelikle programımıza bu isim uzayını ekliyoruz. Ardından z1 ve z2 metodlarını birer iş parçacığı olarak tanımlamamız gerekiyor. İşte kodlarımız.

```csharp
public void z1()
{
    for (int i = 1; i < 60; ++i)
    {
        zaman1.Value += 1;
        for (int j = 1; j < 10000000; ++j)
        {
            j += 1;
        }
    }
}
public void z2()
{
    for (int k = 1; k < 100; ++k)
    {
        zaman2.Value += 1;
        for (int j = 1; j < 25000000; ++j)
        {
            j += 1;
        }
    }
}
ThreadStart ts1;
ThreadStart ts2;
Thread t1;
Thread t2;
private void btnBaslat_Click(object sender, System.EventArgs e)
{
    ts1 = new ThreadStart(z1);
    /* ThreadStart iş parçacığı olarak kullanılıcak metod için bir temsilcidir. 
        * Bu metod için tanımlanacak thread sınıfı nesnesi için paramtere olucak ve 
        * bu nesnenin hangi metodu iş parçacığı olarak göreceğini belirtecektir. 
        */
    ts2 = new ThreadStart(z2);
    t1 = new Thread(ts1);
    t2 = new Thread(ts2);
    t1.Start(); /* İş parçağını Start metodu ile başlatıyoruz. */
    t2.Start();

    btnBaslat.Enabled = false;
}
private void btnDurdur_Click(object sender, System.EventArgs e)
{
    t1.Suspend();
    /* İş parçacağı geçici bir süre için uyku moduna geçer. 
        * Uyku modundaki bir iş parçacağını tekrar aktif hale getirmek için Resume metodu kullanılır. */
    t2.Suspend();
}
private void btnDevam_Click(object sender, System.EventArgs e)
{
    t1.Resume();
    /* Uyku modundaki iş parçacığının kaldığı yerden devam etmesini sağlar. */
    t2.Resume();
}
private void btnKapat_Click(object sender, System.EventArgs e)
{
    if (t1.IsAlive)
    /* Eğer iş parçacıkları henüz sonlanmamışsa bunlar canlıdır ve IsAlive özellikleri true değerine sahiptir. 
        * Programımızda ilk biten iş parçacığı t1 olucağından onun bitip bitmediğini kontrol ediyoruz. 
        * Eğer bitmiş ise programımız close metodu sayesinde kapatılabilir. */
    {
        MessageBox.Show("Çalışan threadler var program sonlanamaz.");
    }
    else
    {
        this.Close();
    }
}
```

Uygulamamızda z1 ve z2 isimli metodlarımızı birer iş parçacığı (thread) haline getirdik. Bunun için System.Threding isim uzayında yer alan ThreadStart ve Thread sınıflarını kullandık. ThreadStart sınıfı, iş parçacığı olucak metodu temsil eden bir delegate gibi davranır. İş parçacıklarını başlatıcak (start), durdurucak (suspend), devam ettirecek (resume) thread nesnelerimizi tanımladığımız yapıcı metod ThreadStart sınıfından bir temsilciyi parametre olarak alır. Sonuç itibariyle kullanıcı Başlat başlıklı button kontrolüne tıkladığında, her iki progressBar kontrolününde aynı zamanda dolmaya başladığını ve ilerlediklerini görürüz. Bu aşamada Durdur isimli button kontrolüne tıklarsak her iki progressBar'ın ilerleyişinin durduğunu görürüz. Nitekim iş parçacıklarının Suspend metodu çağırılmış ve metodların çalıştırılması durdurulmuştur.

![mk33_2.gif](/assets/images/2004/mk33_2.gif)

Şekil 2. Suspend metodu sonrası.

Bu andan sonra tekrar Devam button kontrolüne tıklarsak thread nesnelerimiz Resume metodu sayesinde çalışmalarına kaldıkları yerden devam ediceklerdir. Dolayısıyla progressBar kontrolllerimizde kaldıkları yerden dolmaya devam ederler. Bu sırada programı kapatmaya çalışmamız henüz sonlanmamış iş parçacıkları nedeni ile hataya neden olur. Bu nedenle Kapat button kontrolünde IsAlive özelliği ile iş parçacıklarının canlı olup olmadığı yani metodların çalışmaya devam edip etmediği kontrol edilir. Eğer sonlanmamışsa kullanıcı aşağıdaki mesaj kutusu ile uyarılır.

![mk33_3.gif](/assets/images/2004/mk33_3.gif)

Şekil 3. İş Parçacıkları henüz sonlanmamış ise.

Evet geldik Threading ile ilgili makale dizimizin ilk bölümünün sonuna. Bir sonraki makalemizde Threading kavramını dahada derinlemesine incelemeye çalışacağız. Hepinize mutlu günler dilerim.