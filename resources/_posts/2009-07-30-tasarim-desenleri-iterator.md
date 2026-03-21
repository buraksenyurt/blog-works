---
layout: post
title: "Tasarım Desenleri - Iterator"
date: 2009-07-30 12:52:00 +0300
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - design-patterns
  - oop
  - csharp
---
Küçüklüğümde pek çoğumuz gibi sahip olduğum bir pul koleksiyonum vardı. Halen daha sakladığım pullar bulunmaktadır. Hatta o zamanlarda, çocuklar posta aracılığıyla yurt dışından arkadaşlar edinir, birbirleriyle pul değiş tokuşu bile yaparlardı. Düşünsenize, hem yabancı dilinizi geliştiriyor hem pul koleksiyonunuzu genişletiyorsunuz.

![blg53_Giris.jpg](/assets/images/2009/blg53_Giris.jpg)

Tabiki posta mesajlaşması biraz zaman alan bir mevzuydu. Bu günkü gibi sosyal içerikli portallar veya mesajlaşma cihazları ve daha nice gelişmiş teknoloji yoktu. Acaba bu devirde yaşayan çocuklardan kaçı pul koleksiyonu yapıyor

![Undecided](/assets/images/2009/smiley-undecided.gif)

Neyse bu duygusal ortamdan çıkalım hemen. Pul koleksiyonumda yaptığım işlerden birisi zaman zaman onları baştan sonra, yada sondan başa, yada ortadan bir yerden herhangibir yöne doğru gözle taramak olurdu. Bazen kendi kafama göre sıralarını değiştirirdim. Peki nesne yönelimli dillerde kullandığımız koleksiyon veya dizi gibi veri yapıları üzerindede bu ve benzer işlemleri yapmıyor muyuz? Çeşitli tipte veri yapılarında (Data Structures) dolaşıyor, içeriklerine bakıyoruz.

Koleksiyonlar, C# gibi bir programlama dilinde belkide en önemli veri yapılarından (Data Structures) birisidir. Bir koleksiyon kendi içerisinde farklı tipte veya aynı tipte nesneleri çeşitli formatlarda (List, Stack, Queue, Dictionary vb...) saklayabilen nesne bütünleri olarak düşünülebilir. Hatta bildiğiniz üzere.Net 2.0 ile birlikte C# ve Vb.Net tarafına kazandırılan generic yetenekler ile, koleksiyonların tip güvenli (Type Safety) olarak ele alınmalarıda garanti edilmiştir. Hatta, C# 3.0 ve Vb 9.0 ile birlikte neler olmuştur neler

![Wink](/assets/images/2009/smiley-wink.gif)

Artık koleksiyonlar üzerinden LINQ sorguları yardımıyla sanki bir veritabanı tablosunu sorgularmışcasına filtrelemeler yapılabilmektedir. Ancak olayın en başından beri süre gelen ve bu yazımıza konu olan bir durumda söz konusudur. Bir koleksiyonun veya bir dizinin iç yapısını bilmeye gerek duymadan, başından sonunda kadar dolaşılabilmesi mümkündür. Dolayısıyla, koleksiyon veya dizi gibi bir nesne bütününün içerisindeki elemanlara erişilmesi ve dolaşılması noktasında devreye giren bir aktör olmalıdır. Aslında bu sorumluluk, bir öteleme nesnesine (Iterator Object) verilmiştir.

Bu açıdan bakıldığında nesne bütününün elemanlarına (çoğunlukla koleksiyon veya dizi olarak düşünebiliriz) erişilmesi, bu elemanların baştan sona dolaşılması, bir öteleme sırasında nerede kalındığının tutulması, hangi koşula göre devam edilmesi gerektiğinin bilinmesi, devam edilecek ise bir sonra gelen nesnenin döndürülmesi gibi sorumlulukları üstüne alan bir aktörden bahsetmekteyiz. Ki bu aktör aslında generic programlamada önemli bir yerede sahiptir. Nitekim, herhangibir nesne bütünün içinde dolaşılması için standart bir yol sunulması generic programlamanın gereksinimlerinden birisidir. Veri yapılarının ne kadar sık kullanıldığı düşünülünce doğal olarak ortaya, tasarımı kalıplaşmış bir uygulama biçimi çıkmaktadır. İşte bu yazımızın konusu, Behavioral (Davranışsal) kalıplardan olan Iterator tasarım deseni.

Tabiki programlama dillerine zaman içerisinde gelen bazı ek yetenekler sayesinde desenin uygulanış biçimi çok daha kolaylaşmıştır. Özellikle C# tarafında, 2.0 versiyonu ile birlikte gelen yield anahtar kelimesinin kullanımı, C# 3.0 ile birlikte LINQ (Language Integrated Query) özelliklerinin gelmesi aslında nesnelerin elemanları üzerinde bir uçtan diğerine farklı filtrelemelere göre hareket edilmesini son derece kolaylaştırmaktadır. C# tarafında bu konu ile ilişkili baş aktör IEnumerable arayüzüdür (Interface). Kendisi doğal yollardan Iterator deseninin uygulanabilir olmasını sağlamaktadır. Biz bu yaklaşımları yazımızın sonlarında değerlendireceğiz. Şimdilik desenimizi kuralına uygun olaraktan geliştireceğiz. Öncesinde iterator kalıbına örnek bir kaç senaryo üzerinde durmaya çalışalım.

Örneğin herhangibir bilgisayar sisteminde yer alan klasör yapısında bu desenin uygulanışını değerlendirebiliriz. Klasörler kendi içlerinde alt klasörler veya dosyalar içerir. Bunların ekrana beliri bir formatta listelenmesi sırasında o anki klasör ağacının tamamının bir uçtan diğerine dolaşılması gerekecektir. Ya da bir klasörün toplam boyutunun bulunması istendiğinde, alt klasör ve içlerindeki dosyaların boyutlarınında bir uçtan diğerine değerlendirilmesi gerekecektir. Buradaki klasör yapısı ve içeriği nesnel bazda düşünüldüğünde, çeşitli filtrelemelere göre değerlendirilebilmesinde sorumluluk, Iterator nesnesi tarafından üstlenilebilir.

Unutulmaması gereken noktalardan birisi de, Iterator tasarım kalbında nesne bütünü içerisindeki elemanların nasıl yapılandırıldıklarının bir öneminin olmayışıdır. Desenin amacı söz konusu nesne bütünü baştan sona dolaşabilmektir. Bir başka örnek olarak bir şirketin organizasyonel yapısını ifade eden bir nesne bütünü göz önüne alınabilir. Ağaç yapısı şeklinde ifade edilebilecek bu nesne bütünün içerisinde hareket edilebilmesi sırasında dalların dizilişleri, alt dallarda kimlerin olduğu, nasıl hareket edilmesi gerektiği gibi kriterlerin sorumlulukları Iterator nesnesine yüklenebilir.

Şimdi basit bir örnekten ilerleyerek desenimizi kavramaya çalışalım. Ama öncesinde UML şemamıza bakmakta yarar olduğu kanısındayım.

![blg53_uml.gif](/assets/images/2009/blg53_uml.gif)

Şekildende görüldüğü üzere Iterator nesnesi, öteleme sorumlulukları için bir arayüz sunmakta ve nesne kümesi içerisindeki hareketlilik sırasında yapılması gereken bazı operasyonları bildirmektedir. O anki nesnenin kim olduğunun bilinmesi için CurrentItem gibi bir metod (veya özellik-property olabilir) kullanılmaktadır. Bütün içerisindeki bir sonraki elemana geçmek için MoveNext metodu, takip eden nesne olup olmadğını tespit etmek için IsContinue metodu kullanılabilir. Yada iterasyona başlarken ilk elemana gitmek için First operasyonu ele alınabilir. Tabiki bu metodlar tamamen semboliktir. Nitekim IEnumerator arayüzüde kendi içerisinde buna benzer metodları sunmaktadır.

![blg53_4.gif](/assets/images/2009/blg53_4.gif)

UML şemamıza baktığımızda, istemciden nesne bütününün kendisine (Aggregate object) ve Iterator tipine doğru bir Association tanımlandığını görmekteyiz. Sonuç olarak istemci tarafı Aggregate nesnesini kullanmakta ve içerisinde dolaşmak için Iterator örneklerinden yararlanmaktadır. Benzer şekilde iterasyon sorumluluğunu yerine getiren nesnede (Concrete Iterator) çok doğal olarak ConcreteAggregate nesnesinin üyelerine erişmekte ve kullanmaktadır. Yani ConcreteIterator'dan ConcreteAggregate'e doğru bir ilişki (Association) mevcuttur.

Artık örneğimizi tasarlamaya başlayabiliriz. Kalıbın nasıl uygulandığını görmek istediğimizden çok basit bir senaryo üzerinden gideceğiz. Senaryomuzda Product tipinden nesne örneklerini barındıran bir nesne bütünümüz olduğunu göz önüne alacağız. Buna göre Iterator tasarım kalbının kullanaraktan, ürünleri dolaşabilmek için bir Iterator nesnesinin nasıl geliştirilebileceğini ele alacağız. Sınıf diagramımız,

![blg53_2.gif](/assets/images/2009/blg53_2.gif)

şeklinde olup kodlarımızda aşağıdaki gibidir.

```csharp
using System;
using System.Collections;
using System.Collections.Generic;

namespace IteratorPattern
{
    // Item
    class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", ProductId.ToString(), Name, ListPrice.ToString("C2"));
        }
    }

    // Iterator
    // Nesne bütünü içerisindeki hareketlerin, yönlenmelerin gerçekleştirilebilmesi için gerekli operasyon arayüzünü tanımlar.
    interface IProductIterator
    {        
        Product First();
        Product MoveNext();
        bool IsContinue { get; }
        Product Current { get; }
    }

    // Aggregate
    // Nesne bütününün, iterasyon için Concrete Iterator tipinden nesne örneği döndürecek bir metodunun olmasını söyler.
    interface IProductCollection
    {
        IProductIterator GetIterator();
    }

    // Concrete Aggregate
    // Nesne kümesini barındıran tipimiz.
    class ProductCollection
        : IProductCollection
    {
        // Product topluluğunu saklamak için generic bir List<T> koleksiyonundan yardım alıyoruz.
        private List<Product> list = new List<Product>();

        // Ürün sayısını dışarıya vermek için kullanılan bir özellik
        public int ProductCount
        {
            get { return list.Count; }
        }

        // Eleman eklemek ve okumak için kullanılan bir Indeksleyici
        public Product this[int index]
        {
            get { return list[index]; }
            set { list.Add(value); }
        }
                
        #region IProductCollection Members

        // Iterator nesnesini örnekler
        public IProductIterator GetIterator()
        {
            // Iterator nesnesi örneklenirken parametre olarak o andaki ProductCollection nesne örneği referans olarak gönderilir. 
            // Bu sayede ProductIterator isimli Concrete Iterator nesne örneği, çalışma zamanında hangi nesne bütününü dolaşacağını bilecektir.
            return new ProductIterator(this);
        }

        #endregion
    }

    // Concrete Iterator
    // Nesne bütününün bir ucundan diğerine hareket edilebilmesine olanak sağlayacak fonksiyonellikleri uygulayan asıl Iterator tipi
    class ProductIterator
        : IProductIterator
    {        
        // Iterator nesne örneğinin, çalışma zamanında hangi nesne bütününü dolaşacağını bilmesi gerekmektedir. 
        private ProductCollection _books;
        private int _currentIndex = 0;
        // İstemci isterse adım sayısını değiştirebilir. Örneğin ikişer ikişer atlanarak gidilmesi sağlanabilir,
        public int StepSize { get; set; }

        // bu nedenle yapıcı metoda parametre olarak, ProductCollection(Concrete Aggregate) nesne örneğinin referansı gelir. Bu referansın GetIterator metodu içerisindeki çağrı ile gönderildiğini hatırlayalım.
        public ProductIterator(ProductCollection productCollection)
        {
            _books = productCollection;
        }
        #region IProductIterator Members

        // İlk elemana gidilmesini sağlayan metod
        public Product First()
        {
            // Nerede olunduğunun takibi için _currentIndex değeri set edilir
            _currentIndex = 0;
            return _books[0];
        }

        // Bir sonraki elemana geçilmesini sağlayan metod
        public Product MoveNext()
        {
            // Nerede olunduğunun takibi için _currentIndex değeri set edilir. Adım sayısı kadar arttırılır.
            _currentIndex += StepSize;
            if (IsContinue) // Eğer takip eden bir eleman var ise geri döndürülür
                return _books[_currentIndex];
            else
                return null;
        }

        // Takip eden ürün olup olmadığını belirten read-only özellik
        public bool IsContinue
        {
            get { return _currentIndex < _books.ProductCount; }
        }

        // O anki elemanı döndüren read-only özellik
        public Product Current
        {
            get { return _books[_currentIndex]; }
        }

        #endregion
    }

    class Program
    {
        static void Main(string[] args)
        {
            ProductCollection products = new ProductCollection();

            products[0]=new Product{ ProductId=1, Name="330 ml Seramik Bardak", ListPrice=12M};
            products[1] = new Product { ProductId = 2, Name = "1 Lt Cam Bardak", ListPrice = 12.5M };
            products[2] = new Product { ProductId = 3, Name = "50 cl Pet Şişe", ListPrice = 14.45M };

            // Iterator nesnesi products isimli koleksiyonu kullanmak üzere oluşturulur
            ProductIterator iterator = new ProductIterator(products);

            // Adım sayısı belirlenir
            iterator.StepSize = 1;

            // First ile ilk elemana konumlanılır.
            // Koşul olarak IsContinue değerine bakılır
            // İlerleme için MoveNext metodu kullanılır.
            for (
                Product product = iterator.First()
                    ; iterator.IsContinue
                    ; product = iterator.MoveNext()
                    )
            {
                Console.WriteLine(product.ToString());
            }
        }
    }
}
```

Örneğimizi çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçları elde ederiz.

![blg53_1.gif](/assets/images/2009/blg53_1.gif)

Tabiki amacımız sadece kalıbın nasıl uygulandığını öğrenmek olduğundan, işimizi kolaylaştırması için aslında içeride generic bir List koleksiyonundan yararlandık. Ama tabiki var olan koleksiyonlar ile ifade edilemeyecek bir nesne bütünü olduğunda (özel bir ağaç yapısı olabilir) daha farklı bir depolama modeli kullanmamız gerekebilir. DoFactory.com sitesinin istatistiklerine göre neredeyse kullanılmadığı görülmemiş bir tasarım deseni ile karşı karşıyayız aslında. Peki, aramızdan kaç geliştirici C# veya Vb.Net tarafında bu deseni isteyerek ve bilinçli olarak kullandı.

Ne kadar ilginç değil mi? Kodlama sırasında bir koleksiyon üzerinde dolaşırken tek yapmamız gereken çoğunlukla bir döngüyü kullanmaktır (for, foreach, while vb...). Hatta basit bir LINQ sorgusu sonrası filtrelenmiş bir içeriği bile for, while gibi döngüler ile dolaşmamız söz konusudur. Ama hiç arka planda bu sorumluluğu alan bir Iterator nesnesi olduğunu ve bir kalıp uygulandığını düşünmeyiz. Şimdi bu moral bozukluğu ile aslında işleri nasıl kolaylaştırmış olduğumuza bir bakalım

![Wink](/assets/images/2009/smiley-wink.gif)

Yukarıda geliştirdiğimiz örneğin benzeri ile devam ediyor olacağız. İşte IEnumerable arayüzü ve yield anahtar kelimesi...

```csharp
class ProductList
    : IEnumerable<Product>
    {
        private List<Product> list = new List<Product>();

        public Product this[int index]
        {
            get { return list[index]; }
            set { list.Add(value); }
        }

        #region IEnumerable<Product> Members

        public IEnumerator<Product> GetEnumerator()
        {
            foreach (Product product in list)
            {
                yield return product;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
```

Dikkat edileceği üzere.Net içerisinde yer alan ve nesnelere iterasyon öğreten IEnumerable arayüzünü yield ile birlikte ele alarak, ProductList nesne örnekleri içerisinde dolaşılabilmesini sağlayacak geliştirmeyi kolayca yapmış olduk.(Tabi bir versiyon daha geriye gidebilir ve IEnumerator arayüzünü kullanaraktanda bu işlemleri gerçekleştirebiliriz, bunuda hatırlatayalım) Örnek kullanımı ise şu şekilde gerçekleştirebiliriz;

```csharp
ProductList products2 = new ProductList();
products2[0] = new Product { ProductId = 1, Name = "330 ml Seramik Bardak", ListPrice = 12M };
products2[1] = new Product { ProductId = 2, Name = "1 Lt Cam Bardak", ListPrice = 12.5M };
products2[2] = new Product { ProductId = 3, Name = "50 cl Pet Şişe", ListPrice = 14.45M };

foreach (Product product in products2)
{
     Console.WriteLine(product.ToString());
}
```

Uzun uzun zaman önce, C# 2.0 ile birlikte gelen yenilikleri anlatırken yield anahtar kelimesinide içeriklere kattığımı gayet net hatırlıyorum. Aslında bu anahtar kelimenin, var olan Iterator deseninin uygulanmasını dahada kolaylaştırdığı gün gibi ortada. Bir başka deyişle Iterator deseninin aslında.Net içerisine gömülü olduğunu söyleyebiliriz. Tabi kalıbı bizzat uygulamamış olsakta aslında IL (Intermediate Language) tarafındaki kodlara bakıldığında Iterator tasarım kalıbının izlerini görmemiz mümkündür.

![blg53_3.gif](/assets/images/2009/blg53_3.gif)

Böylece geldik bir tasarım kalıbının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[IteratorPattern.rar (30,73 kb)](/assets/files/2009/IteratorPattern.rar)
