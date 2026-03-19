---
title: "Tasarım Desenleri - State"
pubDate: 2009-08-06 06:00:00
categories:
  - Tasarım Kalıpları(Design Patterns)
tags:
  - Tasarım Kalıpları(Design Patterns)
  - C#
  - LINQ
  - Threading
  - Generics
---

# Tasarım Desenleri - State
![blg57_vendingMachine.gif](images/blg57_vendingMachine.gif)

Merhaba Arkadaşlar,

Bir süre öncesine kadar özel bir bankada uzman yazılım geliştirici olarak görev almaktaydım. Bankada en çok hoşuma giden bazende en çok nefret ettiğim hususlardan biriside otomat makinesi idi.

![Smile](images/smiley-smile.gif)

Makineyi severdim çünkü fazla mesai yapıldığı hallerde içerisinde son derece işe yarar tuzlu ve tatlı gıdalar olurdu. Makinenin başına geçer, yemek istediğim ürüne bakar karar verdikten sonra ise gerekli miktarı makineye atardım.

Sonra almak istediğim ürünün kodunu tuşlardım. Makine, ürünü benim için ilgili yerden aşağıya doğru ittirerek sunardı. Sonrada ürünü afiyetle yerdim. Ama makinenin şu huyunada çok kızardım. Para üstü vermezdi

![Undecided](images/smiley-undecided.gif)

Eksiği bazı ürünler ile tamamlardım yada tamamlayamazdım. Çünkü eksik kalan kısma verilebilecek bir ürün olmazdı. Yine mesai yaptığım akşamların birisinde makineye gittim, ürüne karar verdim, paraları attım ve makine bip, bap, bup dedikten sonra öylece kala kaldım.

Çünkü makine sözüm ona ürünü vermişti. Ancak makinenin alt sepetinde ürün yoktu. Nitekim ürün tam bulunduğu cepten aşağıya doğru düşmek üzereyken oracıkta takılıvermişti. Para gitmişti, nitekim makinin dijital kısmında Teşekkürler yazıyordu

![Sealed](images/smiley-sealed.gif)

Ben olaya klasik bir insan piskolojisi ile yaklaştım. Makineyi öne arkaya itekleyerek ürünü takıldığı yerden düşürttüm ve afiyetle yedim. Makineye pis pis bakarken aklıma şunlar geldi.

Makineye yaklaşırken durağandı. Öylece birbirimize bakıyorduk. Sonra paramı attıp ürünü seçtiğimde makine bir dizi kontrol yaptı ve hazırlık moduna geçti. Ardından ürünü bana teslim etmek üzere kendi içerisindeki mekanikleri çalıştırdığında ürünü teslim etme modundaydı. Peki ürün takılıp bana veremediğinde hangi moddaydı da "Teşekkürler" diyip, paramı yutup, ürünü vermemişti

![Laughing](images/smiley-laughing.gif)

Her neyse konumuz bu değil tabiki. Ama bu makinenin bu senaryo içerisinde anlattığım tüm durumları aslında yazılım terminolojisinde State Machine tipinden bir akış ile ifade edilebilmektedir. İşte bu günkü konumuz State tasarım kalıbı...

Davranışsal (Behavioral) tasarım desenlerinden olan State kalıbı, bir nesnenin içsel durumunda (Internal State) meydana gelecek değişimler sonrası çalışma zamanında dinamik olarak farklı davranışları sergileyebilmesini sağlayan bir model sunmaktadır. Aslında State tasarım kalıbını, Workflow terminolojisinde yer alan State Machine kavramının nesne yönelimli (Object Oriented) karşılığı olarak düşünebiliriz. Öyleki nesnenin durumunun değişmesi halinde farklı davranışlar sergilemesi, sahip olduğu fonksiyonların tetiklenmesi ve bunlar arasında duruma göre gerekli geçişlerin (Transitions) sağlanması anlamına da gelmektedir. Dolayısıyla, State Machine kavramına aşina olanlarımız için State desenini kavramak son derece kolaydır.

Farklı bir örnek ile devam edelim. Bu amaçla bir müşterinin sahip olduğu banka hesabının durumlarını göz önüne alabiliriz. Bakiyenin içeriği müşterinin para yatırmasına, çekmesine, faiz ödemesine, fon alıp satmasına vb... gibi aksiyonlara göre sürekli değişiklik gösterecektir. Bir başka deyişle hesabın kendi iç durumunda bir takım değişiklikler olması söz konusudur.

Bu değişikliker oldukça hesabın farklı durumları olması (bir başka deyişle müşterinin farklı şekillerde değerlendirilmesi) gerekir. Örneğin fazla borçlanma nedeniyle farklı bir hesap durumu olmalıdır. Yada hesabın ilk açılmasında başlangıç durumu tesis edilmeli, standart faiz oranları belirlenmelidir (Aynen otomat makinesinin prize takıldığında ön hazırlıklar yaptığı sıradaki konumu gibi). Hatta müşterinin düzenli ödemelerinin ona ekstradan bir anlam katması sonucu, hesabının kolay kredi almaya uygun bir duruma geçmesi mümkün olabilir.

Yazılım tarafından olaya baktığımızda aslında State diagramları ile ifade edilebilen her nesne için State deseninin uygulanabileceğini düşünebiliriz. Örneğin uygulamanın çalıştığı makinenin bellek durumları State kalıbına uygun olarak tasarlanabilir. Makinin normal seviyede olması, sistem kaynaklarının çok tüketilmesi sonucu alarm haline geçmesi veya alarm verilmeden önce uyarı moduna geçmesi söz konusu olabilir. Bu durumlar arasındaki geçişler aslında bilgisayarın bazı iç değerlerine göre gerçeklenir. Memory, CPU, Running Process ölçümleri birer kriter olabilir ve örneğin Computer isimli bir nesnenin iç durumunu ifade edebilir.

Başka bir örnek olarak oyun programlarında yer alan bazı senaryoları verebiliriz. Söz gelimi RPG tipinden bir oyunda yer alan herhangibi kahramanı düşünelim. Bu kahramanın duruma göre savaşması veya bir takım kontrollerde bulunması gibi davranışları, State deseninden yararlanılarak modellenebilir. Öyleki, savaş halinde iken kahramanın tüm gücüyle çarpışması, aynı zamanda devriyede olması söz konusu iken, barış halinde savaşmaması ama devriyeye devam etmesi durumları söz konusudur. Bu durumların nesne yönelimli tarafta ifadesinde State kalıbından yararlanılır.

Aslında tüm bu örneklerde dikkat edilmesi gereken ortak bir notkada vardır. State tasarım kalıbında, durum değişmelerine neden olacak (yani davranışların farklılaşmasına) bir takım nesne içi değerler vardır. Bunların tamamı aslında davranış değişimi için takip edilecek içeriği oluşturmaktadır. Müşteri hesabı örneğinde Hesap (Account) asıl içeriği oluşturmaktadır. Bilgisayarın durumlarının ele alındığı örnekte makinenin kendisi asıl içeriği oluşturmaktadır. Hımmm... Bu durumda ortaya şöyle bir soru çıkmaktadır. İçeriğindeki veri değişimleri eğer bir nesnenin davranışlarını belirliyorsa, bu davranışların n sayıda olması ve içeriği sağlayan tip tarafından kullanılması nasıl sağlanabilir?

Bundan sonra internal state'i taşıyan nesneye Context dediğimizi düşünelim. Birden fazla davranış ve doğal olarak durum olabileceğinden, Context'in farklı durumlara erişebilip aralardaki geçişleri (Transitions) sağlayabilmesi gerekir. Bu durumda, Context tipinin tüm durumlar için ortak bir arayüz sunan başka bir tip ile (buna State diyebiliriz) Aggregation ilişkisini sağlaması uygundur. State tipinin kendisi aslında, Context tipinin belli bir durumu ile ilişkilendirilmiş davranışların kapsüllenmesi için bir arayüz sunmaktadır. Bu arayüz sunumu aslı durum tipleri (Concrete State) tarafından değerlendirilebilir. Aslında bu yazdıklarımızdan deseninin sınıf diagramını az çok hayal edebiliriz.

![blg57_uml.gif](images/blg57_uml.gif)

E haydi öyleyse basit bir örnek ile kalıbı kavramaya çalışalım. Senaryomuzda yazımızın başında bol bol kulakları çınlayan otomat makinesini ele alıyor olacağız.

![Laughing](images/smiley-laughing.gif)

Tabiki amacımız kalıbın nasıl uygulandığını ele almak olduğundan mümkün olduğunca sade (her zamanki gibi) bir örnek geliştireceğiz. Otomat makinesi için olası durumları şu şekilde düşünebiliriz. Makine elektrik şalterinden açıldığında bazı ön hazırlıklar yapar. Bu zaman diliminde makine Initialize modundadır (InitializeState). Initialize işlemleri başarılı ise makine bekleme moduna geçer (WaitingState). Ne bekler? Tabiki bizden bir ürün almamızı

![Wink](images/smiley-wink.gif)

Müşteri bir ürün talep ettiğinde bunu almak için makineye para atması ve sonrasında seçimi bildirmesi gerekir. Bu işlemi Context tipimiz içerisindeki bir metodun üstlendiğini düşünebiliriz. Eğer atılan para yeterli ise ürünün hazırlanması moduna geçilir (PreparingState) ve işlem başarılı bir şekilde tamamlanırsa ürün teslim edilir (DeliveryState). Kısaca Context tipi olarak düşündüğümüz VendingMachine sınıfı için dört farklı durum (State) düşünüyoruz. İşte sınıf diagramımız;

![blg57_classDiagram.gif](images/blg57_classDiagram.gif)

ve kodlarımız

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace StatePattern
{
    // State tipi
    // abstract sınıf olabileceği gibi interface şeklinde de tasarlanabilir
    abstract class VendingMachineState
    {
        public abstract void HandleState(VendingMachine context);
    }

    // Concrete State tipi
    // Otomat start düğmesine basılarak çalıştırıldığında öncelikli olarak bir ön hazırlık yapacaktır.
    class InitializeState
        :VendingMachineState
    {
        public InitializeState()
        {
            Console.WriteLine("Initialize...");
        }
        public override void HandleState(VendingMachine context)
        {
            Console.WriteLine("Ön hazırlıklar yapılıyor");
            Thread.Sleep(2000);            
            // Makinenin durumu değiştiriliyor. Makine initialize edilmiş. Bekleme konumuna geçebilir.
            context.State=new WaitingState();     
        }
    }

    // Concrete State tipi
    class PreparingState : VendingMachineState
    {
        public PreparingState()
        {
            Console.WriteLine("Preparing...");
        }
        public override void HandleState(VendingMachine context)
        {
            Console.WriteLine("İstenilen ürün hazırlanıyor. Lütfen bekleyiniz");
            // Makienin durumu değiştiriliyor. Ürün hazırlanması bitmiş. Buna göre ürünü teslim etme durumuna geçiyor.
            context.State = new DeliveryState();
        }
    }

    // Concrete State tipi
    class WaitingState 
        : VendingMachineState
    {
        public WaitingState()
        {
            Console.WriteLine("Waiting...");
        }        
        public override void HandleState(VendingMachine context)
        {
            int totalProduct=context.ProductList.Sum<Product>(p => p.Count);

            Console.WriteLine("Makine bekleme konumunda. Şu anda {0} adet ürün var.",totalProduct.ToString());
            // Makine bekleme konumundayken  aslında bir State değişikliği söz konusu değil. Değişimi sağlayacak olan aslında istemcinin vereceği bir aksiyon. Context tipi üzerindeki RequestProduct metodunun çağırılması bu anlamda düşünülebilir.
        }
    }

    // Concrete State tipi
    class DeliveryState 
          : VendingMachineState
    {
        public DeliveryState()
        {
            Console.WriteLine("Delivering...");
        }
        public override void HandleState(VendingMachine context)
        {
            Console.WriteLine("Ürün teslim ediliyor");
            // Makinin durumu değiştiriliyor. Ürün teslim edildikten sonra tekrar bekleme konumuna alınıyor.
            context.State = new WaitingState();
        }
    }

    // Context tipi
    class VendingMachine
    {
        public List<Product> ProductList = new List<Product>();
        // Context tipi, kendi içerisinde State nesne referanslarını değiştirebilir. Bunun için State tipinden bir özellik sunmaktadır
        private VendingMachineState _state;

        public VendingMachineState State
        {
            get { return _state; }
            set
            {
                // State değiştiğinde, üretilen State nesne örneğinin çalışma zamanındaki referansına ait HandleState metodu çalıştırılır. Parametre olarak o anki Context gönderilir.
                _state = value;
                // Burada durum değişimleri sonucu çalıştırılacak davranışların başlatılma noktasınıda merkezileştirmiş oluyoruz.
                _state.HandleState(this);
            }
        }       

        // Context nesnesi örneklenirken başlangıç durumu belirtilir.
        public VendingMachine()
        {
            // Test için makineye örnek ürünler yüklenir.
            ProductList.Add(new Product { Name = "Çikolata K", ListPrice = 10,Count=50 });
            ProductList.Add(new Product { Name = "Biskuvi Bis", ListPrice = 3.45 ,Count=50});
            ProductList.Add(new Product { Name = "Tuzlu mu tuzlu çıtır", ListPrice = 4.50 ,Count=35});

            // Makineye ürünleri yükledikten sonra durumunu değiştir
            State = new InitializeState();
        }
        public void RequestProduct(string productName,double money)
        {
            Console.WriteLine("Ürün siparişi geldi. {0} için atılan para : {1}",productName,money);
            Product prd = (from p in ProductList
                           where (p.Name == productName && (money >= p.ListPrice && p.Count >= 1))
                           select p).SingleOrDefault<Product>();
                        
            // Eğer talep edilen ürün stokta var ve atılan para yeterli ise 
            if (prd != null)
            {
                prd.Count--;
                // Makinenin durumunu değiştir
                State = new PreparingState();
            }
            else
                State = new WaitingState();
        }
    }

    // Yardımcı tip
    class Product
    {
        public string Name { get; set; }
        public double ListPrice { get; set; }
        public int Count { get; set; }
    }

    // Client
    class Program
    {
        static void Main(string[] args)
        {
            // Context tipine ait nesne örneği oluşturulur
            VendingMachine machine = new VendingMachine();

            // İstemci bir ürün ister
            machine.RequestProduct("Çikolata K",10);
            
            machine.RequestProduct("Bsissi", 12); // Bu ürün olmadığı için vermeyecektir. Herhangibir aksiyon alınmayacaktır.
        }
    }
}
```

Örneğimizde, Client yani müşteri makineyi çalıştırarak işe başlıyor. Bir başka deyişle VendingMachineI (Context) tipinden bir nesne örneği oluşturuluyor. Bu nesne ayağa kalkarken içerisindeki bir listeye 3 farklı üründen değişik miktarlarda aktarıyor. Bu noktada VendingMachine nesnesinin durumlarında da değişmeler oluyor. Sonrasında, machine isimli nesne örneği üzerinden RequestProduct metodu çağırılıyor. Yani müşteri makineden bir ürün istiyor. Bu sırada, yine makinenin durumları arasında bazı geçişler oluyor. Özet olarak makinenin iç durumunda yapılan bazı değişikliklere göre farklı durumlara geçmesi ve farklı davranışların sergilenmesi sağlanıyor.

Ben geliştirdiğimiz örnekte pek çok durumuda göz ardı ettim.

![Embarassed](images/smiley-embarassed.gif)

Örneğin makinede talep edilen ürünün olmaması, atılan paranın yetersiz kalması veya fazla gelmesi yada makinin fişten çekilmesi hali...Bu olaylar gerçkeştiğindede aslında makinenin farklı durumlara geçmesi ve dolayısıyla Context tipinin farklı davranışlar sergilemesi gerekebilir. Bu kısımları, bir desen uyguladığımız için sisteme eklememiz aslında son derece basittir. Örneğimizi çalıştırdığımızda aşağıdakine benzer bir sonuç ile karşılaştığımızı görebiliriz.

![blg57_runing.gif](images/blg57_runing.gif)

Tabiki yukarıdaki gibi bir deseni uygulamak yerine her şeyi if veya switch gibi kontrol deyimleri ile ele almaya çalışabiliriz. Tabi bu durumda hem kodun karmaşıklaşmasına neden olur hemde genişletilebilirliğini zorlaştırmış oluruz. Nitekim şu anda uygulanan desene göre, makine için yeni bir davranış eklemek aslında State arayüzünden türüyen bir tip ekleyip bunu ilgili yerlerde değerlendirmekten başka bir işlem değildir. Bunu daha iyi anlamak için aynı örneği if ve switch yapıları ile geliştirmeye çalışmalısınız. Böylece geldik bir tasarım deseninin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[StatePattern.rar (26,49 kb)](assets/StatePattern.rar)
