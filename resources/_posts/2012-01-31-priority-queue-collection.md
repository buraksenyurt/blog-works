---
layout: post
title: "Priority Queue Collection"
date: 2012-01-31 15:01:00 +0300
categories:
  - csharp
tags:
  - collections
  - priortiy-queue
  - data-structures
  - max-heap
  - min-heap
  - heap-data-structure
  - csharp
---
1996 yılıydı. Efes Pilsen (bu günkü adıyla Anadolu Efes) spor klübü altın dönemlerini yaşıyordu. Tamer Oyguç, Ufuk Sarıca, Peter Naumoski, Hidayet Türkoğlu, Mirsad Türkcan ve diğerleri. O yıl kulüp Avrupa Koraç kupasında finale yükselmiş ve İtalyan temsilcisi Stefanel Milano'nun rakibi olmuştu. İlk maç İstanbul Abdi İpekçi spor salonundaydı. Üniversitedeydim ve arkadaşlarımla günün erken saatlerinde stad kuyruğuna girmiştim. Biletlerimiz vardı ancak bulunduğumuz tribünde herkes karışık oturacağı için iyi bir yer bulmak adına sabahın köründe ve soğuk bir havada kendimizi orada buluvermiştik. Ne varki sabahın erken saatlerinde sıranın başlarında iken bu zamanla değişmeye başlamıştı. Çünkü alt yola bir basketçi geliyor, hayranları ona bakmak için bulunduğumuz kuyruğa hücum ediyor ve baktıkları yerden tekrar geriye çıkmıyorlarda. 8nci sıradan 148nci sıraya kadar gelmişimdir sanıyorum ki. Bu noktada çok doğal olarak sıranın en başındaki kişi de sıksık değişiyordu.

![artcl_7_1.jpg](/assets/images/2012/artcl_7_1.jpg)

![Sealed](/assets/images/2012/smiley-sealed.gif)

Aslında VIP benzeri bir durum söz konusu idi belkide. Hani pek çok filmde görmüşüzdür. Lüks bir barın veya gece kulübünün önünde içeri girmek için bekleyen pek çok insandan oluşan bir kuyruk söz konusu olur. Ama genelde filmin kahramanı ve hatta arkadaşları gelir, en önden kulübe pat diye girerlar. Çünkü yüksek öncelikli şahsiyetlerdir. Sanırım hayatımızda yaşadığımız bu ve benzeri tipteki vakalar yazılım dünyasından da nasibini almıştır. Çünkü yazılım tarafında da Öncelikli Kuyruk Koleksiyonu (Priority Queue Collection) denen bir veri yapısı (data structure) söz konusudur

![Smile](/assets/images/2012/smiley-smile.gif)

(Detaylı bilgi için [Wiki](http://en.wikipedia.org/wiki/Priority_queue) adresine bakabilirsiniz)

Temel olarak bu tip bir koleksiyon Queue (FIFO - First In First Out ilkesine göre çalışmaktadır) ve Stack (LIFO - Last in First Out ilkesine göre çalışmaktadır) tipinden olanlarına benzer. Bu tip koleksiyonlar (generic karşılıkları da dahil olmak üzere), bildiğiniz gibi tek boyutludur. Priority koleksiyonunda ise devreye bir öncelik değeri girmektedir. Bir başka deyişle koleksiyona eklenecek olan elemanın öncelik derecesi söz konusu olup ilk sıraya yerleşmesi söz konusudur. Priority Queue koleksiyonunda, koleksiyona dahil olacak eleman yüksek öncelik derecesine göre kuyuruğun ön sırasına alınır. Bunun dışında söz konusu koleksiyon modelinde ilk olarak elde edilmesi gereken veya listeden çıkartılması icap eden bir eleman bulunurken bu yüksek öncelik seviyesi göz önüne alınabilir (Yine de dilerseniz bu standart veri yapısı kurallarını esnetebilirsiniz)

Bildiğiniz gibi.Net Framework platformu ilk sürümünden itibaren çeşitli tipte koleksiyonlara hizmet etmektedir. İlk sürümde Stack, Queue, ArrayList, Hashtable ve SortedList gibi farklı şekillerde çalışan koleksiyonlar söz konusudur. Framework.Net 2.0 sürümüne yükseldiğinde ise koleksiyonların generic olma durumu devreye girmiş ve bu sayede tür bağımsız ve tip güvenli (Type Safety) versiyonlar ortaya çıkmıştır (List, Dictionary, Stack, Queue, HashSet, SortedDictionary, SortedList gibi). Tabi Framework sürümü 4.0' a yükseldiğinde ve işin içerisinde paralel programlama kabiliyetleri de girdiğinde, Concurrent olarak çalışabilen koleksiyonlar ortaya çıkmıştır (BlockingCollection, ConcurrentBag, ConcurrentDictionary, ConcurrentQueue, ConcurrentStack, OrderablePartitioner, Partitioner gibi).

Ne yazık ki tüm bu koleksiyon tipleri arasında öncelik seviyelendirmesini kullanan bir Queue koleksiyonu mevcut değildir. Bir başka deyişle iş başa düşer ve bu koleksiyonu bizim yazmamız gerekir. Aslında.Net Framework alt yapısı içerisindeki koleksiyonların gelişimini düşündüğümüzde, söz konusu yeni koleksiyon tipinin generic bir versiyonu dışında Concurrent çalışabilen sürümünü de yazmak icap etmektedir

![Sealed](/assets/images/2012/smiley-sealed.gif)

Biz bu yazımızda basit ve daha kolay bir adım atıp generic olan bir sürümünü yazmaya çalışıyor olacağız.

Tasarlayacağımız yeni koleksiyon tipi içerisinde işimizi kolaylaştırmak adına Sorted List kullanımı söz konusu olabilir aslında. Fakat yaptığım araştırmalar sonucunda bu tip bir kullanımın performans açısından olumsuz etkileri olduğunu gördüm. Dolayısıyla bu tip bir veri yapısının literatürde bahsedildiği üzere Yığın Veri Yapısı (Heap Daha Structure) ile çalışması tavsiye edilen ve önerilen yoldur. Heap veri yapısı, ağaç tabanlı (Tree Based) bir içeriği model olarak almaktadır. Ağacın en üst dalında her zaman için en yüksek Key değerine sahip olan eleman yer alır. Tabi bu durumu tersine doğru çevirebiliriz de. Yani en düşük Key değerine sahip elemanın ağacın en üst dalında olması da sağlanabilir. Söz konusu kullanımlara göre Heap veri yapısı Max-Heap veya Min-Heap olarakta adlandırılmaktadır. (Heap veri yapısı hakkında daha detaylı bilgiye yine [Wiki adresi](http://en.wikipedia.org/wiki/Heap_%28data_structure%29) üzerinden ulaşabilirsiniz)

Dilerseniz öncelikli olarak bu koleksiyon tipini geliştirmeye çalışalım. Bu amaçla basit bir Console projesi açıp aşağıdaki sınıfı geliştirdiğimizi düşünebiliriz.

```csharp
using System;
using System.Collections.Generic;

namespace PQueue
{
    public class PriorityQueue<P, V> // P priority derecesini V ise değeri temsil eder
    {
        private List<KeyValuePair<P, V>> heap; // Heap veri yapısına uygun olacak şekilde içeriğimizi tutacağımız koleksiyon
        private IComparer<P> comparer; // Max-Heap veya Min-Heap' e göre bir uyarlamaya hizmet verebilmek için kullanılacak arayüz referansı
        private const string ioeMessage = "Koleksiyonda hiç eleman yok";

        #region Constructors

        // P ile gelen tipin varsayılan karşılaştırma kriterine göre bir yol izlenir
        public PriorityQueue()
            : this(Comparer<P>.Default)
        {
        }

        // P tipinin karşılaştırma işlevselliğini üstlenen bir IComparer implementasyonu ile bir Construct işlemi söz konusudur
        public PriorityQueue(IComparer<P> Comparer)
        {
            if (Comparer == null)
                throw new ArgumentNullException();

            heap = new List<KeyValuePair<P, V>>();
            comparer = Comparer;
        }

        #endregion

        #region Temel Fonksiyonlar

        // Koleksiyona bir veri eklemek için kullanıyoruz
        public void Enqueue(P Priority, V Value)
        {
            KeyValuePair<P, V> pair = new KeyValuePair<P, V>(Priority, Value);
            heap.Add(pair);

            // Sondan başa doğru yeniden bir sıralama yaptırılır
            LastToFirstControl(heap.Count - 1);
        }

        public KeyValuePair<P, V> Dequeue()
        {
            if (!IsEmpty) // Eğer koleksiyon boş değilse 
            {
                KeyValuePair<P, V> result = heap[0]; // Heap' in Root' undaki elemanı al
                // Dequeue mantıksal olarak ilk elemanı geriye döndürürken aynı zamanda koleksiyondan çıkartmalıdır
                if (heap.Count <= 1) // 1 veya daha az eleman söz konusu ise zaten temizle
                {
                    heap.Clear();
                }
                else // 1 den fazla eleman var ise ilgili elemanı koleksiyondan çıkart
                {
                    heap[0] = heap[heap.Count - 1]; 
                    heap.RemoveAt(heap.Count - 1);
                    FirstToLastControl(0); // ve koleksiyonu baştan sona yeniden sırala
                }
                return result;
            }
            else
                throw new InvalidOperationException(ioeMessage);
        }

        // Peek operasyonu varsayılan olarak ilk elemanı geriye döndürür ama koleksiyondan çıkartmaz(Dequeue gibi değildir yani)
        public KeyValuePair<P, V> Peek()
        {
            if (!IsEmpty)
                return heap[0];
            else
                throw new InvalidOperationException(ioeMessage);
        }

        // Koleksiyonda eleman olup olmadığını belirtir
        public bool IsEmpty
        {
            get { return heap.Count == 0; }
        }

        #endregion
        
        #region Sıralama Fonksiyonları

        private int LastToFirstControl(int Posisiton)
        {
            if (Posisiton >= heap.Count) 
                return -1;

            int parentPos;

            while (Posisiton > 0)
            {
                parentPos = (Posisiton - 1) / 2;
                if (comparer.Compare(heap[parentPos].Key, heap[Posisiton].Key) > 0)
                {
                    ExchangeElements(parentPos, Posisiton);
                    Posisiton = parentPos;
                }
                else break;
            }
            return Posisiton;
        }

        private void FirstToLastControl(int Position)
        {
            if (Position >= heap.Count) 
                return;

            while (true)
            {
                int smallestPosition = Position;
                int leftPosition = 2 * Position + 1;
                int rightPosition = 2 * Position + 2;
                if (leftPosition < heap.Count &&
                    comparer.Compare(heap[smallestPosition].Key, heap[leftPosition].Key) > 0)
                    smallestPosition = leftPosition;
                if (rightPosition < heap.Count &&
                    comparer.Compare(heap[smallestPosition].Key, heap[rightPosition].Key) > 0)
                    smallestPosition = rightPosition;

                if (smallestPosition != Position)
                {
                    ExchangeElements(smallestPosition, Position);
                    Position = smallestPosition;
                }
                else break;
            }
        }

        private void ExchangeElements(int Position1, int Position2)
        {
            KeyValuePair<P, V> val = heap[Position1];
            heap[Position1] = heap[Position2];
            heap[Position2] = val;
        }

        #endregion
    }
}
```

PriortiyQueue isimli sınıfımız, P tipinin değerine ve seçilen karşılaştırma kriterine göre bir öncelik seviyelendirmesini kullanmaktadır. Söz konusu seviyelendirmeye göre de V tipi ile belirtilen değerlerin koleksiyon içerisindeki önceliği değişmektedir. Enqueue ve Dequeue gibi veri ekleme ve en yüksek önceliğe sahip veriyi elde edip listeden çıkartmak gibi temel Queue fonksiyonellikleri dışında Peek isimli operasyon desteği de mevcuttur. Şimdi dilerseniz yazdığımız bu yeni koleksiyon tipini test edelim. Bu amaçla aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System;

namespace PQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            PriorityQueue<int, Person> pQueue = new PriorityQueue<int, Person>();

            pQueue.Enqueue(9, new Person { Name = "Bill", PersonId = 10, Salary = 1000 });
            pQueue.Enqueue(10, new Person { Name = "Jeni", PersonId = 5, Salary = 950 });
            pQueue.Enqueue(8, new Person { Name = "Samantha", PersonId = 4, Salary = 750 });
            pQueue.Enqueue(17, new Person { Name = "Richard", PersonId = 3, Salary = 800 });
            pQueue.Enqueue(12, new Person { Name = "Steve", PersonId = 9, Salary = 2000 });

            Console.WriteLine("Şu anda en yüksek öncelik {0} isimli personeldedir",pQueue.Dequeue().Value.Name);

            // Şu anda en yüksek öncelik seviyesi Dennis' de olduğundan Root' a yerleştirilecektir
            pQueue.Enqueue(3, new Person { Name = "Dennis", PersonId = 1, Salary = 350 });
            Console.WriteLine("Şu anda en yüksek öncelik {0} isimli personeldedir", pQueue.Dequeue().Value.Name);
            Console.WriteLine("Şu anda en yüksek öncelik {0} isimli personeldedir", pQueue.Dequeue().Value.Name);
        }
    }

    class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
    }
}
```

Örneğimizde kullandığımız PriorityQueue nesne örneği, seviyelendirme için int tipini kullanmakta ve buna göre Person tipinden olan nesne örneklerini Min-Heap mantığında değerlendirmektedir. İlk olarak Bill, Jenni, Samantha, Richard ve Steve isimli çalışanlar kuyruğa eklenmiştir. Bu personelin öncelik dereceleri sırasıyla 9,10,8,17 ve 12' dir. Geliştirdiğimiz tip Min-Heap yapısını ele aldığından yani aslında düşük değer yüksek öncelik anlamına geldiğinden ilk dizilime göre kuyruktan ilk çıkacak kişi Samantha'dır. Bu andan itibaren Dequeue metodunu kullandığımız noktalarda öncelik seviyesi yüksek olan (yani en küçük int değerine sahip olan) çalışan elde edilecek ve aynı zamanda koleksiyondan çıkartılacaktır. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![image.axd](/assets/images/2012/image.axd)

Burada dikkat edilmesi gereken nokta Dequeue metodunun her zaman için öncelik seviyesi yüksek olan (örneğimizde int değeri en düşün olan) elemanı ilk olarak listeden getiriyor olmasıdır. Yani klasik Queue koleksiyonunda olduğu gibi FIFO ilkesi değerlendirilirken, öncelik seviyesi (Priority Level) baz alınmaktadır. Kodu Debug ederek incelediğinizde ancak var olan elemanlardan daha yüksek öncelik seviyesine sahip bir eleman eklendiğinde listenin başına alındığını görürüz. Bu işleyişte koleksiyon üzerinde her hangibir sıralama operasyonunun söz konusu olmadığına özellikle dikkat etmenizi isterim.

Görüldüğü gibi biraz uğraşarak öncelikli derecelendirmeli bir kuyruk koleksiyonu tipini yazabildik. Sizde denemelerinizde IComparer implementasyonu yapan bir kritere göre söz konusu koleksiyonu Max-Heap mantığında çalıştırmay deneyebilirisiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[PQueue.rar (31,33 kb)](/assets/files/2012/PQueue.rar)
