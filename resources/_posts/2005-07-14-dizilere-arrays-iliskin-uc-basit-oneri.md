---
layout: post
title: "Dizilere(Arrays) İlişkin Üç Basit Öneri"
date: 2005-07-14 09:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - xml
  - performance
---
Hepimiz dizileri (Arrays) bilir ve kullanırız. Her ne kadar günümüzde koleksiyonlar, xml kaynakları ve tablo yapıları veri saklamak amacıyla daha çok kullanılıyor olsalar da, dizilerdende yoğun şekilde yararlanmaktayız. Örneğin kendi tasarladığımız bir sınıfa ait nesneler topluluğunu pekala bir dizi şeklinde ifade edebilir hatta serileştirebiliriz (serializable). Lakin dizileri kullanırken tercih edeceğimiz ve bize performans açısından avantaj sağlayacak teknikleri çoğu zaman göz ardı ederiz. İşte bu makalemizde dizileri kullanırken işimize yarayacak performans kriterlerinden bahsedeceğiz.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Öneri 1; Dizi elemanları arasında gezinirken kullanacağımız for döngüleri, foreach döngülerine nazaran daha süratli çalışır.

İki döngü yapısı arasındaki farkı daha net olarak görebilmek için aşağıdaki basit console uygulamasını göz önüne almakta fayda var. Uygulamamızda basit olarak double tipinden bir dizi tanımlıyoruz. Bu dizinin boyutunu dikkat ederseniz yüksek verdik. Hem for döngüsünü hem de foreach döngüsünü sırasıyla çalıştıyoruz. İlk olarak for döngüsü ile indeksleyici operatörünü kullanarak dizimizin elemanları üzerinde ilerliyor ve bir toplam değeri alıyoruz. Aynı işlemi daha sonra bir foreach döngüsü ile gerçekleştiriyoruz. (Aldığımız toplama işleminin bizim için özel bir önemi veua anlamı yok.) Döngülerin çalışma sürelerini yaklaşık olarak hesaplayabilmek amacıyla da TimeSpan ve DateTime sınıflarından faydalanıyoruz. Bu hesaplamalar bize yaklaşık olarak döngülerin çalışma sürelerini verecektir.

```csharp
using System;

namespace UsingArrays
{
    class DizilerTest
    {
        static void DiziTest_1()
        {
            double[] dizi=new double[50000000]; 
            double toplam=0;

            for(int i=0;i<dizi.Length;i++)
            {
                dizi[i]=i;
            } 

            #region for döngüsü ile
            DateTime dtBaslangic=DateTime.Now;
            for(int i=0;i<dizi.Length;i++)
            {
                toplam+=dizi[i];
            }
            DateTime dtBitis=DateTime.Now;
            TimeSpan tsFark=dtBitis-dtBaslangic;
            Console.WriteLine(tsFark.TotalSeconds+" saniye...");
            #endregion

            #region foreach döngüsü ile
            toplam=0;
            dtBaslangic=DateTime.Now;
            foreach(int i in dizi)
            {
                toplam+=i;
            }
            dtBitis=DateTime.Now;
            tsFark=dtBitis-dtBaslangic;

            Console.WriteLine(tsFark.TotalSeconds+" saniye...");
            #endregion

            Console.ReadLine();
        } 

        static void Main(string[] args)
        {
            DiziTest_1();
        }
    }
}
```

Kodumuzu bu haliyle çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü elde ederiz. Ben kendi test ortamımda (windows 2000, 512 mb Ram, P4 2.4 ghz cpu tabanlı bir pc) uygulamayı iki kez farklı zamanlarda çalıştırdığımda aşağıdaki sonuçları aldım. Yaklaşık değerler olmasına rağmen, belirgin derecede fark olduğu hemen gözlemlenmektedir. Tabiki uygulamanın verdiği sonuçlar sistemin o anki işlem yoğunluğuna göre değişiklik gösterebilir. Ancak sonuç olarak for döngüleri söz konusu olan iterasyonu foreach döngülerine göre daha hızlı tamamlamaktadır.

İlk çalıştırılış;

![mk129_1.gif](/assets/images/2005/mk129_1.gif)

İkinci çalıştırılış;

![mk129_2.gif](/assets/images/2005/mk129_2.gif)

Elbetteki dizimizdeki eleman sayısının azalması halinde iki döngünün çalışma süreleri arasındaki fark azalacaktır hatta bir birlerine daha da yakınlaşacaktır. Örneğin dizimizi 10 milyon elemanlı olarak tanımlarsak uygulamanın çalışmasının sonucu aşağıdaki gibi olacaktır. Döngülerin çalışma süreleri arasındaki fark azalmış olmasına rağmen yinede for döngüsü iterasyonu çok daha önce tamamlamıştır.

İlk çalıştırılış;

![mk129_3.gif](/assets/images/2005/mk129_3.gif)

İkinci çalıştırılış;

![mk129_4.gif](/assets/images/2005/mk129_4.gif)

Gelelim diziler ile ilgili ikinci önemli öneriye. Bu öneri, geriye dönüş değerleri dizi olan metodlar ile ilgilidir.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Öneri 2; Metodlardan geriye dizi referansları dönüyorsa, dönüş değeri olmayacak koşullu durumlarda null değer yerine 0 elemanlı bir dizinin döndürülmesi uygulamada daha az kontrol yapmamızı sağlar.

Bu öneriyi anlayabilmek için aşağıdaki kod parçasında ye alan basiy console uygulamasını göz önüne alalım.

```csharp
using System;

namespace UsingArrays
{
    class DizilerTest
    {
        static double[] DiziTest_2(int elemanSayisi)
        {
            if (elemanSayisi <= 0)
            {
                return null;
            }
            else
            {
                return new double[elemanSayisi];
            }
        }

        static void Main(string[] args)
        {
            double[] dizi;
            dizi = DiziTest_2(-1);
            for (int i = 0; i < dizi.Length; i++)
            {
                Console.WriteLine(dizi[i]);
            }
        }
    }
}
```

Bu örnekte DiziTest_2 isimli metodumuz diziyi içeride oluşturup referansını geri döndürmek üzere tasarlanmıştır. Eğer bu metoda aktardığımız parametre değerimiz 0 veya negatif ise, metodumuz geriye null değer döndürmektedir. Bu mümkündür çünkü diziler bildiğiniz gibi referans türünden elemanlardır. Ana kodumuzda ise for döngüsü yardımıyla oluşturulan dizinin elemanları ekrana yazdırılmaktadır. Uygulamamızı bu haliyle derlediğimizde aşağıdaki ekran görüntüsünde olduğu gibi çalışma zamanında NullReferenceException istisnası ile karşılaşırız. Bu sonuç for döngüsü yerine foreach döngüsünü kullandığımız takdirde de kaçınılmazdır.

![mk129_5.gif](/assets/images/2005/mk129_5.gif)

Sorunu çözebilmek için döngünün çalıştırılmasından hemen önce dizi referansının null olup olmadığının kontrolünü yapmamız gerekir. Dolayısıyla kodumuzu aşağıdaki gibi düzenlemeliyiz.

```csharp
double[] dizi;
dizi = DiziTest_2(-1);
if (dizi != null) 
{
    foreach (double eleman in dizi)
    {
        Console.WriteLine(eleman);
    }
}
```

Ancak basit bir teknik ile bu kontrolü atlayabiliriz. Tek yapmamız gereken DiziTest_2 isimli metodumuzda null değer yerine 0 elemanlı bir dizi referansı döndürmek olacakır. Yani;

```csharp
static double[] DiziTest_2(int elemanSayisi)
{
    if (elemanSayisi <= 0)
    {
        return new double[0];
    }
    else
    {
        return new double[elemanSayisi];
    }
}
```

Bu tekniğin performans açısından kazanımı olup olmadığı tartışılacak bir konudur. Ancak bizi istisna yakalama ve if kontrolünden kurtarmıştır.

Diziler ile ilgili üçüncü önerimiz ise özellikle iki boyutlu dizileri ilgilendirmektedir.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Öneri 3; İki boyutlu dizilerde iç içe kullanılan döngülerde en dıştan içe doğru gerçekleştirilen ötelemelerde, dizinin boyutlarındaki sıraya göre işlem yapılması performansı olumlu yönde etkiler.

Ne kastetmek istediğimizi anlayabilmek için aşağıdaki kod parçasında verilen örneği göz önüne alalım.

```csharp
static void DiziTest(int x)
{
    int[,] dizi = new int[x, x];
    DateTime dtBaslangic, dtBitis;
    TimeSpan tsFark;

    dtBaslangic = DateTime.Now;
    for (int i = 0; i < x; i++)
        for (int j = 0; j < x; j++)
            dizi[i, j] = i + j;
    dtBitis = DateTime.Now;
    tsFark = dtBitis - dtBaslangic;
    Console.WriteLine("Geçen süre...{0} saniye.", tsFark.TotalSeconds);

    dtBaslangic = DateTime.Now;
    for (int i = 0; i < x; i++)
        for (int j = 0; j < x; j++)
            dizi[j, i] = i + j;
    dtBitis = DateTime.Now;
    tsFark = dtBitis - dtBaslangic;
    Console.WriteLine("Geçen süre...{0} saniye.", tsFark.TotalSeconds);
}

static void Main(string[] args)
{
    for (int testSayisi = 0; testSayisi < 5; testSayisi++)
    {
        DiziTest(6000);
        Console.WriteLine("-------------");
    }
}
```

Bu kod parçasında, iki boyutlu bir diziye eleman atanışları için iki adet iç-içe geçmiş döngü kullandığımızı görüyorsunuz. Bu döngüler arasında ki tek fark dizi elemanlarına erişim şekli. İlk döngüde i,j sırası kullanılırken ikinci döngümüzde j,i sırası kullanılıyor. İkinci iç - içe döngümüzde aslında i,j sırasında bir iterasyon yapılmasına rağmen dizi elemanlarına j,i sırasında erişilmekte. Bu hepimizin gözden kaçırabileceği bir nokta. Dalgınlık sonucu dahi bu tarz bir kod satırı yazma ihtimalimiz var. Peki ya bu tarz bir yazımın sonuçları ne olabilir? Döngülerin çalışmalarının sonucu aynı olsa da aşağıdaki şekil aradaki farkı açıkça ortaya koymaktadır. Sistemden siteme süreler fark gösterebilir, ancak iki döngü mekanizmasının arasındaki süre farkı çok açık ve belirgindir.

![mk129_6.gif](/assets/images/2005/mk129_6.gif)

Elbette aynı durum boyutları farklı bir dizi içinde geçerlidir. Örneğimizdeki döngü yapılarını aşağıdaki gibi değiştirelim ve tekrar deneyelim.

```csharp
static void DiziTest(int x,int y)
{
    int[,] dizi = new int[x, y];
    DateTime dtBaslangic, dtBitis;
    TimeSpan tsFark;

    dtBaslangic = DateTime.Now;
    for (int i = 0; i < x; i++)
        for (int j = 0; j < y; j++)
            dizi[i, j] = i + j;
    dtBitis = DateTime.Now;
    tsFark = dtBitis - dtBaslangic;
    Console.WriteLine("Geçen süre...{0} saniye.", tsFark.TotalSeconds);

    dtBaslangic = DateTime.Now;
    for (int i = 0; i < y; i++)
        for (int j = 0; j < x; j++)
            dizi[j, i] = i + j;
    dtBitis = DateTime.Now;
    tsFark = dtBitis - dtBaslangic;
    Console.WriteLine("Geçen süre...{0} saniye.", tsFark.TotalSeconds);
}

static void Main(string[] args)
{
    for (int testSayisi = 0; testSayisi < 5; testSayisi++)
    {
        DiziTest(6000,5000);
        Console.WriteLine("-------------");
    }
}
```

![mk129_7.gif](/assets/images/2005/mk129_7.gif)

Bu kez süreler biraz daha uzamış görünüyor. Özellikle ikinci iç-içe döngüde. Burada en büyük etkenlerden birisi x-y sırası yerine y-x sırasını tercih edişimizdir. Bunu yapmamızın sebebi tahmin edeceğiniz gibi IndexOutOfRangeException istisnasından kurtulmaktır. Bu küçük hileye rağmen dizi boyutlarına ters sırada erişilmeye çalışılması, performansı olumsuz yönde etkiler. Elbetteki geliştirdiğimiz uygulamalarda çoğu zaman bu tip çok boyutlu döngüleri kullanmayabiliriz. O sebepten her iki kullanımda derleme zamanı hatası vermeyeceği gibi düzgün bir biçimde çalışacaktır. Özellikle küçük boyutlu dizilerde bu farklar çok ama çok azdır. Yine de siz siz olun ve profesyonel bir yazılım geliştirici olarak bu öneriyi ve daha öncekilerini dikkate alın. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.