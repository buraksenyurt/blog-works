---
layout: post
title: "C# Temelleri - Olayları(Events) Kavramak"
date: 2007-06-06 09:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - events
  - delegate
  - event-driven-programming
  - oop
---
Olaylar (Events), görsel uygulamalar ile uğraşan her geliştirici tarafından bilinçli veya bilinçsiz bir şekilde kullanılmaktadır. Nesne yönelimli programlama ortamında olayları tanımlamak için klasik olarak verilen bir örnek vardır. Hepinizin bir sonraki cümlede ne diyeceğimi bildiğinizden eminim. Söz konusu örnekte görsel ortamda yer alan bir düğme kontrolü (çoğunlukla Button sınıfına ait bir nesne örneği) ve bu düğmeye kullanıcının mouse ile basması sonucu oluşan Click isimli bir olay mevcuttur.

Oysaki olayların her zaman için görsel ortamda olması ve işletim sistemi tarafından algılanacak bir etkileşime karşılık olarak uygulama ortamına fırlatılması şart değildir. Dolayısıyla olayları kavrayabilmenin en güzel yolu, geliştirici tarafından yazılan tiplere özel olarak nasıl yazılacağını ve kullanılacağını bilmekle mümkün olabilir. İşte bu makalemizde kendi tiplerimiz için özel olayları nasıl yazabileceğimizi incelemeye başlayacak ve olayları daha derinlemesine kavramaya çalışacağız.

Öncelikli olarak düğme kontrolü örneğinden sıyrılıp farklı vakkalar düşünerek ilerlemeye çalışalım. Örnek olarak stoktaki ürün bilgilerini içeren basit bir sınıf göz önüne alınabilir. Ürünlerin stoktaki değerlerininde bu tip içerisinde bir özellik yardımıyla sarmalanmış (wrap) olduğunu düşünebiliriz. Buna göre stok miktarının belirli bir değerin altına düşmesi sonrasında başka nesne örnekleri tarafından ele alınabilecek bir olay tanımlaması yapılabilir. Burada Urun tipi içerisinde tanımlanan ve bu tipe ait nesne örneklerinin ele alabileceği bir olay söz konusudur.

Başka bir örnek daha. Herhangibir bir kargo şirketinin ulaştırma filosundaki araçların programatik ortamda birer nesne ile ifade edilebildiği bir kütüphane (library) olduğunu düşünelim. Bu kütüphane içerisindeki fonksiyonelliklerden biriside, araçların uydu sistemleri yardımıyla düzenli olarak izlenmesi ve güncel koordinat, anlık hız gibi bilgilerinin elde edilmesi olarak göz önüne alınabilir. Bu hizmeti sağlayan kodlar ayrı bir kütüphane olacak şekilde geliştirilmiş olarak ticari bir paket halinde sunulabilir. O halde araçların belirlenen hız limitlerini aşmaları sonrasında oluşacak durumların söz konusu kütüphaneyi kullanan uygulamalar tarafından, istenirse ele alınmalarını sağlamak amacıyla olaylar yazılabilir. Böylece söz konusu program, araç hız limitini aştığı zaman neler yapmak istiyorsa bunları istediği şekilde ele alabilecektir.

Temel olarak kendi tiplerimiz için olay tanımlamak aslında temsilcileri (delegates) daha kolay kapsüllenmiş bir halde sunmak şeklinde de yorumlanabilir. Bir olayın (event) tanımlanabilmesi için mutlaka bir temsilci tipi ile eşleştirilmesi gerekmektedir.

> Temsilciler (delegates) çok kanallı programlamada (multi threading),asenkron (asynchronous) mimarilerde (Polling, Callback, WaitHandle gibi) ve son olarak olay tabanlı (event based) kodlamada kullanılmaktadır.

Temsilci dışında dikkat edilmesi gereken bir diğer noktada olayın bir şekilde ortama fırlatılmasını sağlamaktır. Düğme örneğini burada göz önüne alabiliriz. Dikkat ederseniz bir düğmeye basıldığında gerçekleştirilmek istenenleri yazmak için tek yapılan oluşan olay metodunun içeriğini doldurmaktan ibarettir. Sistem arka tarafta söz konusu Button nesne örneği için bir olay yüklemsi yapmaktadır. Peki düğmeye basıldığında söz konusu olay metodu nasıl çağırılacaktır? (Burada temsilcinin rolünün ne kadar önemli olduğu ortadadır)

Olayın tetiklenmesi işletim sistemi tarafından gerçekleştirilir. Aslında Button nesne örneğinin arka planda yaptıklarından biriside, işletim sistemindeki bu aksiyonu yakalamaktır. Sonuç itibariyle kullanıcının söz konusu Button nesne örneğine yükleme yaptığı olay metodu çağırılır. Bu anlatılanlar bize şunu ifade etmelidir. Kendi olaylarımızı tanımlıyorsak, söz konusu olayın diğer nesneler tarafından ele alınabilmesini sağlamak için manuel olarak tetiklemeliyiz. Manuel olarak yapılan bu tetiklemenin sonucunda çalışma zamanında ele alınacak olay metodunun işaret edilmesini ise, temsilciler (delegates) yardımıyla sağlamalıyız.

> Kendi tiplerimiz için olay tanımlıyorsak bu olayın çalışma zamanında diğer bir nesne tarafından ele alınabilmesi için bir şekilde tetiklenmesi gerekmektedir.

Aslında bir olayın tetiklenmesi, bir istisna (exception) nesne örneğinin ortama fırlatılmasına (throw) benzetilebilir. Tek fark ortama fırlatılan istisnaların catch blokları ile yakalanabiliyor olmasıdır. Olaylarda durum farklıdır. Ortada bir catch bloğu yoktur. Bunun yerine bir abone (subscriber) vardır. Basit olarak olayın tetiklenmesi sonucu çalıştırılacak olay metodunun bulunduğu nesne örneğini abone olarak düşünebiliriz. Bir başka deyişle olayı yakalayıp değerlendirecek olan nesne, olayın sahibi olan nesnenin ilgili olayına (event) abone olmaktadır. Dolayısıyla olayı tanımlayan ve tetikleyen nesneyi yayımcı (publisher) olarakda göz önüne alabiliriz. Aslında bahsettiğimiz kavramları daha kolay anlayabilmek amacıyla aşağıdaki grafiği incelemekte fayda vardır.

![mk207_1.gif](/assets/images/2007/mk207_1.gif)

Birinci adıma göre Program nesnesi Urun tipine ait bir nesne örneği oluşturur. Program nesne örneğinden kasıt aslında uygulamanın ta kendisi olabilir. Örneğin bir konsol uygulamasındaki Program sınıfı veya windows uygulamasındaki bir Form nesne örneği olabilir. Hangisi olursa olsun değişmez gerçek olaya abone olmak isteyen bir nesnenin olmasıdır. Olay tanımlamamızın Urun tipi içerisinde olduğu varsayılırsa, Program nesnesinin ilgili olay metodunu Urun nesnesine abone etmesi gerekmektedir. Bu ikinci adımda sembolize edilmeye çalışılmaktadır.

Nesne kullanıcısı (Object User), Program nesnesi içerisinde olay yüklemesi ile birlikte bir olay metodunuda yazar. Böylece Urun nesnesinin üçüncü adımda yapacağı tetikleme sonucunda Program nesnesi tarafından yazılan olay metodu çağırılabilecektir. Burada söz konusu olan abone etme işlemi aslında Urun sınıfına ait olayın (event) += operatörü yardımıyla Program nesne örneği içerisinde yüklenmesidir. Olaylar tanımlanırken hep bir temsilci (delegate) tipi yardımıyla oluşturulurlar. Dolayısıyla Program sınıfı içerisinde Urun nesnesi için ilgili olay += operatör ile yüklendiğinde, temsilciye (delegate) parametre olarak verilen metod referansı Urun nesnesine bildirilir. Böylece Urun nesne örneği içerisinde ilgili olay tetiklendiğinde hangi metodun çağırılacağı bilinmektedir.

Bu kadar teorik bilgiyle aslında, olayların gerçek anlamda sadece button ve click kelimelerinden ibaret olmadığını göstermeye çalıştık. Artık birazda pratik yaparak bahsedilenleri örneklemekte fayda olacağı kanısındayım. Bu amaçla basit bir console uygulamasını göz önüne alacağız. Konsol uygulamamız içerisinde yer alan Program sınıfımız abonemiz (subscriber) olacak. Urun sınıfı içerisinde tanımlayacağımız olay (Event), stoktaki ürün sayısı 10 değerinin altına düştüğünde tetiklenecek şekilde tasarlanacaktır. Bir olay tanımlanırken mutlaka bir temsilcinin olması gerektiğinden bahsetmiştik. Dolayısıyla birde temsilci (delegate) tipi geliştirmemiz gerekecektir. Söz konusu temsilci tipini ve Urun sınıfını aşağıdaki gibi tasarlayabiliriz. (Örneklerimizde sadece olay kavramına yoğunlaşmak istediğimizden, yapılması gereken pek çok kontrol ortadan kaldırılmıştır. Örneğin ürün adının boş geçilmesini, StokMiktari veya BirimFiyat özelliklerine sıfırın altında değer atanmasını engellemek gibi. Daha pek çok kontrol ve fonksiyonellik düşünülebilir elbette. Siz kendi uygulamalarınızda bu noktaları sakın gözden kaçırmayın ve mutlaka uygulayın.)

![mk207_2.gif](/assets/images/2007/mk207_2.gif)

```csharp
using System;

namespace Olaylar
{
    delegate void StokAzaldiEventHandler();

    class Urun
    {
        private int id;
        private string ad;
        private double birimFiyat;
        private int stokMiktari;

        public event StokAzaldiEventHandler StokAzaldi;
    
        public int StokMiktari
        {
            get { return stokMiktari; }
            set {
                    stokMiktari = value; 
                    if (value < 10
                        && StokAzaldi != null)
                            StokAzaldi(); 
                }
        }

        public double BirimFiyat
        {
            get { return birimFiyat; }
            set { birimFiyat = value; }
        }

        public string Ad
        {
            get { return ad; }
            set { ad = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Urun(int idsi, string adi, double fiyati, int stokSayisi)
        {
            Id = idsi;
            Ad = adi;
            BirimFiyat = fiyati;
            StokMiktari = stokSayisi;
        }
    }
}
```

Şimdi kodlarımızda neler yaptığımıza kısaca bakalım. İlk olarak StokAzaldiHandler tipinden bir temsilci (delegate) tanımlıyoruz. Hatırlayacağınız gibi zaman zaman.Net içerisinde var olan isimlendirme standartlarından bahsediyoruz. Niteliklere ait sınıf adlarının Attribute kelimesi ile, istisna (exception) tiplerinin Exception kelimesi ile bittiklerini biliyoruz. Özellikle olaylar ile ilişkili temsilcilerinde çoğunlukla EventHandler kelimesi ile bittiğini görürüz. Bu nedenle olay ile ilişkili temsilcimizi StokAzaldiEventHandler olarak isimlendirdik. StokAzaldiEventHandler isimli temsilci tipi, geriye değer döndürmeyen ve parametre almayan metodları işaret edebilecek şekilde tasarlanmıştır.

> Temsilcilerin (delegate) çalışma zamanında metodların başlangıç adreslerini işaret ettiklerini ve işaret edebileceği metodun parametrik yapısı ile geri dönüş tipini belirtiklerini hatırlayalım.

Gelelim Urun sınıfımıza. Urun sınıf içerisinde UrunAzaldi isimli bir olay (event) tanımlanmıştır.

```csharp
public event StokAzaldiEventHandler StokAzaldi;
```

Dikkat edilecek olursa event anahtar kelimesinden sonra StokAzaldiEventHandler isimli temsilci tipi gelmektedir. Son olarakta olayın adı yer alır. Böylece söz konusu olay için çalıştırılabilecek olay metodlarının yapısını StokAzaldiEventHandler isimli temsilcinin söyleyeceğide belirtilmiş olur. Geriye kalan tek pürüz, ilgili olay metodun nasıl ve nerede tetikleneceğidir. Örnek olması açısından StokMiktari isimli özelliğin set bloğunda aşağıdaki kod parçası kullanılmıştır.

```csharp
set {
    stokMiktari = value; 
    if (value < 10
        && StokAzaldi != null)
           StokAzaldi(); 
    }
```

Burada stok miktarı eğer 10 rakamının altındaysa ve StokAzaldi olayı null değere eşit değilse StokAzaldi () isimli bir metod çağrısı yapılmaktadır. StokAzaldi olayının null olmaması bir şekilde += operatörü ile yüklendiği anlamına gelmektedir. Yani başka bir nesne bu olaya kendisini abone (subscribe to) etmiştir. Bu durumda söz konusu olay metodunun buradaki set bloğu içerisinden çağırılması gerekir. Bu iş için yine olayı sanki bir metodmuş gibi çağırmak yeterli olacaktır. Nitekim bu çağrı += operatörü ile bağlanan olay metodunun yürütülmeye başlanması anlamınada gelmektedir. Buraya kadar += operatörü ile olayın yüklenmesi gerektiğinden bahsedip durduk. Peki bu nasıl gerçekleştiriliyor? Cevap aşağıdaki kod parçasında olduğu gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Olaylar
{ 
    class Program
    {
        static void Main(string[] args)
        {
            Urun ciklet = new Urun(10001, "Tipitipitip", 1.20, 35);
            ciklet.StokAzaldi += new StokAzaldiEventHandler(ciklet_StokAzaldi);

            for (int i = 0; i <5; i++)
            {
                ciklet.StokMiktari -= 7;
                Thread.Sleep(600);
                Console.WriteLine(ciklet.Ad + " için stok miktarı " + ciklet.StokMiktari.ToString());
            }
        }

        static void ciklet_StokAzaldi()
        {
            Console.WriteLine("Stok miktarı 10 değerinin altında...Alarrrmmm!");
        }
    }
}
```

Main metodu içerisinde Urun sınıfına ait bir nesne örneklendikten sonra StokAzaldi isimli olay, += operatörü ile yüklenmektedir. Burada Visual Studio kullanılıyorsa += işaretinden sonra iki kez tab tuşuna basmak yeterli olacaktır. Bu durumda Visual Studio otomatik olarak bir olay metodu oluşturacaktır. Örneğimizde bu olay metodu cikletStokAzaldi adıyla anılmaktadır.

![mk207_4.gif](/assets/images/2007/mk207_4.gif)

![mk207_5.gif](/assets/images/2007/mk207_5.gif)

> Aslında bir olay tanımlandığında, bu olayın sahibi olan tip için CIL (Common Intermediate Language) kısmınada add_OlayAdı ve remove_OlayAdı isimli iki metod eklenir. Bu metodlar içerisinde olay yüklemesi yapıldığında veya çıkartıldığında, gereken temsilci bağlama ve ayırma işlemleri yapılmaktadır.
> ![mk207_10.gif](/assets/images/2007/mk207_10.gif)

Program kodu içerisinde test amacıyla StokMiktari özelliğinin değeri 7şer 7şer azaltılmaktadır. Sonuçta ekran çıktısı aşağıdaki gibi olacaktır.

![mk207_3.gif](/assets/images/2007/mk207_3.gif)

Dikkat edilecek olursa StokMiktari özelliğinin 10 değerinin altında olduğu her durum için otomatik olarak olay metodu tetiklenmiş ve içerisinde yazılan kod parçaları çalıştırılmıştır.

Yazılan olay her ne kadar faydalı görünsede bazı eksiklikleri olduğu ortadadır. Örneğin olay metodu içerisinde bir de stoğun o anki miktarının ne olduğunu öğrenebilsek fena olmaz mıydı acaba? Yada birden fazla Urun nesne örneğini aynı olay metoduna bağlayacaksak (ki bu mümkündür), olay metodu içerisinde hangi Urun nesne örneğinin ilgili olayın sahibi olduğunu tespit edebilsek fena olmaz mıydı? İşte bu iki gereksinime benzer ihtiyaçlar,.Net içerisinde var olan tüm olaylar içinde geçerlidir.

O nedenle önceden tanımlanmış olan tüm olaylar aslında standart olarak iki parametre almaktadır. İlk parametre olayı tetikleyen nesne örneğine ait referansın yakalanması için kullanılırken, ikinci parametre olay metodu içerisine bilgi aktarmak maksadıyla ele alınır. Bu standart bir olay temsilcisinin işaret edeceği metodun parametrik yapısıdır. Tahmin edileceği gibi ilk parametre object tipindendir. İkinci parametre ise genellikle EventArgs gibi kelimeler ile biten özel bir sınıftır. Kendi örneğimizi göz önüne aldığımızda öncelikli olarak, olay metoduna özel bilgilerin aktarılmasını sağlayacak şekilde bir tipin geliştirilmesi gerekmektedir.

![mk207_6.gif](/assets/images/2007/mk207_6.gif)

```csharp
class StokAzaldiEventArgs:EventArgs
{
    private int guncelStokMiktari;

    public int GuncelStokMiktari
    {
        get { return guncelStokMiktari; }
        set { guncelStokMiktari = value; }
    }
    public StokAzaldiEventArgs(int gStk)
    {
        GuncelStokMiktari = gStk;
    }
}
```

StokAzaldiEventArgs isimli tipin tek yaptığı, güncel stok miktarını ilgili olay metodu içerisine taşımaktır. Bu tip asıl StokAzaldi olayı için anlamlıdır. Olay argümanlarını taşıyacak kendi tiplerimizi geliştirdiğimizde bunların EventArgs tipinden türetilmesi bir zorunluluk değildir ancak bir gelenektir. Amaç aynen isimlendirme kurallarında olduğu gibi kodun standardize edilmesidir. Nitekim.Net içerisindeki tüm olay argüman tipleri, bir şekilde EventArgs sınıfından türemektedir. Bu şekilde olay metoduna bilgi taşıyabileceğimiz bir tip tanımladıktan sonra temsilcininde aşağıdaki gibi değiştirilmesi gerekmektedir.

```csharp
delegate void StokAzaldiEventHandler(object sender,StokAzaldiEventArgs args);
```

Elbette temsilcide yapılan değişikliklerin olayın tetiklendiği yerede adapte edilmesi gerekmektedir. Bu amaçla StokMiktari özelliğinin set bloğu aşağıdaki gibi değiştirilmelidir.

```csharp
public int StokMiktari
{
    get { return stokMiktari; }
    set {
        stokMiktari = value;
        if (value < 10
            && StokAzaldi != null)
                StokAzaldi(this, new StokAzaldiEventArgs(value)); 
    }
}
```

Dikkat edilecek olursa ilk parametreye this anahtar kelimesi getirilmiştir. Hatırlayacağınız gibi ilk parametre için, olayı tetikleyen nesne referansını taşıdığını belirtmiştik. İşte buradaki this anahtar kelimesi çalışma zamanındaki (run-time) nesne referansının alınıp ilgili olay metoduna gönderilmesini sağlamaktadır. İkinci parametrede ise StokAzaldiEventArgs tipinden bir nesne örneği oluşturulmakta ve güncel stok miktarının değeri value anahtar kelimesi ile yapıcı metoduna gönderilmektedir. Tahmin edeceğiniz üzere nesne örneğide, olay metodu içerisinde ele alınabilecektir. Artık program içerisinde kodlarımızıda aşağıdaki gibi düzenlememiz gerekmektedir.

```csharp
class Program
{
    static void Main(string[] args)
    {
        Urun ciklet = new Urun(10001, "Tipitipitip", 1.20, 35);
        ciklet.StokAzaldi += new StokAzaldiEventHandler(ciklet_StokAzaldi);

        for (int i = 0; i <5; i++)
        {
            ciklet.StokMiktari -= 7;
            Thread.Sleep(600);
            Console.WriteLine(ciklet.Ad + " için stok miktarı " + ciklet.StokMiktari.ToString());
        }
    }

    static void ciklet_StokAzaldi(object sender, StokAzaldiEventArgs args)
    {
        Console.WriteLine("Güncel stok değeri {0} . Stokta limit altına inilmiştir. Alarrmmmm!",args.GuncelStokMiktari.ToString());
    }
}
```

Uygulamayı bu haliyle çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçları elde ederiz.

![mk207_7.gif](/assets/images/2007/mk207_7.gif)

Gelelim olaylar ile ilgili bir başka konuya. Daha öncede birden fazla nesne için aynı olay metodunun ele alınabileceğinden bahsetmiştik. Örneğimizi göz önüne aldığımızda, birden fazla Urun nesnesini aynı StokAzaldi olay metoduna yönlendirme şansına sahibiz. Bu şekilde bir ihtiyaç özellikle dinamik olarak oluşturulan kontrollerin bir olay metoduna bağlanarak ele alınması gibi durumlarda kullanılmaktadır. Ki böylece bir den fazla olay metodunu düşünmek yerine tek bir merkez metoddan kontrol ve yönetim işlemlerini gerçekleştirebiliriz. Örneğimizdeki kod parçalarını aşağıdaki gibi değiştirelim.

```csharp
class Program
{
    static void Main(string[] args)
    {
        Urun ciklet = new Urun(10001, "Tipitipitip", 1.20, 35);
        Urun cikolata = new Urun(10034, "Marsi", 2.5, 25);
        cikolata.StokAzaldi+=new StokAzaldiEventHandler(urun_StokAzaldi);
        ciklet.StokAzaldi += new StokAzaldiEventHandler(urun_StokAzaldi);

        for (int i = 0; i <5; i++)
        {
            ciklet.StokMiktari -= 7;
            cikolata.StokMiktari -= 5;
            Thread.Sleep(600);
            Console.WriteLine(ciklet.Ad + " için stok miktarı " + ciklet.StokMiktari.ToString());
            Console.WriteLine(cikolata.Ad + " için stok miktarı " + cikolata.StokMiktari.ToString());
        }
    }

    static void urun_StokAzaldi(object sender, StokAzaldiEventArgs args)
    {
        Urun urn = (Urun)sender;
        Console.WriteLine("{0} için güncel stok değeri {1} . Stokta limit altına inilmiştir. Alarrmmmm!",urn.Ad,args.GuncelStokMiktari.ToString());
    }
}
```

![mk207_8.gif](/assets/images/2007/mk207_8.gif)

İlk olarak ciklet ve cikolata isimli iki ayrı Urun nesnesi örneklediğimize ama bunların her ikisi içinde aynı olay metodunu kullandığımıza dikkat edelim.

```csharp
cikolata.StokAzaldi+=new StokAzaldiEventHandler(urun_StokAzaldi);
ciklet.StokAzaldi += new StokAzaldiEventHandler(urun_StokAzaldi);
```

Dikkat edilmesi gereken önemli noktalardan biriside olay metodu içerisinde sender isimli parametre değişkeninin nasıl kullanıldığıdır. sender isimli değişken cast işlemine tabi tutularak bir Urun nesne örneğine dönüştürülmekte ve kullanılmaktadır. Burada elbetteki akla şu soru gelebilir. Urun nesne örneğine dönüştürme işlemi yapıldıktan sonra güncel stok miktarı gibi verilerde elde edebilir bu nedenle StokAzaldiEventArgs gibi tipleri geliştirmeye ihtiyacımız var mıdır?

Aslında bu bir anlamda doğru olsada bir argüman tipinin var olması, olay metodu içerisine gerçektende ne aktarmak istediğimizi belirten bir kodlama yolu ve standardı sağlamaktadır. Diğer taraftan gereksiz cast işlemlerininde önüne geçilmiş olacaktır. Bunların dışında nesne örneği üzerinden elde edilemeyen ancak argümanlar yardımıyla ele alınabilecek bazı verilerin yayıncı nesne (publisher object) içerisinden sadece olay metoduna aktarılmasıda sağlanabilir. Bir gerekçe daha vardırki o da biraz sonra generic bir temsilci ile karşımıza çıkacaktır.

C# 2.0 ile gelen yeniliklerden biriside isimsiz metodlardır (anonymous methods). Bu kavram özellikle temsilcilerin içerisinde rol aldıgı kodlama alanlarında kullanılmaktadır. Dolayısıyla olay metodlarınıda isimsiz olarak tanımlama ve geliştirme şansına sahibiz. Yani yukarıdaki kodlarımızı aşağıdaki gibi geliştirebiliriz.

```csharp
static void Main(string[] args)
{
    Urun ciklet = new Urun(10001, "Tipitipitip", 1.20, 35);
    ciklet.StokAzaldi += delegate(object sender, StokAzaldiEventArgs arg)
                                {
                                    Urun urn = (Urun)sender;
                                    Console.WriteLine("{0} için güncel stok değeri {1} . Stokta limit altındayız. Alarrmmmm!", urn.Ad, arg.GuncelStokMiktari.ToString());
                                };

    for (int i = 0; i <5; i++)
    {
        ciklet.StokMiktari -= 7;
        Thread.Sleep(600);
        Console.WriteLine(ciklet.Ad + " için stok miktarı " + ciklet.StokMiktari.ToString());
    }
}
```

![mk207_9.gif](/assets/images/2007/mk207_9.gif)

Programı çalıştırdığımızda yine olay metodunun başarılı bir şekilde işletildiğini görebiliriz. Burada dikkat edilecek olursa StokAzaldi olayı (event) yüklenirken isimsiz metod (anonymous method) kullanılmıştır. delegate anahtar kelimesi otomatik olarak StokAzaldiEventHandler temsilcisine (delegate) bürünmektedir. Sonrasında gelen kod bloğu içerisinde ise olay metodunda yapılması gerekenler yer almaktadır. Ortada bir olay metodu adı olmadığına dikkat edelim. Buda zaten neden isimsiz metod denildiğini açıklamaktadır.

C# 2.0 ile birlikte gelen özelliklerden birisi ve belkide en önemliside generic mimaridir. Bildiğiniz gibi generic mimari sayesinde tür bağımsız tipler geliştirebilme şansına sahibiz..Net içerisinde var olan pek çok tipin bu şekilde tür bağımsız versiyonları geliştirilmiş ve tip güvenli ile performans gibi konularda daha güçlü tipler ortaya çıkmıştır. Olaylarla ilişkili olaraktan, EventHandler isimli standart temsilci (delegate) tipinin generic bir versiyonu vardır. Söz konusu temsilcinin prototipi aşağıdaki gibidir.

```csharp
[SerializableAttribute] 
public delegate void EventHandler<TEventArgs> (Object sender,TEventArgs e) where TEventArgs : EventArgs
```

Buna göre kendi olaylarımız için ayrıca temsilci yazmaya gerek kalmamaktadır. Dikkat edilecek olursa TEventArgs isimli generic türün yazılan kısıtlama (constraint) sayesinde EventArgs tipinden türemiş bir tip olması beklenmektedir. Buna göre Urun tipi içerisindeki olay tanımlamasını aşağıdaki gibi değiştirebiliriz.

```csharp
public event EventHandler<StokAzaldiEventArgs> StokAzaldi;
```

EventHandler temsilcisi ilk parametre olarak object tipinden bir değişken almaktadır. İkinci parametre ise EventArgs sınıfından türemiş bir tiptir. Bizim örneğimizde söz konusu tip StokAzaldiEventArgs sınıfıdır. (Sanırım kendi olay argüman tiplerimizi EventArgs sınıfından türetmenin bir faydasını daha görmüş oluyoruz.) Sonuç olarak uygulamanın çalışması değişmeyecektir. Kazancımız ekstradan temsilciler tasarlanmasına gerek kalmayışıdır. Tabi isimsiz bir metod kullanmıyorsak olayın yükleniş şeklinide aşağıdaki gibi değiştirmemiz gerekecektir.

```csharp
ciklet.StokAzaldi += new EventHandler<StokAzaldiEventArgs>(urun_StokAzaldi);
```

Olaylar ile ilgili olarak bilmemiz gereken bir diğer noktada; += ile yüklenen olayların -= ile kaldırılabildiği ve her iki durumunda add ve remove isimli bloklar içerisinde kontrol altına alınabildiğidir. Bu konunun incelemesinide siz değerli okurlarıma bırakıyorum. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde event kavramını daha detaylı bir şekilde incelemeye çalıştık ve kendi olaylarımızı nasıl yazabileceğimizi gördük. Temel olarak işlediklerimizi aşağıdaki maddeler ile özetleyebiliriz.

- Olaylar temsilcilerin (delegates) özelleştirilmiş bir hali olarak düşünülebilir.
- Bir olay tanımlandığında mutlaka bir temsilci tipi ile eşleştirilir. Nitekim olay meydana geldiğinde çağırılacak metodun, birisi tarafından çalışma zamanında (runtime) işaret ediliyor olması gerekir ki bunu temsilciler yapabilir.
- Kendi olaylarımızı geliştirirken isimlendirme standardı açısından temsilcilerimizi EventHandler, olay argümanlarını taşıyacak sınıflarımızı EventArgs kelimeleri ile bitirmekte fayda vardır.
- Olayın tanımlı olduğu nesne tarafından tetiklenmesi sonrasında yakalanabilmesi için, söz konusu nesneyi kullanan diğer nesnenin (subscriber) olaya abone olması gerekmektedir.
- Birden fazla nesne olayını aynı olay metoduna bağlayabiliriz.
- Olay metodlarını işaret edecek temsilciler, ilk parametre olarak olayı meydana getiren nesne referansını taşıyan object bir değişken, ikinci parametre olarakta olay metoduna bilgi taşıyacak bir sınıf örneğini alan metodları işaret edecek biçimde tasarlanırlar.
- Olay metodlarını yazmak zorunlu değildir. Bunun yerine isimsiz metodlarda (anonymous methods) kullanılabilir.
- İstenirse kendi olaylarımız için temsilci yazmak yerine EventHandler generic tipi kullanılabilir.
- Olaylara argüman taşıyacak tiplerimizi hem kod standardı hemde EventHandler desteği için EventArgs sınıfından türetmekte fayda vardır.
- Olayların += operatörü ile yüklenmesi ve -= operatörü ile kaldırılması durumlarını kontrol altına almak için add ve remove bloklarından faydalanılabilir.

Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](/assets/files/2007/Olaylar.zip)