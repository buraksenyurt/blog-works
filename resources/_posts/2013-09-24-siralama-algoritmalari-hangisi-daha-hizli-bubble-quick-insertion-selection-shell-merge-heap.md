---
layout: post
title: "Sıralama Algoritmaları - Hangisi Daha Hızlı (Bubble, Quick, Insertion, Selection, Shell, Merge, Heap)"
date: 2013-09-24 14:08:00 +0300
categories:
  - csharp
  - data-structures-algorithms
tags:
  - csharp
  - data-structures-algorithms
  - http
  - java
  - performance
  - pointers
---
Evimdeki çalışma odasında yer alan kütüphanemi zaman zaman gelen yeni kitaplar ve afacan S (h) arp Efe'nin haylazlıkları nedeni ile darma duman halde bulabiliyorum. Hal böyle olunca çoğu zaman kitaplıkta yer alan onlarca kitabı tekrardan düzenlemem ve uygun bir sırada dizmem gerekebiliyor. Hatta bunu kitapların tozunu almak için hepsini yerlere indirdikten sonra da yaşayabiliyorum. Aslına bakarsanız her seferinde farklı bir kategorilendirme yapıyor ve buna göre bir sıralama işlemi icra etmeye çalışıyorum. Tabi el çabukluğu dışında akıllı düşününce sıralamak ve yerleştirmek kısa sürede bitebiliyor. Ama bazen de kafa bulanık olunca bu işlem sandığımdan da uzun sürüp bir işkence haline gelebiliyor.

[![Sorting_1](/assets/images/2013/Sorting_1_thumb.jpg)](/assets/images/2013/Sorting_1.jpg)


Üniversite yıllarında özellikle programlama derslerinde buna benzer şekilde sıralama algoritmaları ile haşır neşir olmayanımız yoktur eminim ki. Hatta çoğu sınavın korkulu rüyası sorularının kaynağını teşkil etmektedir ki hocalarımız genellikle bunları kağıt kalem kullanarak çözmemizi isterler (En azından benim zamanında böyleydi)

Özellikle bu algoritmaların dil bağımsız olan Pseudo Code içeriklerinden yararlanarak her hangi bir dile uygulanmaya çalışılması üzerine epeyce kafa yormuşuzdur. C, C++, Java, Basic, Pascal ve benzeri temel programlama derslerinde edindiğimiz bilgiler ile bu algoritmaları yazmak için çokça uğraşmışızdır. Tabi üzülerek söylemeliyim ki ben üniversite yıllarındayken Bubble ve Quick Sort sıralama algoritmalarından fazlasını ne yazık ki göremedim.

Dolayısıyla o zamanlarda iş başa düşmüş ve diğer algoritmaları kendi başıma araştırıp öğrenmeye çalışmıştım. Çok şükür ki günümüzde Internet elimizin altında ve her ne kadar bilgi kirliliği olması söz konusu ise de her çeşit bilgiye kolayca ulaşmamız mümkün. Dolayısıyla sıralama algoritmalarının fazlasını herhangibir dil için kolayca bulabiliyoruz.

Gelelim bu yazımızın konusuna. Geçtiğimiz günlerde yine boş anlarıma denk gelen bir zaman diliminde, sıralama algoritmaları arasındaki hız farklarını incelemeye çalışmaktaydım. İş bir süre sonra Console uygulamasının Main metodu içerisine hapsolmanın ötesine geçti. Dilerseniz hiç vakit kaybetmeden kodlarımızı değerlendirmeye başlayalım. Amacımız bir performans test uygulaması geliştirmek olacak.

Ele almayı düşündüğüm bir kaç sıralama algoritması söz konusu idi. Kimisinin yazımı oldukça kolay, kimisin ise inanılmaz karmaşıktı. Örnekte Bubble, Heap, Insertion, Merge, Quick, Selection ve Shell sıralama algoritmalarını ele aldım. Aslında amacım algoritmaların yazılmasından ziyade, bunları aynı dizi içerikleri ile test edebilmenin kolay bir yolunu bulmaktı. Bu noktada aklımdaki yapıda tek bir Execute metodunun, kendisine parametre olarak gelen herhangibir sıralama algoritmasını, belli bir dizi için yapması gerektiğini düşünmekteydim. Bu tip bir test ortamını kurmak çok zor değildir aslında. Aynı metodun kendisine parametre olarak gelen herhangibir sıralama algoritmasını icra etmesini planlıyorsak bir arayüz (Interface) tipinden yararlanabiliriz.

> Hatırlayacağınız üzere (Hatta hatırlamanız gerektiği üzere) Interface'ler ile, kendilerini uygulayan diğer tiplere icra etmesi gereken zorunlulukları bildirebilir ve diğer yandan çok biçimli olarak hareket etmelerini sağlayabiliriz. (Özellikle Plug-In tabanlı programlamada çok işe yaradığını unutmayalım)

Örneğimizde son derece basit bir arayüz tipi kullanılmaktadır. Aşağıdaki kod parçasında görüldüğü gibi.

```csharp
namespace SortingAlgorithm 
{ 
    public interface ISorter 
    { 
        string Description { get; } 
        void Execute(int[] Array); 
    } 
}
```

ISorter arayüz tipi basit olarak iki üye (Member) bildirimi içermektedir. Execute metodu parametre olarak gelen Array dizisi üzerinde uygulanacak sıralama işlemini icra eden fonksiyon bildirimidir. Diğer yandan Description özelliği ile de sıralama algoritması için kısa bir açıklama verilmesi sağlanabilir. Dolayısıyla sıralama işlerini üstlenecek olan tiplerin bu arayüzü uygulamasını sağlayarak yola devam edebiliriz. (Hem de tüm sıralama algoritmalarının C# tarafındaki kod karşılıklarını tek bir yazı içerisinde toplamış oluruz) Öyleyse hiç vakit kaybetmeden sıralama algoritma tiplerini yazmaya başlayalım.

## Bubble Sort

```csharp
namespace SortingAlgorithm.Sorters 
{ 
    public class Bubble 
        :ISorter 
    { 
        #region ISorter Members 

        public string Description 
        { 
            get { return "Bubble Sort Sıralama Algoritması"; } 
        } 

        public void Execute(int[] Array) 
        { 
            int i; 
            int j; 
            int TempValue; 

            for (i = (Array.Length - 1); i >= 0; i--) 
            { 
                for (j = 1; j <= i; j++) 
                { 
                    if (Array[j - 1] > Array[j]) 
                    { 
                        TempValue = Array[j - 1]; 
                        Array[j - 1] = Array[j]; 
                        Array[j] = TempValue; 
                    } 
                } 
            } 
        } 

        #endregion 
    } 
}
```

## Quick Sort

```csharp
namespace SortingAlgorithm.Sorters 
{ 
    public class Quick 
        :ISorter 
    { 
        #region ISorter Members 

        public string Description 
        { 
            get { return "Quick Sort Sıralama Algoritması"; } 
        } 

        public void Execute(int[] Array) 
        { 
            Sort(0, Array.Length - 1,Array); 
        } 

        #endregion 

        private void Sort(int LeftValue, int RightValue,int[] Array) 
        { 
            int PivotValue, LeftHoldValue, RightHoldValue; 

            LeftHoldValue = LeftValue; 
            RightHoldValue = RightValue; 
            PivotValue = Array[LeftValue]; 

            while (LeftValue < RightValue) 
            { 
                while ((Array[RightValue] >= PivotValue) && (LeftValue < RightValue)) 
                { 
                    RightValue--; 
                } 

                if (LeftValue != RightValue) 
                { 
                    Array[LeftValue] = Array[RightValue]; 
                    LeftValue++; 
                } 

                while ((Array[LeftValue] <= PivotValue) && (LeftValue < RightValue)) 
                { 
                    LeftValue++; 
                } 

                if (LeftValue != RightValue) 
                { 
                    Array[RightValue] = Array[LeftValue]; 
                    RightValue--; 
                } 
            } 

            Array[LeftValue] = PivotValue; 
            PivotValue = LeftValue; 
            LeftValue = LeftHoldValue; 
            RightValue = RightHoldValue; 

            if (LeftValue < PivotValue) 
            { 
                Sort(LeftValue, PivotValue - 1,Array); 
            } 

            if (RightValue > PivotValue) 
            { 
                Sort(PivotValue + 1, RightValue,Array); 
            } 
        } 
    } 
}
```

## Insertion Sort

```csharp
namespace SortingAlgorithm.Sorters 
{ 
    public class Insertion 
            : ISorter 
    { 
        #region Sorter Members 

        public string Description 
        { 
            get { return "Insertion Sort Sıralama Algoritması"; } 
        } 

        public void Execute(int[] Array) 
        { 
            int i; 
            int j; 
            int IndexValue; 

            for (i = 1; i < Array.Length; i++) 
            { 
                IndexValue = Array[i]; 
                j = i; 

                while ((j > 0) && (Array[j - 1] > IndexValue)) 
                { 
                    Array[j] = Array[j - 1]; 
                    j = j - 1; 
                } 

                Array[j] = IndexValue; 
            } 
        } 

        #endregion 
    } 
}
```

## Selection Sort

```csharp
namespace SortingAlgorithm.Sorters 
{ 
    public class Selection 
        :ISorter 
    { 
        #region ISorter Members 

        public string Description 
        { 
            get { return "Selection Sıralama Algoritması"; } 
        } 

        public void Execute(int[] Array) 
        { 
            int i, j; 
            int MinValue, TempValue; 

            for (i = 0; i < Array.Length - 1; i++) 
            { 
                MinValue = i; 

                for (j = i + 1; j < Array.Length; j++) 
                { 
                    if (Array[j] < Array[MinValue]) 
                    { 
                        MinValue = j; 
                    } 
                } 

                TempValue = Array[i]; 
                Array[i] = Array[MinValue]; 
                Array[MinValue] = TempValue; 
            } 

        } 

        #endregion 
    } 
}
```

## Shell Sort

```csharp
namespace SortingAlgorithm.Sorters 
{ 
    public class Shell 
       :ISorter 
    { 
        #region ISorter Members 

       public string Description 
        { 
            get { return "Shell Sıralama Algoritması"; } 
        } 

        public void Execute(int[] Array) 
        { 
            int i, j, Increment, TempValue; 

            Increment = 3; 

            while (Increment > 0) 
            { 
                for (i = 0; i < Array.Length; i++) 
                { 
                    j = i; 
                    TempValue = Array[i]; 

                    while ((j >= Increment) && (Array[j - Increment] > TempValue)) 
                    { 
                        Array[j] = Array[j - Increment]; 
                        j = j - Increment; 
                    } 

                    Array[j] = TempValue; 
                } 

                if (Increment / 2 != 0) 
                { 
                    Increment = Increment / 2; 
                } 
                else if (Increment == 1) 
                { 
                    Increment = 0; 
                } 
                else 
                { 
                    Increment = 1; 
                } 
            } 

        } 

        #endregion 
    } 
}
```

## Merge Sort

```csharp
namespace SortingAlgorithm.Sorters 
{ 
    public class Merge 
        :ISorter 
    { 
        int[] Array2; 

        #region ISorter Members 

        public string Description 
        { 
            get { return "Merge Sıralama Algoritması"; } 
        } 

        public void Execute(int[] Array) 
        { 
            Array2 = new int[Array.Length]; 
            Sort(0, Array.Length - 1,Array); 
        } 

        #endregion 
        
        private void Sort(int LeftValue, int RightValue,int[] Array) 
        { 
            int mid; 

            if (RightValue > LeftValue) 
            { 
                mid = (RightValue + LeftValue) / 2; 
                Sort(LeftValue, mid,Array); 
                Sort(mid + 1, RightValue,Array); 

                DoMerge(LeftValue, mid + 1, RightValue,Array); 
            } 
        } 

        private void DoMerge(int LeftValue, int MiddleValue, int RightValue,int[] Array) 
        { 
            int i, LeftEnd, NumberOfElements, TempPosition; 

            LeftEnd = MiddleValue - 1; 
            TempPosition = LeftValue; 
            NumberOfElements = RightValue - LeftValue + 1; 

            while ((LeftValue <= LeftEnd) && (MiddleValue <= RightValue)) 
            { 
                if (Array2[LeftValue] <= Array[MiddleValue]) 
                { 
                    Array2[TempPosition] = Array[LeftValue]; 
                    TempPosition = TempPosition + 1; 
                    LeftValue = LeftValue + 1; 
                } 
                else 
                { 
                    Array2[TempPosition] = Array[MiddleValue]; 
                    TempPosition = TempPosition + 1; 
                    MiddleValue = MiddleValue + 1; 
                } 
            } 

            while (LeftValue <= LeftEnd) 
            { 
                Array2[TempPosition] = Array[LeftValue]; 
                LeftValue = LeftValue + 1; 
                TempPosition = TempPosition + 1; 
            } 

            while (MiddleValue <= RightValue) 
            { 
                Array2[TempPosition] = Array[MiddleValue]; 
                MiddleValue = MiddleValue + 1; 
                TempPosition = TempPosition + 1; 
            } 

            for (i = 0; i < NumberOfElements; i++) 
            { 
                Array[RightValue] = Array2[RightValue]; 
                RightValue = RightValue - 1; 
            } 
        } 
    } 
}
```

## Heap Sort

```csharp
namespace SortingAlgorithm.Sorters 
{ 
    public class Heap 
       :ISorter 
    { 
        #region ISorter Members 

        public string Description 
        { 
            get { return "Heap Sıralama Algoritması"; } 
        } 

        public void Execute(int[] Array) 
        { 
            for (int i=(Array.Length-1)/2; i >= 0;i--) 
                Adjust (Array, i, Array.Length - 1); 
  
            for (int i = Array.Length - 1; i >= 1; i--) 
            { 
                int Temp = Array[0]; 
                Array[0] = Array[i]; 
                Array[i] = Temp; 
                Adjust (Array, 0, i - 1); 
            } 
        } 

        #endregion 

        private void Adjust(int[] Array, int i, int m) 
        { 
            int TempValue = Array[i]; 
            int j = i * 2 + 1; 

            while (j <= m) 
            { 
                if (j < m) 
                    if (Array[j] < Array[j + 1]) 
                        j = j + 1; 

                if (TempValue < Array[j]) 
                { 
                    Array[i] = Array[j]; 
                    i = j; 
                    j = 2 * i + 1; 
                } 
                else 
                { 
                    j = m + 1; 
                } 
            } 

            Array[i] = TempValue; 
        } 
    } 
}
```

Vowwww!!! Ne çok kod, ne çok algoritma, ne çok matematik...

Örneğimizde bir de Performans testini gerçekleştirecek yardımcı bir tipe ihtiyacımız olacaktır. Bu sınıfı da aşağıdaki gibi tasarladığımızı düşünebiliriz.

```csharp
using System; 
using System.Diagnostics; 
using System.IO; 

namespace SortingAlgorithm 
{ 
    public class PerformanceTester 
    { 
        public void Execute(ISorter Sorter, int[] Array) 
        { 
            Stopwatch watcher = new Stopwatch(); 
            watcher.Start(); 

            Sorter.Execute(Array); 

            watcher.Stop(); 

            File.AppendAllText( 
                Path.Combine(Environment.CurrentDirectory, "ExecutionLog.txt") 
                , String.Format("{0} Sıralama Algoritması için Toplam Çalışma Süresi : {1} milisaniyedir. Dizi boyutu {2}|" 
                , Sorter.Description 
                , watcher.ElapsedMilliseconds.ToString() 
                ,Array.Length) 
                ); 
        } 
    } 
}
```

Execute metoduna dikkat edilecek olursa ISorter interface tipinden bir parametre aldığı görülmektedir. Dolayısıyla bu metoda, yukarıda tanımlanmış olan sıralama sınıflarından herhangibiri atanabilir. Çünkü hepsi bu arayüzü (Interface) implemente etmektedir. Diğer yandan Execute metodu, çalışma zamanında (Runtime), Sorter isimli değişkenin büründüğü sıralama sınıfının Execute metodunu yürütmekte ve ayrıca bu sıralama algoritması için bir Text dosyaya log atmaktadır. Metodumuzun önemli özelliklerinden birisi de, Stopwatch tipini kullanarak sıralama işlemi için gerekli çalışma süresi farkını hesaplıyor ve bunu yine Text dosyası içerisine raporluyor olmasıdır.

Uygulamanın Testi

Örnek uygulamamız aslında bir Test uygulamasıdır. Amacı çeşitli sıralama algoritmalarını, içerikleri karışık tamsayılardan oluşan birden fazla boyuttaki dizi için çalıştırmak ve çalışma zamanındaki toplam icra süresi farklarını görmektir. Dolayısıyla bize yardımcı bir kaç fonksiyonellik daha gerekmektedir. Örneğin belirtilen boyutta rastgele int tipinde sayılardan oluşacak bir dizi üretimi fonksiyonelliği ve hatta bu dizinin ham ve sıralanmış hallerinin karşılıklarını yine log amaçlı olarak Text dosyasına yazdıracak bir metod düşünülebilir. Bunun için uygulamamıza Utility isimli static bir tip ilave edebiliriz.

```csharp
using System; 
using System.IO; 
using System.Text; 

namespace SortingAlgorithm 
{ 
    public static class Utility 
    { 
        public static int[] GenerateRandomNumbers(int Size) 
        { 
            int[] numbers = new int[Size]; 
            Random rnd = new Random(); 
            for (int i = 0; i < Size; i++) 
            { 
                numbers[i] = rnd.Next(1, Size); 
            } 

            return numbers; 
        } 

        public static void WriteLog(int[] Array) 
        { 
            StringBuilder builder = new StringBuilder(); 
  
            foreach (int element in Array) 
            { 
                builder.AppendLine(String.Format("{0} ", element.ToString())); 
            } 

            File.AppendAllText( 
                Path.Combine(Environment.CurrentDirectory, "ExecutionLog.txt") 
                , builder.ToString() 
                ); 
        } 
    } 
}
```

GenerateRandomNumbers metodu testler için gerekli n boyutlu int tipinden dizi üretimini üstlenmektedir. Söz konusu kobay diziler rastgele int sayılarından oluşmaktadır. Diğer yandan WriteLog metodu'da int tipinden herhangibir dizinin içeriğini (Sıralı veya değil) ExecutionLog isimli text tabanlı dosyaya yazmaktadır.Buraya kadar yazmış olduğumuz tipleri aşağıdaki sınıf diagramında daha net bir şekilde görebilirsiniz.

[![Sorting_2](/assets/images/2013/Sorting_2_thumb.gif)](/assets/images/2013/Sorting_2.gif)

Artık yazmış olduğumuz tipleri kullanarak ilgili test işlemlerini gerçekleştirecek olan uygulama kodunu geliştirmeye başlayabiliriz.

```csharp
using System; 
using System.IO; 
using SortingAlgorithm.Sorters; 

namespace SortingAlgorithm 
{ 
    class Program 
    { 
        #region Program değişlenleri 

        static PerformanceTester neco = new PerformanceTester(); 
        static Insertion insertion = new Insertion(); 
        static Bubble bubble = new Bubble(); 
        static Heap heap = new Heap(); 
        static Quick quick = new Quick(); 
        static Selection selection = new Selection(); 
        static Shell shell = new Shell(); 
        static Merge merge = new Merge(); 
        static Tuple<int[], int[], int[], int[], int[], int[], int[]> numbers; 

        #endregion 

        static void Main(string[] args) 
        { 
            Console.WriteLine("Testler başladı. Lütfen bekleyiniz"); 

            numbers = Load(100); 
            Execute(numbers); 

            numbers = Load(500); 
            Execute(numbers); 

            numbers = Load(1000); 
            Execute(numbers); 

            numbers = Load(5000); 
            Execute(numbers); 

            numbers = Load(10000); 
            Execute(numbers);

            numbers = Load(50000); 
            Execute(numbers);

            numbers = Load(100000); 
            Execute(numbers);

            numbers = Load(500000); 
            Execute(numbers);

            Console.WriteLine("Testler tamamlandı. {0}"); 

            string Result = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "ExecutionLog.txt")); 
            Console.WriteLine(Result); 
        } 

        private static Tuple<int[], int[], int[], int[], int[], int[], int[]> Load(int MaxValue) 
        { 
            int[] randomNumbers = Utility.GenerateRandomNumbers(MaxValue); 
            numbers = new Tuple<int[], int[], int[], int[], int[], int[], int[]>( 
                    randomNumbers 
                    , (int[])randomNumbers.Clone() 
                    , (int[])randomNumbers.Clone() 
                    , (int[])randomNumbers.Clone() 
                    , (int[])randomNumbers.Clone() 
                    , (int[])randomNumbers.Clone() 
                    , (int[])randomNumbers.Clone() 
            ); 
            return numbers; 
        } 

        private static void Execute(Tuple<int[], int[], int[], int[], int[], int[], int[]> numbers) 
        { 
            neco.Execute(bubble,numbers.Item1); 
            neco.Execute(heap, numbers.Item2); 
            neco.Execute(insertion, numbers.Item3); 
            neco.Execute(merge, numbers.Item4); 
            neco.Execute(quick, numbers.Item5); 
            neco.Execute(selection, numbers.Item6); 
            neco.Execute(shell, numbers.Item7); 
        } 
    } 
}
```

Program kodumuzda kritik olan noktalardan birisi, üretilen 7 adet dizinin Tuple<> tipine yüklenişi sırasında ortaya çıkmaktadır. Diziler bildiğiniz üzere referans tipleridir. Dolayısıyla bir metoda parametre olarak atandıklarında ve içeride değişikliğe uğradıklarında, bu metodun çağırıldığı yerdeki orjinal dizinin içeriğinin de değişmesi söz konusudur. Çünkü her ikiside zaten aynı adresi işaret eden bir pointer'dır. Bu sebepten Tuple<> içerisindeki dizileri set ederken klonlamamız gerekmektedir. Aksi durumda ilk dizinin sıralanmasını takiben diğer metodların kullanacağı tüm dizilerin zaten sıralanmış olarak işleme alındığı görülecektir.

Demek ki basit bir Performans Test uygulaması yazarken dahi çok dikkali davranmalı ve özellikle Debug işlemlerine ağırlık vermeliyiz.

Sonuçlar

Artık uygulamamızı çalıştırabilir ve sonuçları irdeleyebiliriz. Dizilerimize ait değer aralıkları 100, 500, 1000, 5000, 10000, 50000 ve 100000' dir. Çok doğal olarak son değer aralıklarının büyüklüğü nedeni ile uygulama ilgili sıralama algoritmalarını oldukça zorlayacaktır ki bu istediklerimizden birisidir. Ben şahsen uygulamayı denediğimde bir kaç saat çalıştığına şahit oldum. Tabi burada işi yavaşlatan farklı faktörler ve etkenlerde var. Ancak sonuç olarak aşağıdaki test değerlerini elde ettim.

[![Sorting_3](/assets/images/2013/Sorting_3_thumb.gif)](/assets/images/2013/Sorting_3.gif)

Görüldüğü üzere değer aralığı büyüdükçe en hızlı sonuçlar Quick Sort algoritmasından gelmektedir. Diğer yandan Bubble Sort algoritmasının çok fazla maliyet getirdiği ve uzun sürdüğü ortadadır. Heap ve Merge algoritmaları birbirlerine yakın süreler vermiştir. Insertion, Selection ve Shell algoritmaları tatmin edici hızlarda değildir. Tabi buradaki süreler Milisaniye cinsinden olup alsında çok fazla önemli görünmeyebilir. Ancak matematiksel hesaplamaların yoğun yapıldığı bilimsel uygulamalarda sıklıkla kullanıldıkları ve ihtiyaç duyuldukları da bilinmektedir.

Elbette çok daha etkili ve faydalı bir test uygulaması yazılabilir. Mesela [http://www.sorting-algorithms.com](http://www.sorting-algorithms.com) adresinde bunun web tabanlı güzel bir örneği bulunmaktadır. Diğer yandan akılda oluşması gereken önemli sorulardan birisi de şudur. Acaba bu sıralamaların paralelize edilmiş versiyonları var mıdır? Olay sadece Parallel.ForEach veya Parallel.For metodlarını kullanmak kadar basit olabilir mi? Yoksa dikkat edilmesi gereken başka unsurlar da var mıdır? Bu soruların cevaplarını ilerleyen yazılarımızda bulmaya çalışıyor olacağız. Diğer yandan bu yazımızda sadece kullandığımız sıralama algoritmaları hakkında en iyi bilgilere wikipedia üzerinden ulaşabilirsiniz.([Bubble Sort](http://en.wikipedia.org/wiki/Bubble_sort), [Insertion Sort](http://en.wikipedia.org/wiki/Insertion_sort), [Selection Sort](http://en.wikipedia.org/wiki/Selection_sort), [Heap Sort](http://en.wikipedia.org/wiki/Heapsort), [Quick Sort](http://en.wikipedia.org/wiki/Quicksort), [Merge Sort](http://en.wikipedia.org/wiki/Merge_sort), [Shell Sort](http://en.wikipedia.org/wiki/Shell_sort)) Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SortingAlgorithm.rar (40,36 kb)](/assets/files/2013/SortingAlgorithm.rar)