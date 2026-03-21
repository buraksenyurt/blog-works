---
layout: post
title: "WCF - One Way Ticket"
date: 2007-05-31 09:00:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
---
One Way Ticket...One Way Ticket... Bu sözleri duyduğumda aklıma bu şarkıyı yapan eruption ve cover versiyonunu söyleyip efsaneleşen Boney M grupları gelir. Ancak One Way ikilisi ne tesadüftürki.Net Remoting mimarisinde de karşımıza çıkmaktadır. Kısaca tek yön olarak çevirebileceğimiz bu iki kelime aslında fırlat ve unut (fire and forget) anlamındada düşünülebilir. Yada bir başka deyişle istemci tarafından olaya bakıldığında, "metodu çağırdım gerisi umrumda değil" de denebilir. Aynı kelimelerin Windows Communication Foundation içerisinde de yer alması elbetteki şaşırtıcı değildir. Nitekim One Way operasyonlar aslında asenkron istemci-sunucu modelininde önemli bir parçasıdır.

Normal şartlarda Windows Communication Foundation istemcileri servisten bir talepte bulunduklarında, proxy tarafından hazırlanan mesaj sunucuya gönderilir. Servis gelen mesajı alır, çözümler ve gereken yürütme işlemlerini gerçekleştirir. Burada söz konusu yürütmeye dahil olan metodların çalıştırılmasının sonucunda istemciye bu işlemin tamamlandığı bilgisi eğer geri dönüş değeri var ise onunla birlikte döner. Bu, talep/cevap mesajlaşma deseni (request/response messaging pattern) olarak adlandırılan klasik çalışma modelidir. Ancak burada istemcinin çağrıda bulunduğu metodun tamamlanışını beklemesi gerekir. Aksi takdirde ilerlemesi söz konusu değildir. Burada bahsi geçen konu kod satırında bir alt ifadeye geçilememesidir. Çok doğal olarak servis tarafındaki metodun çalışmasının uzun sürdüğü durumlarda istemci uygulama beklemede olacaktır.

Özellikle istemci tarafındaki uygulamaların çağrıda bulundukları metodlar geriye değer döndürmüyorlarsa asenkron programlama (asynchronous programmin) adına One Way tekniğinden yararlanılabilir. One Way tekniği uygulanması kolay olmasına rağmen dikkat edilmesi gereken noktalara sahiptir. İşte bu makalemizde söz konusu noktalara değinerek One Way tekniğinin WCF açısından detaylarını görmeye çalışacağız.

> İstemci programların uzak metod çağrımlarında uygulamaları duraksatmasını engellemek adına kullanılabilecek tek yol One Way değildir. Diğer Asenkron erişim modelleride ele alınabilir. Bunlar ilerleyen makalelerimizde ele alınacaktır. One Way ve diğer asenkron modellerinde istemci ve sunucu uygulamaların aynı zaman diliminde çalışıyor olmaları gerekir. Bunun aksi bir durumda ise Message Queue sisteminin kullanılmasında fayda vardır.

One Way formasyonuna uygun bir çağrım için OperationContract niteliğinin IsOneWay özelliğine true değerini aktarmak yeterlidir. OneWay metodlar ile ilişkili bilinmesi gereken öncelikli kurallar vardır. Herşeyden önce OneWay olarak işaretlenenen metodlar geri dönüş değerine sahip olmazlar. Bir başka deyişle bu metodlar void olarak tanımlanırlar. Burada metoda parametre olarak ref veya out tipinden aktarımlar yapılmak istenebilir ancak buda mümkün değildir. Diğer taraftan söz konusu metodlar içerisinde çok doğal olarak istisnalar (Exceptions) olabilir. Burada söz konusu olan SOAP hata mesajları (SOAP Fault Message) istemci tarafına gönderilmez. Bu sebeplerden ötürü istemci servis üzerindeki metodun başarılı bir şekilde tamamlanıp tamamlanmadığını asla bilemez.

Ancak en azından istemcinin gönderdiği metod çağrısı mesajının servise ulaşıp ulaşmadığının bilinmesinde yarar vardır. Eğer servis uygulaması çalışmıyorsa çok doğal olarak istemci bir istisna alacaktır ki bu durumda istemciden gönderilen talebin ulaşmadığı zaten doğrudan anlaşılabilir. Ancak bazı iletişim protokollerinde (örneğin Tcp) gelen mesajlar tampona alınırarak işlenirler. Arka arkaya gelen bu taleplere ait mesajların toplu olarak bir maksimum sayısı vardır. Sonuçta birden fazla istemci söz konusudur ve bunların gönderecekleri sayısız çağrı bulunmaktadır.

Böyle bir durumda eğer servis tarafında kabul edilebilen maksimum talep sayısı aşılırsa gelen yeni talepler var olan yarım kalmışlar tamamlanıncaya kadar beklemeye alınır. Dolayısıyla istemci uygulama her ne kadar OneWay çağrısı yapsada beklemede kalır. Bu durumla başedebilmek için güvenilir oturumlar (Reliable Session) açılmasında fayda vardır. Güvenilir oturumlar sayesinde servis, bir çağrı aldığında gelen mesajı işlemeye başladığına dair istemciye bilgi gönderebilir.

One Way tekniği istemci için önem arz eden SendTimeout sürelerini önemsemez. Normal şartlarda istemciler bir metod çağrısında bulunduklarında varsayılan olarak 1 dakikalık bir timeout süreleri vardır. Eğer bu süre içerisinde servisten bir cevap gelmesse otomatik olarak TimeoutException tipinden bir çalışma zamanı hatası alınır. One way operasyonlarında çok doğal olarak bu tip bir süre kontrolü önemli değildir. Nitekim istemci tarafı çağrıda bulunduğu metod için bir cevap beklememektedir. Ama varsayılan olarak bazı aksilikler olacaktır ki ilerleyen kısımlarda buna değinmeye çalışacağız.

Şimdi örnek bir uygulama üzerinden ilerleyerek One Way tekniğini nasıl ele alabileceğimizi inclemeye çalışacak ve dikkat edilmesi gereken durumları analiz edeceğiz. Bu makalemizdeki örneğimizde, file-based olarak web üzerinde barındırılan bir servis uygulaması yer alacaktır. İstemciyi olayları kolay bir şekilde takip edebilmek adına bir konsol uygulaması olarak tasarlayabiliriz. Burada diğer makalelerimizden farklı olarak wsHttpBinding bağlayıcı tipini ele alacağız. Bu tip aslında BasicHttpBinding bağlayıcı tipine benzerdir ancak güvenilir oturumlara (reliable sessions) sahip, transcation yönetimine izin veren Http ve Https iletişim protokollerinin kullanılabildiği, WS-Adressing işlemlerin yapılabildiği bir ortam sunarak ekstra imkanlar sağlar.

Geliştirilecek servis uygulaması için WCF Service şablonu kullanılabilir. Bu amaçla ilk olarak Visual Studio 2005 ile aşağıdaki gibi bir servis uygulaması açarak işe başlanmalıdır. WCF Service uygulaması file-based olarak host edileceğinden ve istemcilerin tek bir adresten ilgili servise erişmeleri istendiğinden projenin Properties penceresindeki özelliklerden port numarası sabit bir değere ayarlanabilir. Bu işlem için öncelikle Dynamic Ports özelliğine false değer atamak gerekir.

![mk206_1.gif](/assets/images/2007/mk206_1.gif)

Servisimiz için gerekli tiplerimizi ise aşağıdaki gibi geliştirebiliriz. Söz konusu tipler AppCode klasörü içerisinde yer almaktadır.

![mk206_2.gif](/assets/images/2007/mk206_2.gif)

IRaporUretici Arayüzü İçeriği;

```csharp
[ServiceContract(Name = "RaporlamaServisi", Namespace = "http://www.bsenyurt.com/RaporlamaServisi/2007/30/05")]
public interface IRaporUretici
{
    [OperationContract(IsOneWay = true)]
    void RaporUret(string kullaniciAdi);
    [OperationContract(IsOneWay = true)]
    void SatisYapildi(int productId);
}
```

RaporUretici sınıfı içeriği;

```csharp
public class RaporUretici
               : IRaporUretici
{
    #region IRaporUretici Members

    public void RaporUret(string kullaniciAdi)
    {
        Thread.Sleep(65000); // 1 dakika 5 saniye duraksatma
        FileStream fs = new FileStream("C:\\Izleme.txt", FileMode.Append, FileAccess.Write);
        StreamWriter writer = new StreamWriter(fs);
        writer.WriteLine(kullaniciAdi + " İÇİN RAPOR ÜRETME EMRİ VERİLDİ");
        writer.Close();
        fs.Close();
    }

    public void SatisYapildi(int productId)
    {
        Thread.Sleep(65000); // 1 dakika 5 saniye duraksatma
        FileStream fs = new FileStream("C:\\Izleme.txt", FileMode.Append, FileAccess.Write);
        StreamWriter writer = new StreamWriter(fs);
        writer.WriteLine(productId + " ÜRÜNÜ İÇİN SATIŞ YAPILDI");
        writer.Close();
        fs.Close();
    }

    #endregion
}
```

Arayüz tanımlaması içerisinde dikkat edilecek olursa OperationContract niteliğinin IsOneWay özelliğine true değeri atanmıştır. Servis sözleşmesini uygulayan sınıf içerisinde sembolik olarak iki metod yer almaktadır. Özellikle bu metodların içerisinde Thread sınıfının static Sleep metodu yardımıyla uygulama 65 saniye duraksatılmaktadır. Burada 1 dakikalık TimeOut sınırının aşılmasının doğuracağı bazı sonuçlar olacaktır. Bu sonuçlar ilerleyen kısımlarda ele alınacaktır. Servis tarafında yer alan web.config dosyasının içeriği aşağıdaki gibi tasarlanabilir.

web.config içeriği;

```xml
<?xml version="1.0"?>
<configuration>
    <system.serviceModel>
        <services>
            <service behaviorConfiguration="RaporlamaServisiBehavior" name="RaporlamaServisi.RaporUretici">
                <endpoint address="http://localhost:2215/RaporlamaServisi/Service.svc" binding="wsHttpBinding" bindingConfiguration="" name="RaporlamaServisiEndPoint" contract="RaporlamaServisi.IRaporUretici" />
            </service>
        </services>
        <behaviors>
            <serviceBehaviors>
                <behavior name="RaporlamaServisiBehavior">
                    <serviceMetadata httpGetEnabled="true"/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
    </system.serviceModel>
    <system.web>
        <compilation debug="true"/>
    </system.web>
</configuration>
```

Servis web tabanlı olarak host edildiğinden svc uzantılı bir dosyanında var olması gerekmektedir. Bu dosyanın içeriğinde servis sözleşmesini uygulayan sınıfın adı ve fiziki dosya adı aşağıdaki gibi yer alır.

Service.svc;

```text
<% @ServiceHost Language=C# Debug="true" Service="RaporlamaServisi.RaporUretici" CodeBehind="~/App_Code/Service.cs" %>
```

Servis ile ilgili işlemlere geçmeden önce çalıştığından emin olmakta fayda vardır. Servis başarılı bir şekilde hazırlanmışsa aşağıdaki ekran görüntüsünün elde edilmesi gerekir.

![mk206_3.gif](/assets/images/2007/mk206_3.gif)

Artık istemci için gerekli kodlar yazılıp testlere başlanabilir. Öncelikli olarak istemci uygulamalar için gerekli proxy tipinin üretilmesi gerekir. Bunun için komut satırında (Visual Studio 2005 Command Prompt) svcutil.exe aracı kullanılabilir yada Visual Studio 2005 ortamında konsol uygulamasında Add Service Reference seçeneği ile aşağıdaki ekran görüntüsünde olduğu gibi eklenebilir.

![mk206_4.gif](/assets/images/2007/mk206_4.gif)

Bu işlemin ardından istemci uygulamasına ait proje içerisinde gerekli proxy tipi ve App.config konfigurasyon dosyası üretilmiş olacaktır.

![mk206_5.gif](/assets/images/2007/mk206_5.gif)

İstemci uygulamaya ait kodlar aşağıdaki gibi geliştirilebilir.

```csharp
using System;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;
using Istemci.localhost;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RaporlamaServisiClient srv = new RaporlamaServisiClient("RaporlamaServisiEndPoint");
                Console.WriteLine("Rapor üretimini başlat..." + DateTime.Now.ToString());
                srv.RaporUret("Mayk");
                Console.WriteLine("Rapor üretimi başlatıldı..." + DateTime.Now.ToString());
                Console.WriteLine("Ürün satışı yapıldı bilgisini gönder..." + DateTime.Now.ToString());
                srv.SatisYapildi(1001);
                Console.WriteLine("Ürün satışı yapıldı bilgisi gönderildi..." + DateTime.Now.ToString());

                srv.Close();
            }
            catch (Exception hata)
            {
                Console.WriteLine(hata.Message);
            }
        }
    }
}
```

RaporlamaServisiClient nesnesi örneklendikten sonra RaporUret ve SatisYapildi isimli metodlar çağırılmaktadır. Bu metodları işaret eden servis sözleşmesinde OneWay oldukları belirtilmiştir. Buna göre istemci uygulamanın, bu metodlar içerisindeki 65 saniyelik sürelerden etkilenmeden diğer kod satırlarını işletmesi gerekmektedir. Bakalım gerçekten böyle mi olacak?

Testleri yapabilmek için hem sunucu uygulamanın hemde istemci uygulamanın aynı zaman diliminde çalışıyor olmaları gerekmektedir. Dolayısıyla Solution özelliklerinde Multiple Startup Projects seçilmeli ve önce servis uygulaması ardından istemci uygulama çalışacak şekilde ayarlanmalıdır.

![mk206_6.gif](/assets/images/2007/mk206_6.gif)

Bu aşamadan sonra uygulama çalıştırılırsa ilk etapta aşağıdaki gibi bir ekran görüntüsü ile karşılaşılır.

![mk206_7.gif](/assets/images/2007/mk206_7.gif)

Görüldüğü gibi RaporUret çağrısı kolayca aşılmış ancak, SatisYapildi çağrısından sonra istemci uygulama ekranı duraksamıştır. Oysaki ikinci metodda OneWay olarak çağrılmaktadır. Dolayısıyla aynen ilk metod çağrısında olduğu gibi buradada istemci uygulamanın diğer kod satırlarından işlemlerine devam etmesi beklenir. Ne varki böyle olmamıştır. Sonuç olarak bir süre bekledikten sonra aşağıdaki ekranda görülen hata mesajı ile karşılaşılır.

![mk206_8.gif](/assets/images/2007/mk206_8.gif)

Peki sorun nedir? İstemci uygulama neden çalışma zamamında bir istisna alarak sonlanmıştır? Hatta yeterli süre beklenildiğinde sunucu tarafında Izleme.txt dosyasının içeriğine bakıldığında metod içeriklerinin çalıştığıda gözlemlenebilir.

![mk206_9.gif](/assets/images/2007/mk206_9.gif)

Ne varki istemci açısından bazı sorunlar olduğu ortadadır. Problemin nedeni oturumlardır (Sessions). Aynı oturum içerisinde gelen arka arkaya iki çağrı söz konusudur. Bu çağrılar doğal olarak aynı andalık ilkesine (Concurrency) göre değerlendirilir. Dolayısıyla ikinci çağrıdan sonra istemci uygulama, ilk çağrı sonucunun tamamlanmasını beklemek durumunda kalmıştır. Bu sebepten servisin özellikle aynı oturum içerisinden gelecek eş zamanlı metod çağrılarına cevap verebilecek şekilde özelleştirilmesi gerekir. Burada elebetteki Session moddan vazgeçilmesi tercih edilebilir. Örneğin PerCall mode seçilebilir. Ancak oturumların (Sessions) kullanım amaçları göz önüne alındığında bir çözüm yolu bulunmasında fayda vardır. Yapılması gereken servis sözleşmesini uygulayan RaporUretici sınıfına, ServiceBehavior niteliğini uygulamak ve ConcurrencyMode özelliğine Multiple değerini aşağıdaki gibi vermektir.

```csharp
namespace RaporlamaServisi
{
    [ServiceBehavior(ConcurrencyMode= ConcurrencyMode.Multiple)]
    public class RaporUretici
                        : IRaporUretici
    {
```

Uygulamamızı bu haliyle test ettiğimizde ise aşağıdaki ekran görüntüsü ile karşılaşırız.

![mk206_14.gif](/assets/images/2007/mk206_14.gif)

Herşey ilk etapta yolunda gibidir. Nitekim SatisYapildi metod çağrısı sonrasındaki kodlarda çalışmıştır. Ancak uygulama kapanırken yine bir hata mesajı alınmıştır. Bu hata mesajının üretildiği nokta istemci tarafında servis için Close metodunun çağrıldığı yerdir. Bu çağrıya gelindiğinde istemci uygulama uzun bir süre beklemede kalmıştır. Sonrasında ise bir istisna mesajı fırlatarak sonlanmıştır. Bunun sebebi kullanılan WsHttpBinding tipinin özellikleridir. WsHttpBinding oturumları kullanan ve varsayılan olarak mesaj seviyesinde güvenlik (Message-Level Security) kullanan bir bağlayıcı (Binding) tipidir.

İstemci Close metodunu çağırdığında servis tarafında kendisi için açılan oturumu (Session) kapatmak ister. Buna karşılık servis ilgili oturuma ait işlemlerin bitmesini bekler ve tamamlandıklarından emin olduktan sonra bunu istemciye bir mesaj ile bildirir. Örnekte çok doğal olarak servis tarafındaki işlemler 1 dakika sınırını aştığından, söz konusu mesaj istemciye iletilemez. Buda doğal olarak istemci tarafında bir istisna mesajı fırlamasına neden olur. Çözüm olarak güvenlik ile ilgili ayarlar kapatılabilir ki bu önerilen bir yöntem değildir. Diğer bir yöntem ise iletişim seviyesinde güvenlik (transport levet security) kullanmaktır. Ancak bu yöntemde sertifika kullanılması gerekir nitekim Https politikası söz konusudur.

Ancak mesaj seviyesinde yapılabilecek bir seçenek daha vardır. Buda güvenilir bir oturum (Reliable Session) açmaktır. Nitekim bu sayede istemcinin yaptığı çağrılar sonucu servisten bir bilgi beklenmesine gerek kalmayacaktır. Bunun için servis tarafında ve istemci tarafında wsHttpBinding tipi için konfigurasyon ayarları yapılmalı ve ReliableSession için Enabled özelliklerine true değeri atanmalıdır. Aşağıdaki ekran görüntüsünde servis tarafı için ilgili değişikliklerin Microsoft Service Configuration Editor yardımıyla nasıl yapıldığı gösterilmektedir.

![mk206_10.gif](/assets/images/2007/mk206_10.gif)

Bu değişiklikler sonucu servis tarafı için konfigurasyon dosyasının içeriği aşağıdaki gibi olur.

![mk206_11.gif](/assets/images/2007/mk206_11.gif)

Benzer değişikliklerin istemci tarafı içinde yapılması gerekir. Bunun sonucu olarak istemci tarafındaki konfigurasyon dosyasındada aşağıdaki değişiklikler olacaktır.

![mk206_12.gif](/assets/images/2007/mk206_12.gif)

Artık uygulamımızı bu haliyle test edebiliriz. Aşağıda uygulamanın son halinin test sonuçlarına ait ekran görüntüsü yer almaktadır.

![mk206_13.gif](/assets/images/2007/mk206_13.gif)

Sonuç itibariyle basit bir One Way Ticket şarkısından oldukça farklı noktalara geldik. Bu ana kadar gördüklerimizden aşağıdaki sonuçları çıkartabiliriz.

- OneWay tekniği ile Windows Communication Foundation uygulamalarında asenkron çağırımlar gerçekleştirilebilir ve performans artışı sağlanır. Çünkü istemci uygulama, servis tarafındaki metod sonuçlarını beklemeden yoluna devam eder.
- Günvenilir oturumlara (Reliable Sessions) izin verilmesi halinde istemci tarafında Close metodu uygulandığında servis ile olan bağlantı (Connection), servisten cevap beklenmeden sonlandırılabilir.
- Özellikle oturum (Session) kullanıldığı durumlarda servis üzerinde aynı oturuma ait arka arkaya metod çağırımlarında, istemci uygulamanın duraksamasını engellemek adına ConcurrencyMode özelliğinin değeri Multiple olarak set edilebilir.
- ConcurrencyMode özelliğinin Multiple olarak belirlenmesi halinde servisin gerçektende thread-safe olup olmadığına dikkat edilmelidir. Nitekim Multiple değeri ile, servis tarafındaki uygulama Multi-Thread hale gelmektedir.

Bu makalemizde asenkron programlama çeşitlerinden birisi olan One Way metod çağırım tekniğini incelemeye çalıştık. Asenkron programlamaya ilişkin başka örnekleri ilerleyen makalelerimizde incelemeye çalışacağız. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayınız.](/assets/files/2007/OneWayInceleme.zip)