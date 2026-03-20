---
layout: post
title: "Barrier Class, Sıralama Algoritmaları ve At Yarışı"
date: 2011-10-17 09:47:00 +0300
categories:
  - parallel-programming
tags:
  - parallel-programming
  - csharp
  - dotnet
  - http
  - generics
---
At yarışlarına pek ilgim yoktur aslında ama tam da bu günlerde okuduğum kitap nedeniyle, paralel programlama ile aralarında sıkı bir ilişki olduğunu ifade edebilirim

[![The starting gate](/assets/images/2011/The%20starting%20gate_thumb.jpg)](/assets/images/2011/The%20starting%20gate.jpg)


![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_70.png)

Bildiğiniz üzere bu yarışların pek çok meraklısı bulunmaktadır. Özellikle yarışları stadyumdan seyredenler oldukça heyecanlıdır. Gerçi yarışın başlamasından önceki tahminler tam başlangıç anında yerini endişeye bırakır. Bizim gibi yazılımcılar için önemli olan ise başlangıç anıdır. Neden mi? Bazen bir yarışa başlanırken, yarışa iştirak eden katılımcıların start düzlüğünde bir arada yer almalarını bekleriz. At yarışlarındaki başlangıç kapıları bu işe yaramaktadır. Tahmin edeceğiniz gibi bu kapılar atların başlangıç işareti gelmeden hareket etmelerini engellemek üzere tasarlanmışlardır.

Çok doğal olarak ve pek tabi ki aynı durum program ortamı içerisinde yer alan ve paralel çalıştırılması düşünülen metodlar içinde geçerli olabilir

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_70.png)

Bir başka deyişle bazı senaryolarda paralel olarak yürütülmesi istenen metodların, çalışmaya başlamadan önce bir kutu içerisinde de paralel olarak dizilmeleri ve kutu beklenen limiti aştığında başlatılmaları istenebilir. Yani paralel çalıştırılması istenen fonksiyonellikler hazırlanıp belirli bir başlangıç noktasına doğru senkronize edilir ve sonrasında belirli bir kurala göre (örneğin kutunun limitini doldurması gibi) başlatılırlar.

Durumu daha iyi analiz edebilmek için dilerseniz basit bir senaryo üzerinden ilerlemeye çalışalım. Malumunuz günümüz üniversitelerinde özellikle algoritma derslerinde en çok sorulan, okutulan, ezberletilen konuların başında sıralama teknikleri gelmektedir. Bubble Sort, Quick Sort, Insertion Sort vb…Eminim çoğumuz bunları ezberlemek ve sınavlarda çıkacak diye uzun geceler boyu hazırlanmak zorunda kalmışızdır

![Kafası karışmış gülümseme](/assets/images/2011/wlEmoticon-confusedsmile_10.png)

Peki ya bu sıralama algoritmalarını bir yarışa sokmak ister miydiniz? Aslında her birini bir at olarak düşünüp aynı anda yarışı başlatmak istemez miydiniz dersem çok daha uygun olacaktır

![Gülümseme](/assets/images/2011/wlEmoticon-smile_19.png)

Bunu gerçekleştirirken aslında paralel programlama tekniklerinden yararlanma şansımız bulunmaktadır. Her bir sıralama algoritması için birer Task nesne örneği üretip bunları aynı anda çalıştırmamız yeterli olacaktır…Aynı anda

![Açık ağızlı gülümseme](/assets/images/2011/wlEmoticon-openmouthedsmile_17.png)

İşte Barrier sınıfını kullanmak için güzel bir fırsat. Yazımızın odak noktası hangi sıralama algoritmasının hızlı çalıştığını tespit etmekten ziyade, Barrier sınıfını kullanarak ilgili sıralama algoritma metodlarını aynı anda başlatmaktır. Bu amaçla ben örnek olarak 3 adet sıralama algoritmasını baz almayı uygun gördüm. Bubble Sort, Quick Sort ve Insertion Sort…

Hedefimiz bu algoritmaları içeren metodları birer Task olarak tanımlamak ve start düzlüğünde aynı kapı içerisine yerleştirerek aynı anda çalıştırılmalarını sağlamaktır. Kabaca Barrier sınıfını kullanarak gerçekleştireceğimiz örneğimizin çalışma zamanındaki akışı belkide aşağıdaki grafik ile ifade edilebilir.

[![bei_30](/assets/images/2011/bei_30_thumb.gif)](/assets/images/2011/bei_30.gif)

Barrier sınıfı öncellikli olarak bir kapasite bildirimi ile çalışmaktadır. Bu kapasite aslında kutu içerisine dahil edilecek olan Task sayısını bildirmektedir. Söz konusu Task sayısına ulaşıldığında ilgili metodların aynı anda başlatılması söz konusu olacaktır (Tabi arkadan gelen başka bir Task daha olursa, kapıdaki örnekler bir anda çıkana kadar beklemek zorunda kalacaktır) Durumu elbetteki örnek kod parçası üzerinden anlamamız daha uygundur. Bu amaçla basit bir Console uygulaması açıp aşağıdaki kod içeriğini yazarak ilerleyebiliriz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Threading; 
using System.Threading.Tasks;

namespace SortingTest 
{ 
    class Program 
    { 
        static string insertionSortStartTime=String.Empty; 
        static string quickSortStartTime = String.Empty; 
        static string bubbleSortStartTime = String.Empty;

        static void Main(string[] args) 
        { 
            // Önce rastgele sayı koleksiyonumuz oluşturuluyor 
            List<int> testCollection = CreateRandomNumbers(100000,1, 100000);

            #region Barrier Kullanımı

            // 3 metodluk bir başlangıç kutusu tanımlıyoruz. 
            Barrier _barrier = new Barrier(3);

            // Bubble Sort metodu için bir Task üretiliyor ve Task ilk olarak Barrier içerisine ilave ediliyor 
            Task bubbleSortingTask = new Task( 
                () => 
                { 
                    _barrier.SignalAndWait(); 
                    BubbleSorting(testCollection); 
                } 
            ); 
            bubbleSortingTask.Start();

            // Quick Sort metodu için bir Task üretiliyor ve Task içerisinde yine Barrier' e bir ekleme işlemi gerçekleştiriliyor 
            Task quickSortingTask = new Task( 
                () => 
                { 
                    _barrier.SignalAndWait(); 
                    QuickSorting(testCollection, 0, testCollection.Count - 1); 
                } 
            ); 
            quickSortingTask.Start();

            // Insertion Sort metodu için bir Task üretiliyor ve Barrier içerisine ekleniyor 
            Task insertionSortingTask = new Task( 
                () => 
                { 
                   _barrier.SignalAndWait(); 
                    InsertionSorting(testCollection); 
                } 
            ); 
            insertionSortingTask.Start();

            // Task' lerin tamamlanması uzun sürebilir o yüzden bekleyelim 
            Task[] tasks = { bubbleSortingTask, quickSortingTask, insertionSortingTask }; 
            Task.WaitAll(tasks);

            Console.WriteLine("Bubble : {0}\nQuick : {1}\nInsertion : {2}",bubbleSortStartTime,quickSortStartTime,insertionSortStartTime);

            #endregion

            #region Sequential çalıştırma

            bubbleSortStartTime = String.Empty; 
            quickSortStartTime = String.Empty; 
            insertionSortStartTime = String.Empty;

            Console.WriteLine("\nSequential Çalıştırma\n"); 
            // Bubble algoritmasına göre sıralama 
            var bubbleOrdered = BubbleSorting(testCollection); 
            // Quick sort algoritmasına göre sıralama 
            var quickOrdered = QuickSorting(testCollection, 0, testCollection.Count - 1); 
            // Insertion sort algoritmasına göre sıralama 
            var insertionOrdered = InsertionSorting(testCollection);

            Console.WriteLine("Bubble : {0}\nQuick : {1}\nInsertion : {2}", bubbleSortStartTime, quickSortStartTime, insertionSortStartTime);

           #endregion 
        }

        /// <summary> 
        /// Bubble sıralama algoritmasına göre sayı dizisini sıralar 
        /// </summary> 
        /// <param name="Numbers">Sıralanacak olan sayı koleksiyonu</param> 
        /// <returns>Parametre olarak gelen Numbers koleksiyonunun sıralanmış halidir</returns> 
        static List<int> BubbleSorting(List<int> Numbers) 
        { 
            if (String.IsNullOrEmpty(bubbleSortStartTime)) 
                bubbleSortStartTime = DateTime.Now.ToLongTimeString();

            for (int i = 0; i < Numbers.Count - 1; i++) 
            { 
                for (int j = 1; j < Numbers.Count - i; j++) 
                { 
                    if (Numbers[j] < Numbers[j - 1]) 
                    { 
                        int temporary = Numbers[j - 1]; 
                        Numbers[j - 1] = Numbers[j]; 
                        Numbers[j] = temporary; 
                    } 
                } 
            }

            return Numbers; 
        }

        /// <summary> 
        /// QuickSort veya Pivot Sort algoritmasına göre sıralama işlemini yapan metoddur. 
        /// </summary> 
        /// <param name="Numbers">Sıralanacak olan sayı koleksiyonu</param> 
        /// <param name="LeftValue">Sol değer</param> 
        /// <param name="RightValue">Sağ değer</param> 
        /// <returns>Sayı koleksiyonunun sıralanmış hali</returns> 
        static List<int> QuickSorting(List<int> Numbers, int LeftValue, int RightValue) 
        { 
            if (String.IsNullOrEmpty(quickSortStartTime)) 
                quickSortStartTime = DateTime.Now.ToLongTimeString();

            int i = LeftValue; 
            int j = RightValue; 
            double pivotValue = ((LeftValue + RightValue) / 2); 
            int x = Numbers[Convert.ToInt32(pivotValue)]; 
            int w = 0; 
            while (i <= j) 
            { 
                while (Numbers[i] < x) 
                { 
                    i++; 
                } 
                while (x < Numbers[j]) 
                { 
                    j--; 
                } 
                if (i <= j) 
                { 
                    w = Numbers[i]; 
                    Numbers[i++] = Numbers[j]; 
                    Numbers[j--] = w; 
                } 
            } 
            if (LeftValue < j) 
            { 
                QuickSorting(Numbers, LeftValue, j); 
            } 
            if (i < RightValue) 
            { 
                QuickSorting(Numbers, i, RightValue); 
            } 
            
            return Numbers; 
        }

        /// <summary> 
        /// InsertionSort algoritmasına göre sayı koleksiyonunu sıralar 
        /// </summary> 
        /// <param name="Numbers">Sıralanacak olan sayı koleksiyonu</param> 
        /// <returns>Sayı koleksiyonunun sıralanmış hali</returns> 
        static List<int> InsertionSorting(List<int> Numbers) 
        { 
            if (String.IsNullOrEmpty(insertionSortStartTime)) 
                insertionSortStartTime = DateTime.Now.ToLongTimeString();

            for (int j = 1; j < Numbers.Count; j++) 
            { 
                int keyValue = Numbers[j]; 
                int i = j - 1; 
                while (i >= 0 && Numbers[i] > keyValue) 
                { 
                    Numbers[i + 1] = Numbers[i]; 
                    i = i - 1; 
                } 
                Numbers[i + 1] = keyValue; 
            }

            return Numbers; 
        }

        /// <summary> 
        /// Sıralama testlerine tabi tutulacak sayı koleksiyonunu üretir 
        /// </summary> 
        /// <param name="NumberCount">Kaç adet sayıdan oluşacak</param> 
        /// <param name="StartValue">Rastgele sayı üretici için minimum değer</param> 
        /// <param name="EndValue">Rastgele sayı üretici için maksimum değer</param> 
        /// <returns>Oluşan sayı koleksiyonu</returns> 
        static List<int> CreateRandomNumbers(int NumberCount,int StartValue, int EndValue) 
        { 
            List<int> collection = new List<int>(); 
            Random rnd = new Random(); 
            for (int i = 0; i < NumberCount; i++) 
            { 
                collection.Add(rnd.Next(StartValue,EndValue)); 
            } 
            return collection; 
        } 
    } 
}
```

Aslında uygulamamız çok karmaşık gözükse de oldukça basit temellere dayanıyor. Sıralama algoritmalarının üçü için ve bir de rastgele sayı üretimi için tasarlanmış metodlarımız bulunmaktadır. Asıl önemli olan kısım ise Main metodu içerisinde yaptıklarımızdır. Dikkat edileceği üzere ilk olarak Barrier nesne örneği kullanılarak bir akış gerçekleştirilmektedir.

Barrier sınıfına ait nesne örneği üretildikten sonra, ilgili Task örneklerinin buraya dail olmaları için, anonymous metodlarda SignalAndWait fonksiyonuna birer çağrıda bulunulduğuna dikkat edelim

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_70.png)

Sonrasında Task nesne örneklerinin Start metodları çağırılıyor. Ne var ki Barrier bloğunun kapasitesi dolana kadar ilgili metodlar yürütülmeyecektir. Zaten söz konusu Task örneklerinin Barrier bloğuna dahil edilme sebebi de kapasite dolumundan sonra aynı anda çalışmaya başlamalarının istenmesidir. Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsündekine benzer sonuçlar aldığımızı görebiliriz.

[![bei_31](/assets/images/2011/bei_31_thumb.gif)](/assets/images/2011/bei_31.gif)

Görüldüğü üzere Barrier kullanımı nedeni ile ilk üç Task örneğinin tam olarak aynı anda başlatılmaları söz konusudur (Aslında bakarsanız milisaniye cinsinden de kontrol edilmeleri gerekmektedir) Diğer yandan Sequential çalışma da elbetteki metodlar başlatıldıkları sıra ile yürütülmektedirler. Burada Quick Sort algoritmasının gerçekten çok hızlı olması nedeni ile işini çok kısa sürede bitirdiğini ve hemen Insertion Sort metoduna geçilebildiğini vurgulamak isterim.

Sanırım Barrier tipinin kullanımını biraz olsun kavrayabilmişizdir. Konuyu daha net kavrayabilmek için uygulamayı Debug modda çalıştırmanızı öneririm. Özellikle Sort metodlarının başlangıç noktalarına Breakpoint koyarsanız, Main metodu içerisinde ilgili Task nesne örnekleri üzerinden Start metodları çağırılsa bile Barrier bloğunun kapasitesi dolana kadar bu breakpoint noktalarına gelinemediğini görüyor olacaksınız. Burada eğer Debug Mod’ da Parallel Tasks penceresine bakarsanız Task örneklerinin Start metodlarına yapılan çağrılardan sonra Status olarak Running’ e geçtikleri görülecektir ki bu sizi yanıltmamalıdır. Bu sebepten örneği Debug modda izlemeniz son derece önemlidir

![Göz kırpan gülümseme](/assets/images/2011/wlEmoticon-winkingsmile_70.png)

Bakın ben çalışma zamanında durumu analiz ettiğimde Barrier bölgesine son eklenen Insertion Sort algoritma metodundan sonra, bu son eklenen ilk olmak üzere işleyişin başlayabildiğin gördüm. Bir başka deyişle Barrier için belirtilen 3 kapasite değeri InsertionSort metodu eklenene kadar dolmadığından hiç bir metod başlatılmamıştır.

[![bei_32](/assets/images/2011/bei_32_thumb.gif)](/assets/images/2011/bei_32.gif)

[![240496b](/assets/images/2011/240496b_thumb.jpg)](/assets/images/2011/240496b.jpg) Parallel programlama ile ilişkili olarak bir kaç yazı yazmayı daha planlamaktayım. İçeride gözden kaçan oldukça önemli ve hayati pek çok konu var. Özellikle de gelecek nesil.Net platformunda bu konunun ne kadar önemli olduğunu düşünürsek geliştirici olarak iyi hazırlanmamız gerektiği kanısındayım.

Aslında ben şu sıralarda [Parallel Programming Step by Step](http://amzn.com/0735640602) isimli kitabı takip ediyorum. İnce bir kitap ve sizlere de şiddetle tavsiye ederim. Gerçi ince olduğuna aldanmayın lütfen. Yeteri kadar doyurucu bilgi ve kod örneğini barındırmakta. Eğer paralel programlamaya ilgi duyuyorsanız tabi

![Gülümseme](/assets/images/2011/wlEmoticon-smile_19.png)

Buraya kadar sabırla okuduğunuz için teşekkür ederken tekrardan görüşünceye dek hepinize mutlu günler dilerim. [SortingTest.rar (29,66 kb)](/assets/files/2011/SortingTest.rar)