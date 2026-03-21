---
layout: post
title: ".Net 4.0 - Lazy Initialization [Beta 2]"
date: 2009-12-14 01:30:00 +0300
categories:
  - csharp-4-0
tags:
  - csharp
  - .net-framework
---
Eminim hepimiz arada sırada tembellik yapıyor ve ilk bulduğumuz rahat köşeye kıvrılıp hiç bir şeyi düşünmeden rahatça uyuyabiliyoruz. Eğer bulunduğumuz yer uyumaya çok müsait değilse yandaki kedi gibi ortama ayak uydurup yinede uyuyoruz

![blg96_Giris.jpg](/assets/images/2009/blg96_Giris.jpg)

![Smile](/assets/images/2009/smiley-smile.gif)

Siz hiç gözleri açık uyuyabilen insanlar gördünüz mü? Bunun adı düpe düze tembellik olabiliyor bazen.

İhtiyaç dışında uyumak ve hiç bir şeyle uğraşmadan öylece kala kalmak tembelliğin doruk noktaya ulaştığı anlar olarak düşünülebilir. Ama gelin görün ki, programatik ortamda da nesnelerin zaman zaman tembellik etmesi gerekmektedir. Bugünkü yazımızda bu konuyu değerlendiriyor olacağız. Peki neymiş şu Lazy Initialization bir bakalım.

Lazy Initialization yetenekleri sayesinde programların gereksiz bellek tüketimlerinin önüne geçilebilir ve ayrıca performans kazanımı sağlanabilir. Aslında uygulamalarımızda Lazy Initialization kullanmamız için gerek ve yeter iki sebep bulunmaktadır. İlk olarak üretilme maliyetleri yüksek olan nesnelerin tanımlandıkları anda oluşturulmaları yerine, kullanılmaya başladıkları yerde oluşturulmaları gerektiği durumlarda tercih edilebilir. Bu noktada aklımıza LINQ içerisinde zaten var olan Deferred Execution özelliği gelmektedir ki Lazy Initialization yeteneklerine sahip.Net CLR tipleride tam olarak bunu sağlamak üzere tasarlanmıştır. İkinci bir sebep olarak, maliyeti yüksek olan bazı hesaplamaların tamamlanmasından sonra ilgili nesnelerin oluşturulması istenebilir. Söz gelimi uygulamamın çalıştırılması ile üretilen n sayıda nesneden sadece gerekli olanların üretilmesi istendiği durumlarda Lazy Initialization yeteneklerinden yararlanılabilir.

Çok doğal olarak nesnelerin ihtiyaç duyuldukları anda oluşturulmasının sağlanması için kendi tekniklerimizi de geliştirebiliriz. Bu noktada Reflection'ın büyük önemi olduğunu vurgulamak gerekir. Nitekim nesnelerin oluşturulmasI işlemini üstlenecek bir Wrapper'ın mutlaka tasarlanmış olması ve hatta içerisinde Instance üretimi için gerekli kodlamaların yapılması gerekmektedir. Activator sınıfının çalışma zamanında nesne üretimi ile ilişkili çeşitli static metodları bu amaçla kullanılabilir. Ancak.Net Framework 4.0 ile birlikte Lazy Initialization için kullanılabilecek Lazy sarmalayıcı sınıfı gelmektedir. Bu tip sayesinden nesnelerin gerek duyulduğu zamanlarda oluşturulmasının sağlanması son derece kolaylaştırılmaktadır. Üstelik Lazy Thread Safe bir tip olarak kullanılabilmektedir.

Dilerseniz konuyu biraz daha net kavramak adına basit bir örnek geliştirelim. Örnek senaryomuza göre program içerisinde ele alınan bir oyun sahnesi ve bu sahne içerisinde yer alan grafiksel şekillerin tutulduğu çeşitli nesnelerin üretim işleminde Lazy Initialization tekniklerinin nasıl kullanıldığını incelemeye çalışacağız. Bu senaryoda Lazy Initialization kullanmamız için öne sürdüğümüz sebep ise şu olacak; programın ilerleyen adımlarında pek çok farklı sahne ve bu sahne içerisinde yer alan grafik nesnelerin yeri geldiğinde oluşturulması ve böylece programın başlangıcında n sayıda sahne için yapılacak olan oluşturulma maliyetinin azaltılması. Bu amaçla ilk olarak Visual Studio 2010 Ultimate Beta 2 üzerinde aşağıdaki tiplere sahip bir Console uygulaması geliştirdiğimizi düşünelim.

![blg96_ClassDiagram.gif](/assets/images/2009/blg96_ClassDiagram.gif)

```csharp
using System.Collections.Generic;

namespace BeLazy
{
    enum ActorType
    {
        Player,
        Computer,
        Wall,
        Enemy
    }
    class Actor
    {
        public int ActorId { get; set; }
        public string Title { get; set; }
        public string Capability { get; set; }
        public ActorType ActorType { get; set; }
        public byte[] Image { get; set; }

        public override string ToString()
        {
            return string.Format("Actor Id {0} Title {1} Capability {2} Actor Type {3}", ActorId.ToString(), Title, Capability,ActorType.ToString());
        }
    }
    class Scene
    {
        public int SceneId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] Background { get; set; }
        public List<Actor> Actors { get; set; }

        public override string ToString()
        {
            return string.Format("SceneId {0} Widht {1} Height {2}", SceneId.ToString(), Width.ToString(), Height.ToString());
        }
    }
}
```

Tamamen hayali olarak tasarlanmış olan bu tiplerde ana fikir Scene tipi içerisinde bir Actor listesinin olmasıdır. Her iki tip içerisinde byte[] tipinden özellikler yer almaktadır. Scene nesnelerinin sayısının çok fazla olduğu bir durumda, tümünün bir seferde oluşturulması bellek üzerindeki tüketimi arttırabileceği gibi program performansınıda olumsuz yönde etkileyebilir. Bu nedenle Lazy Initialization tekniklerinden faydalanarak sadece gereksinim duyulduğu yerde oluşturulma işlemi yolu tercih edilebilir.

Peki oluşturulma işlemi hangi aşamada gerçekleşmektedir? Bu noktada Lazy tipinin Value özelliği devreye girer. Value özelliğinin kullanıldığı ilk yerde T tipinin oluşturulma işlemi başlamaktadır. Şimdi bu durumu analiz etmek amacıyla Program kodunun içeriğini aşağıdaki gibi yazdığımızı düşünelim.

```csharp
using System;

namespace BeLazy
{
    class Program
    {
        static void Main(string[] args)
        {
            Lazy<Scene> lazyScene = new Lazy<Scene>();

            Console.WriteLine("Scene oluşturuldu mu? {0} ",lazyScene.IsValueCreated);
            lazyScene.Value.SceneId = 10;
            Console.WriteLine("Scene oluşturuldu mu? {0} ", lazyScene.IsValueCreated);
            Console.WriteLine("Scene ID : {0}",lazyScene.Value.SceneId.ToString());
        }
    }
}
```

İlk olarak Lazy tipinden bir nesne örneği oluşturulmaktadır. Sonrasında IsValueCreated özelliği yardımıyla Scene nesnesinin oluşturulup oluşturulmadığına bakılır. Bir sonraki satırda dikkat edileceği üzere Value özelliği üzerinden gidilerek SceneId için değer ataması yapılmıştır. Uygulamanın çalışma zamanı çıktısı aşağıdaki gibidir.

![blg96_FirstRun.gif](/assets/images/2009/blg96_FirstRun.gif)

Volaaaa!!!

![Cool](/assets/images/2009/smiley-cool.gif)

Görüldüğü üzere Value özelliği çağırılana kadar IsValueCreated özelliği false değer döndürmektedir. Bir başka deyişle söz konusu nesne henüz oluşturulmamıştır. Ancak Value özelliğinin kullanılması ile birlikte bir Scene nesnesinin örneklendiği görülmektedir. Burada dikkat edilmesi gereken noktalardan biriside Value özelliğinin Readonly olmasıdır. Bir başka deyişle Value özelliği ile elde edilen bir nesne referansına yeni bir atama gerçekleştirilemez.

![blg96_ReadOnlyError.gif](/assets/images/2009/blg96_ReadOnlyError.gif)

Ama tabiki Value üzerinden gidilen tipin özellikleri değiştirilebilir. Örneğimizde dikkat çekici noktalardan bir diğeri de Scene nesnesinin aslında karmaşık bir tip olarak içerisinde başka tipleride barındırıyor olmasıdır. Buna göre Value özelliğinden yararlanılarak diğer tiplerin değerlerinin de atanması sağlanabilir...Ya da...Ya da aşağıdaki kod parçasında olduğu gibi tembel bir üretim gerçekleştirilebilir.

```csharp
using System;
using System.Collections.Generic;

namespace BeLazy
{
    class Program
    {
        static void Main(string[] args)
        {
            Lazy<Actor> azman = new Lazy<Actor>(() =>
                new Actor {
                     ActorId=1,
                      ActorType= ActorType.Computer,
                       Capability="Çoook!",
                        Image=new byte[1000],
                         Title="Azman"
                }
                );

            Lazy<Actor> gazman = new Lazy<Actor>(() =>
                new Actor
                {
                    ActorId = 1,
                    ActorType = ActorType.Enemy,
                    Capability = "Yüksek Gaz depolama kapasitesi!",
                    Image = new byte[5000],
                    Title = "Gazman"
                }
                );

            Lazy<Scene> scene = new Lazy<Scene>(() =>
                new Scene
                {
                    SceneId = 1001,
                    Background = new byte[10000],
                    Height = 100,
                    Width = 250,
                    Actors = new List<Actor> { azman.Value, gazman.Value }
                }
                );

            Console.WriteLine("Scene oluşturuldu mu? {0} ",scene.IsValueCreated);
            Console.WriteLine("Azman oluşturuldu mu= {0}",azman.IsValueCreated);
            Console.WriteLine("Gazman oluşturuldu mu= {0}", gazman.IsValueCreated);
            
            Scene currentScene=scene.Value;

            Console.WriteLine("Scene oluşturuldu mu? {0} ", scene.IsValueCreated);
            Console.WriteLine("Azman oluşturuldu mu= {0}", azman.IsValueCreated);
            Console.WriteLine("Gazman oluşturuldu mu= {0}", gazman.IsValueCreated);

            Console.WriteLine(currentScene.ToString());

            foreach (Actor actor in currentScene.Actors)
            {
                Console.WriteLine("\t {0}",actor.ToString());
            }
        }
    }
}
```

Buna göre çalışma zamanı içeriği aşağıdaki gibi olacaktır.

![blg96_SecondRun2.gif](/assets/images/2009/blg96_SecondRun2.gif)

Görüldüğü üzere Scene ve Actors özelliğinin işaret ettiği koleksiyon içerisindeki tüm Actor tipleri Lazy olarak üretilmektedir. Üretim noktası ise currentScene değişkenin Value özelliği ile atamanın yapıldığı satırdır. Bu satırdan sonra Scene nesnesi ve içeriğindeki Actor nesnelerinin oluşturulması gerçekleştirilmektedir. Tabi istenirse Actor oluşturma işlemleride sonraki bir adıma bırakılabilir.

İlginç olan bir noktada, Lazy tipinden özelliklerin (Type Property) tanımlanabilmesidir. Dilerseniz aşağıdaki örnek kod parçasını göz önüne alalım.

![blg96_ClassDiagram2.gif](/assets/images/2009/blg96_ClassDiagram2.gif)

```csharp
using System;
using System.Collections.Generic;

namespace BeLazy
{
    class Program
    {
        static void Main(string[] args)
        {
            Contact cntc = new Contact(1, "Burak Selim Şenyurt", "New York", "USA", "1000");
            Console.WriteLine("Contact içerisindeki Address nesnesi üretilmiş mi ? {0}", cntc.IsAddressCreated);
            Console.WriteLine("{0} {1} {2}",cntc.Address.Country,cntc.Address.City,cntc.Address.PostalCode);
            Console.WriteLine("Contact içerisindeki Address nesnesi üretilmiş mi ? {0}", cntc.IsAddressCreated);
        }
    }

    class Contact
    {
        private Lazy<Address> _address;

        public Contact(int contactId,string name,string city,string country,string postalCode)
        {
            _address = new Lazy<Address>(
                () => new Address {
                     City=city,
                     Country=country,
                     PostalCode=postalCode
                }
                );

            ContactId = contactId;
            Name = name;
        }

        public Address Address
        {
            get
            {
                return _address.Value;
            }
        }
        public bool IsAddressCreated
        {
            get
            {
                return _address.IsValueCreated;
            }
        }
        public int ContactId { get; set; }
        public string Name { get; set; }
        
    }

    class Address
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }
}
```

Örnekte yer alan Contact sınıfı içerisinde Lazy tipinden private bir alan tanımlandığı görülmektedir. Bu alanın oluşturulması Contact sınıfının yapıcı metodu içerisinde gerçekleştirilmektedir. Address isimli özellik ilede, Lazy tipinden olan address alanının Value özelliğinin değeri geriye döndürülmektedir. Zaten Value özelliğine referans atanması gerçekleştirilemediğinden Address isimli özellik sadece get bloğuna sahip olacak şekilde tanımlanmıştır (Read Only). Bu kodlamaya göre Contact sınıfına ait bir nesne örneklendikten sonra içerisinde yer alan Address nesnesi hemen oluşturulmaz. Ancak Contact sınıfının nesne örneği üzerinden Address özelliğine gidilir ve Value özelliğine ulaşılırsa örnekleme işlemi gerçekleşecektir. Bu durum çalışma zamanında daha net bir şekilde görülebilir.

![blg96_ThirdRun.gif](/assets/images/2009/blg96_ThirdRun.gif)

Dikkat edileceği üzere Contact nesnesi örneklense bile Address nesnesi henüz oluşturulmamıştır. Ancak Address nesnesinin Value özelliğine ulaşıldığı ilk yerde bu işlem gerçekleşmektedir.

Lazy tipinin kullanımında dikkatli olunması gereken noktalardan biriside Multi-Thread operasyonlardır. Özellikle birden fazla Thread'in aynı Lazy nesne örneğini kullanması halinde ilgili nesnenin hangi Thread içerisinde oluşturulacağının bir önemi kalmamaktadır. Ancak oluşturulma işlemleri sırasında meydana gelecek istisnalarda (Exceptions) diğer Thread'lerinde otomatik olarak etkilenmesi söz konusudur. Buna ek olarak Lazy tipleri oluşturulurken Thread Safe olup olmayacakları çok basit bir şekilde belirlenebilir. Bu durmu daha net bir şekilde anlayabilmek adına dilerseniz aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System;
using System.Collections.Generic;
using System.Threading;

namespace BeLazy
{
    class Program
    {
        static void Main(string[] args)
        {
            Lazy<Information> information = new Lazy<Information>();

            Thread threadA=new Thread(
                ()=>
                    {
                        Console.WriteLine(information.Value.ToString());
                        Console.WriteLine("\tThread A Thread Id : {0}",Thread.CurrentThread.ManagedThreadId.ToString());
                    }
                    );

            Thread threadB = new Thread(
                () =>
                {
                    Console.WriteLine(information.Value.ToString());
                    Console.WriteLine("\tThread B Thread Id : {0}", Thread.CurrentThread.ManagedThreadId.ToString());
                }
                    );

            Thread threadC = new Thread(
                () =>
                {
                    Console.WriteLine(information.Value.ToString());
                    Console.WriteLine("\tThread C Thread Id : {0}", Thread.CurrentThread.ManagedThreadId.ToString());
                }
                    );

            threadA.Start();
            threadB.Start();
            threadC.Start();

            threadA.Join();
            threadB.Join();
            threadC.Join();
        }
    }

    class Information
    {
        private int threadId;
        private string initializeTime;

        public Information()
        {
            threadId = Thread.CurrentThread.ManagedThreadId;
            initializeTime = DateTime.Now.ToLongTimeString();
            Thread.Sleep(2000);
        }

        public override string ToString()
        {
            return String.Format("Thread Id : {0} Time : {1}", threadId.ToString(), initializeTime);
        }
    }
}
```

Örneğimizde 3 farklı Thread'in çalıştırıldığı görülmektedir. Bu Thread'lerin tamamı kendi içlerinde Lazy tipinden olan information değişkenini kullanmaktadır. Buna göre Thread'lerden hangisi ilk olarak Value özelliğine erişirse, Information nesnesi örneklemiş olacaktır. Buna ek olarak çalışma zamanındaki sonuçlara bakıldığında tüm Thread'lerin ilk üretilen Information nesne örneğine ulaştığı bu nedenle Information yapıcı metodunda oluşturulan ManagedThreadId değerlerinin ve zamanların aynı olduğu görülür. İşte sonuçlar.

![blg96_FourthRun.gif](/assets/images/2009/blg96_FourthRun.gif)

Bu senaryoya göre nesnenin hangi Thread içerisinde oluşturulduğunun hiç bir önemi yoktur. Şimdi akla şöyle bir soru gelebilir. Lazy örneği oluşturulurken Thread-Safe davranılacağı nerede belirtilmiştir?

Aslında Lazy tipinin isThreadSafe parametresinin değeri varsayılan olarak true'dur ve istenirse yapıcı metod içerisinde değiştirilebilir. Özelliğe false değer atanması halinde genellikle ilgili nesnenin tek bir thread içerisinde kullanıldığı varsayılır ve bu azda olsa performans kazanımına neden olur. Ancak birden fazla thread'in aynı Lazy nesnesini kullanacağı hallerde koordinasyon daha büyük önem kazanmaktadır ve bu nedenle isThreadSafe özelliğinin değerinin true olarak bırakılması önerilir.

Kişisel Not: Özellikle birden fazla Thread'in aynı nesnenin kendi içlerinde farklı kopyalarını alarak kullanmaları istendiği durumda.Net 4.0 ile gelen ThreadLocal tipinden yararlanılabilir ki.Net 3.5' te bunun için ThreadStatic isimli bir nitelikten yararlanılmaktadır. İşte size güzel bir araştırma konusu ![Laughing](/assets/images/2009/smiley-laughing.gif)

Buraya kadar anlattıklarımıza göre dikkat edilmesi gereken bazı noktalar olduğuda aşikardır;

- Lazy içerisinde kullanılacak tipin mutlaka varsayılan yapıcı (Default Constructor) metodu olmalıdır. Nitekim oluşturma işleminde aslında Activator sınıfının CreateInstance metodu devreye girmektedir.
- Value özelliği ReadOnly'dir. Bu nedenle kullanıldıktan sonra T tipinden bir referansın atanması gerçekleştirilemez.
- Multi-Thread senaryolarda isThreadSafe özelliğini true olarak bırakmak doğru bir yaklaşımdır.
- Single-Thread senaryolarda isThreadSafe özelliği performans kazanımı adına istenirse false bırakılabilir.
- Value özelliğine erişildiğinde oluşabilecek bir istisna halinde kodun ilerleyen kısımlarında Value özelliğine giden her çağrı için aynı Exception örneği söz konusu olacaktır. Yani Value özelliğinin ilk çağırıldığı yerde oluşacak bir istisna devam eden Value çağrılarına da akacaktır. Bu Multi-Thread senaryolarda da isThreadSafe özelliğinin değeri true bırakıldığı sürece de geçerlidir.
- Çekinilmesi gereken nokta şudur; isThreadSafe özelliğine false değer verilmesi halinde thread'ler arasında senkronizasyon ortadan kalkacağından, bir Thread'in exception gördüğü noktada diğer bir thread kullanılabilir bir Value özelliği ile karşılaşabilir. Aman dikkat.![Sealed](/assets/images/2009/smiley-sealed.gif)

Umarım faydalı bir yazı olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[BeLazy.rar (28,31 kb)](/assets/files/2009/BeLazy.rar)
