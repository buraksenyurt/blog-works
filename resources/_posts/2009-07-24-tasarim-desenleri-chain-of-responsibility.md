---
layout: post
title: "Tasarım Desenleri - Chain of Responsibility"
date: 2009-07-24 05:15:00 +0300
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - http
---
Dün gece çok garip bir rüya gördüm. Rüyamda denize açılmak için limanda duran tekneme doğru ilerliyordum. Derken kendimi kaptan köşkünde buldum. E tabi rüya bu. Hareket etmek istedim ama bir türlü beceremedim. Sonunda sorunun ne olduğunu bulmaya çalıştım ve yandaki manzaranın bir benzeri ile karşılaştım. Geminin demir halat zinciri (zincirleri) arap saçına dönmüştü. Sabah uyandığımda ilginç bir şekilde rüyayı hatırlayabildiğimi de farkettim.

![blg49_2.jpg](/assets/images/2009/blg49_2.jpg)

Acaba bu bir işaret miydi? Evet sanırım bu davranışsal tasarım desenlerinden olan Chain of Responsibility'yi anlatmam için bir işaretti. İşte maceramız başlıyor.

Davranışsal (Behavioral) kalıplardan olan Chain of Responsibility deseni, ortak bir mesaj veya talebin (Request), birbirlerine zayıf bir şekilde bağlanmış (Loosly Coupled) nesneler arasında gezdirilmesi ve bu zincir içerisinde asıl sorumlu olanı tarafından ele alınması gerektiği vakalarda kullanılmaktadır. DoFactory.com güncel istatistiklerine baktığımızda kullanım oranı %40' lar seviyesinde görünsede, yazılışı son derece basit bir desendir.

Desende mesajı (talebi) işleyecek olan asıl nesne örnekleri hayali bir zincir şeklinde dizilmektedir. İstemci, işlenmesini istediği bilgiyi bu zincirin en başında yer alan nesneye gönderir. Zincir içerisinde yer alan nesne örnekleride söz konusu içeriği asıl işleneceği yere kadar göndererirler. Bir başka deyişle bir akıştan (Flow) söz etmemiz mümkündür. Zincire atılan her mesaj, zincire dahil olan tüm nesneler tarafından ele alınabilir veya bir sonrakine gönderilebilir.

![blg49_1.jpg](/assets/images/2009/blg49_1.jpg)

Araştırma yaptığım pek çok kaynakta akılda kalıcı bir örnek olarak otomatik ürün makinelerine ait jeton slotları verilmektedir. Her tip jeton için bir slot oluşturmak aslında arka arkaya if blokları yazarak, gelen talebin anlaşılmaya çalışılmasına benzetilebilir. Bunun yerine makine üzerinde, her bir jetonu ele alan tek bir slot tasarlanır (Handler). Ürünü satın almak isteyen kişinin attığı jeton, verdiği komuta (Cola, çikolata vs istemek gibi) ve jetonun tipine göre, içeride uygun olan saklama alanına (ConcreteHandler) düşecektir. Sonrasında ise süreç, jetonun uygun olan saklama alanında değerlendirilerek istenilen ürünün teslim edilmesiyle tamamlanacaktır.

Yine gerçek hayat örneklerinden devam edersek; bir satın alma sürecinde, ödeme onayının kim tarafından verileceğinde de bu desen göz önüne alınabilir. Bu senaryoda ödeme talimatını onaylayabilecek olan yetkililer bulunur. Ancak gelen ödeme talebinin tutarına göre ilk yetkili personel, talebi bir üst yetkiliye iletmek zorunda olabilir. Bu durumda yetkililerin bir sorumluluk zincirinin parçası oldukları düşünülebilir. Burada en alt yetkiliye gelen ödeme talebi, gerektiğinde zincirin sonunda yer alan en üst yetkiliye kadar gidebilmelidir. Ayrıca bu yetkililerin her biri, birbirlerine sadece bu ödeme talepleri kapsamında bağlı olarak düşünülebilir. Bir başka deyişle ödeme onayı için her biri kendi sorumluluklarına sahip iken, farklı işlerde birbirlerinden tamamen bağımsızlardır.

Peki ya bizim dünyamızda (yani Matrix'in içerisinde) ne gibi örnekler verebiliriz? Belkide en yakın örnek olay güdümlü programlamada (Event Based Programming) görülür. Bazı senaryolarda bir olayın birden fazla nesne tarafından ele alınması gerektiği durumlar söz konusu olabilir. Bunu daha çok iç içe bileşenler içeren Form'larda veya diğer taşıyıcı (Container) kontrollerde görebiliriz. Nitekim hepsi için ortak sayılabilecek bir takım olaylar mevcuttur ve kullanıcının herhangibirini tetiklemesi halinde, bu kontrol zinciri içerisindeki hangi bileşenin üretilen olayla ilişkili olduğunun tespit edilmesi ve buna göre işlemlerinin yapılması gerekir. Chain of Responsibility deseni bu noktada devreye girerek üretilen olayın asıl sorumlusu olan bileşen tarafından ele alınmasında önemli bir rol oynamaktadır. Örnekler çoğaltılabilir. Söz gelimi, [wikipedia](http://en.wikipedia.org/wiki/Chain-of-responsibility_pattern)da bu tasarım kalıbı ile ilişkili olaraktan, Loglama örneği verilmektedir.

Gelelim desenimizin UML şemasına;

![blg49_uml.gif](/assets/images/2009/blg49_uml.gif)

Şekildende görüleceği üzere son derece basit bir tasarım kalıbı. Dikkat çekici ilk nokta, Handler tipi ile ConcreteHandler'lar arasında aggregation tadında bir ilişki olmasıdır. Aktörlerimiz ise;

Handler: Kendisinden türeyen ConcreteHandler'ların, talebi ele alması için gerekli arayüzü tanımlar. Abstract class veya Interface olarak tasarlanır.

ConcreteHandler: Sorumlu olduğu talebi değerlendirir ve işler. Gerekirse talebi zincir içerisinde arkasından gelen nesneye iletir. Sonraki nesnenin ne olacağı genellikle istemci tarafında belirlenir.

Client: Talebi veya mesajı gönderir.

Artık kendi örneğimizi geliştirmemizin vakti geldi sanırım.

![Wink](/assets/images/2009/smiley-wink.gif)

Örnek senaryomuzda sorumluluk zincirine dahil edeceğimiz bir servis bilgisi olacak. Servis bilgisini basit bir sınıf olarak tasarlayacağız. Servisin en önemli noktası lokasyon özelliğidir (Location). Servisin yerel makineden, bilgisayarın içinde bulunduğu bir network'ten veya internet üzerinden erişilebilir bir yerde olup olmama durumuna göre zincir içerisindeki sorumlu nesne tarafından ele alınmasını sağlamaya çalışacağız. İşte örnek kodlarımız ve sınıf çizelgemiz.

![blg49_3.gif](/assets/images/2009/blg49_3.gif)

```csharp
using System;

namespace ChainOfResponsibilityPattern
{
    // Yardımcı enum sabiti
    enum ServiceLocation
    {
        LocalMachine,
        Intranet,
        Internet,
        SecureZone,
    }

    // Zincir içerisindeki nesnelerde dolaşabilecek olan tip
    class ServiceInfo
    {
        public string Name { get; set; }
        public ServiceLocation Location { get; set; }
    }

    // Handler
    abstract class ServiceHandler
    {
        protected ServiceHandler _successor;
        public ServiceHandler Successor
        {
            set
            {
                _successor = value;
            }
        }

        public abstract void ProcessRequest(ServiceInfo sInfo);
    }

    // ConcreteHandler
    // Servisin Internet üzerinde olduğu durumu ele alır.
    // Sorumluluk zincirinin son sırasındaki tip
    class InternetHandler
        : ServiceHandler
    {
        public override void ProcessRequest(ServiceInfo sInfo)
        {
            // Eğer lokasyon Internet ise bu tipe ait nesnenin sorumluluğundadır Eğer Internet' de değilse artık sernin son halkası olduğundan gidecek başka bir yer kalmamıştır. Buna uygun şekilde bir hareket yapılmalıdır.
            if(sInfo.Location== ServiceLocation.Internet)
                Console.WriteLine("Web ortamı üzerinde yer alan bir servis.\n\t{0} için gerekli başlatma işlemleri yapılıyor.", sInfo.Name);
            else
                Console.WriteLine("Uzaydan gelen bir servis mi bu yauv?");
        }
    }

    // ConcreteHandler
    // Servisin Intranet üzerinde olduğu durumu ele alır.
    class IntranetHandler
        : ServiceHandler
    {
        public override void ProcessRequest(ServiceInfo sInfo)
        {
            // Eğer servis yerel makinede değilse zincirin bir sonraki tipi olan IntranetHandler' a gelir. Burada servis lokasyonunun Intranet olup olmadığına bakılır. Eğer öyleyse sorumluluk buradadır ve yerine getirilir.Ama değilse, zincirde bir sonraki tip olan InternetHandler nesne örneğine ait ProcessRequest metodu çağırılır.
            if(sInfo.Location== ServiceLocation.Intranet)
                Console.WriteLine("Şirket Network' ü üzerinde yer alan bir servis.\n\t{0} için gerekli başlatma işlemleri yapılıyor.", sInfo.Name);
            else if(_successor!=null)
                _successor.ProcessRequest(sInfo);
        }
    }

    // ConcreteHandler
    // Servisin yerel makineye ait olma durumunu ele alır.
    class LocalMachineHandler
        : ServiceHandler
    {
        public override void ProcessRequest(ServiceInfo sInfo)
        {
            // Eğer servis yerel makinede ise sorumluluk LocalMachineHandler nesne örneğine aittir. Ancak değilse, zincirde bir sonraki tip olan IntranetHandler' a ait ProcessRequest metodu çağırılır.
            if(sInfo.Location== ServiceLocation.LocalMachine)
                Console.WriteLine("Yerel makinede yer alan bir servis.\n\t{0} için gerekli başlatma işlemleri yapılıyor.", sInfo.Name);
            else if (_successor != null)
                _successor.ProcessRequest(sInfo);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Önce zincire dahil olacak nesne örnekleri oluşturulur
            ServiceHandler handlerLocal = new LocalMachineHandler();
            ServiceHandler handlerIntranet = new IntranetHandler();
                ServiceHandler handlerInternet = new InternetHandler();

            // Zincirde yer alan her bir nesne kendisinden sonra gelecek olan nesneyi belirler. 
            // Bu belirleme işlemi için Successor özelliği kullanılır.
            handlerLocal.Successor = handlerIntranet;
            handlerIntranet.Successor = handlerInternet;

            // Zincir halkasındaki nesneler tarafından kullanılacak olan nesne örneği oluşturulur.
            ServiceInfo info = new ServiceInfo { Name = "Order Process Service", Location = ServiceLocation.Intranet };

            // Zincirin ilk halkasındaki nesneye, talep gönderilir.
            handlerLocal.ProcessRequest(info);

            // Servisi kırdığımız nokta. Minik bir bomba ve antrenman sorusu.
            // handlerInternet.ProcessRequest(info);
        }
    }
}
```

Örneği çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![blg49_4.gif](/assets/images/2009/blg49_4.gif)

Servis lokasyonu intranet olduğundan, zincirin ilk halkasındaki handlerLocal isimli nesne örneği, sorumluluğu bir sonraki nesneye atmıştır. Bu nedenle IntranetHandler tipi içerisinde ProcessRequest metodu çalışmıştır ve sonraki adımda yer alan InternetHandler tipine bir geçiş söz konusu olmamıştır. info değişkenine ait Location özelliğinin değerini değiştirerek farklı sonuçları değerlendirebilirsiniz. Tabi mutlaka dikkatinizi çekmiştir, Location özelliğinin işaret ettiği ServiceLocation enum tipi içerisinde, zincir üzerinde ele alınmayan sabit bir değerde vardır. SecureZone. Dın dın dın dııınnnnn ![Sealed](/assets/images/2009/smiley-sealed.gif) Sizce neden panik oldum acaba. Bunu bir düşünün.

Örnekte görüldüğü üzere, ServiceInfo tipinden bir nesne örneğinin Location özelliğinin değerine göre bir akış gerçekleştirilmektedir. Bu akışa ait zincir halkasının ilk nesnesi LocalMachineHandler iken son nesneside InternetHandler tipine aittir. Zincirdeki tüm tipler, ServiceHandler isimli abstract sınıftan türemektedir. Bu abstract sınıf, kendi tipinden bir özelliğe sahiptir. Successor isimli bu özellik ile amaç, halkadaki bir nesnenin kendisinden sonra gelecek olanı işaret etmesini sağlamaktır. Zincirdeki her nesnenin (Sonuncu hariç) bir Successor'u olmalıdır. Doğal olarak ilerleyen zamanlarda zincire başka bir nesnenin eklenmesi söz konusu olabilir. Bu nedenle, Successor özelliğinin aslında tüm ConcreteHandler'ların türediği ata tipi (Handler) kullanması son derece mantıklıdır.

İstemci tarafındaki kod içinde de dikkat edilmesi gereken bir takım hususlar vardır. Zincir içerisindeki her bir nesne örneklendikten sonra, sıraya göre birbirlerine Successor özellikleri üzerinden bağlanırlar. Bu doğal olarak zincirin doğru biçimde sıralanmasını gerektirir. Aksi durumda iş mantığına uygun olmayan sonuçlar alabiliriz. Öyleki asıl gidilmesi gereken yer yerine farklı bir yere gidilebilir.(Ödemenin onayını Genel Müdürün vermesi gerekirken, zincirdeki hatalı atama sonrası gişe memurunun trilyonlar için yetki vermesi gibi ![Undecided](/assets/images/2009/smiley-undecided.gif)) Yada zinciri kıracak şekilde bir çağrıda gelebilir. Örneğin kodun son kısmında minik bir bomba yer almaktadır. Buradaki sorunun ne olabileceğini bulmak ve bir yorum yapmak sizin göreviniz.

![Wink](/assets/images/2009/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ChainOfResponsibilityPattern.rar (25,32 kb)](/assets/files/2009/ChainOfResponsibilityPattern.rar)
