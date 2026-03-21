---
layout: post
title: "En Kısa Metni Bulmak"
date: 2013-09-01 20:45:00 +0300
categories:
  - data-structures-algorithms
tags:
  - csharp
  - string
  - extension-methods
  - np-algorithms
---
Uzun zamandır makale yazmaya çalışmakta ve öğrendiklerimi, edindiğim tecrübeleri sizlere aktarmaktayım. Tabi zaman ilerledikçe yazacak konu bulmakta da bir hayli zorlanıyor insan. Bu noktada öğrenmeninin sınırının olmadığını hepimiz biliyoruz. Olaya bu açıdan baktığımızda yazılmaya ve araştırılmaya değer binlerce konu olduğunu gönül rahatlığıyla ifade edebilirim. Yazma hevesli bir birey olarak bu benim için gerçekten önemli

![205735_getting_the_last_word.jpg](/assets/images/2013/205735_getting_the_last_word.jpg)

![Smile](/assets/images/2013/smiley-smile.gif)

İşte geçtiğimiz hafta içerisinde de Internet üzerinden araştırma yaparken enteresan bir konu ile karşılaştım. Aslında konuyu isimlendirmek oldukça zor ama bir optimizasyon işlemi olduğunu ifade edebilirim. Sorun n sayıda kelimenin saklanmak istendiği bir durumda ortaya çıkıyor. İstenen, bu kelimeleri birleştirerek saklamak ancak bunu yaparkende olabilecek en kısa cümleyi elde ederek ilgili depolama işlemini gerçekleştirmek. Öyleki, üretilen cümle hem çok kısa olmalı hem de tüm kelimeleri içermeli.

Dilerseniz önce sorunu ve ulaşmak istediğimiz hedefi bir örnek üzerinden ifade etmeye çalışalım. Elimizde 4 adet kelime olduğunu farz edelim. Enginar, Arpa, Keten ve Paket. Normal şartlarda bu 4 kelimeyi sırasız olarak birleştirirsek aşağıdaki gibi bir string katarının oluştuğunu görürüz.

EnginarArpaKetenPaket

Bu son derece doğal bir sonuç tabi

![Wink](/assets/images/2013/smiley-wink.gif)

Ancak amacımız bu metni mümkün olduğu kadar kısaltmak. Peki bunu nasıl yapabiliriz?

Aslına bakarsanız bir şekilde kelimeler arasındaki ilişkileri matematiksel olarak anlamlandırmak ve olabilecek en iyi kombinasyonları seçerek ilerlemeye çalışmak yerinde olacaktır. Bunu düşünerek Paket ile Keten kelimelerini göz önüne alalım.

![ko_1.png](/assets/images/2013/ko_1.png)

Dikkat edileceği üzere paket ile keten kelimelerini yan yana getirdiğimizde, KET hecesinin ortak olmasından dolayı paketen şeklinde daha kısa bir birleşim elde etmiş durumdayız. Ancak önce keten sonra paket kelimesini yan yana getirirsek, bu durumda bir kısaltma söz konusu olmayacaktır.

![ko_2.png](/assets/images/2013/ko_2.png)

Dolayısıyla paket ile keten kelimeleri arasındaki ilişkiyi sayısal olarak anlamlandırmaya çalıştığımızda şunları söyleyebiliriz.

- Keten Paket sırası düşünüldüğünde arada hiç ortak birleşim harfi veya hecesi yoktur. Dolayısıyla Keten kelimesinden Paket kelimesine geçişinin değeri 0 olarak nitelendirilebilir.
- Paket Keten sırasına baktığımızda ise, ket hecesinin ortak olduğu bir durum söz konusudur. ket hecesi teke indirgendiğinde, iki kelime birleşimi önemli ölçüde kısaltılmış olmaktadır. Buna göre Paket kelimesinden Keten kelimesine geçişin maliyeti 3 olarak düşünülebilir (3 harfli bir kısım kesilmiş olduğu için)

Diğer kelimeleri de göz önüne alarak devam edelim. İlk olarak Arpa ile Paket'e bir bakalım. Yukarıdaki tekniği göz önüne alacak olursak aşağıdaki görsellerde yer alan ilişkileri kurabiliriz.

![ko_3.png](/assets/images/2013/ko_3.png)

Buna göre paket -> arpa geçişinin değeri 0 iken, arpa -> paket geçişinin değeri pa hecesinin değişmesi nedeniyle 2 olarak hesaplanabilir.

Şimdi de Enginar, Arpa ve Keten kelimeleri arasındaki ilişkiye bir bakalım. Çünkü bu 3 kelime arasında aynı değerlere sahip bir ilişki durumu söz konusudur. Aşağıdaki şekilde bu durum ifade edilmeye çalışılmıştır.

![ko_4n.png](/assets/images/2013/ko_4n.png)

Burada enginar -> arpa birleşimi ile keten -> enginar birleşimlerinin sayısal ağırlıkları aynıdır (2). Karar vermek çok önemli değildir aslında. Nitekim ağırlık puanları eşittir ve iki birleşiminde tüm sözcük dizimine etkisi aynı olacaktır. Elimizde bulunan 4 kelimeyi ve aralarındaki ilişkileri düşündüğümüzde aşağıdaki şekilde olduğu gibi bir ağırlıklandırma yapabiliriz.

![ko_5.png](/assets/images/2013/ko_5.png)

Sanırım bu şekil yardımıyla kelimeler arasındaki ilişkileri daha net görebilmekteyiz. Biraz renkli oldu ama olsun

![Wink](/assets/images/2013/smiley-wink.gif)

Sonuç olarak aşağıdaki iki kelime katarından birisini üretebiliriz.

arpaketenginar

veya

enginarpaketen

![ko_6.png](/assets/images/2013/ko_6.png)

Herşey iyi güzel. Kafamızda kelimeleri bir şekilde birbirlerine bağladık. Peki ama bunun kodlamasını nasıl yapacağız?

![Undecided](/assets/images/2013/smiley-undecided.gif)

Sonuçta en önemli nokta aslında bu sıkıştırma şeklini bir şekilde kod tarafında üretebilmemizdir. İşe ufak bebek adımları ile başlamakta yarar var. Örneğin çok temel bir genişletme metodu (Extension Method) ile iki string'i ağırlık derecesine göre uygun bir biçimde birleştirmeyi düşünelim. İşte kod parçamız.

```csharp
using System;
using System.Linq;
using System.Collections.Generic;

namespace EnKisaCumle
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] words = { "Paket", "KETEN", "EngiNar", "arPA","demir" };

            Console.WriteLine("{0} ile {1} için {2}",words[0],words[1], words[0].Combine(words[1]));
            Console.WriteLine("{0} ile {1} için {2}",words[0], words[2], words[0].Combine(words[2]));
            Console.WriteLine("{0} ile {1} için {2}", words[2], words[3], words[2].Combine(words[3]));
            Console.WriteLine("{0} ile {1} için {2}", words[3], words[0], words[3].Combine(words[0]));

        }                
    }

    public static class Extensions
    {
        public static string Combine(this string LeftWord,string RightWord)
        {
            int weight = FindWeight(LeftWord, RightWord);
            string result =string.Format("{0}{1}", LeftWord, RightWord.Substring(weight, RightWord.Length - weight));
            return result;
        }

        private static int FindWeight(string wordLeft, string wordRight)
        {
            int maxLength = wordLeft.Length < wordRight.Length ? wordLeft.Length : wordRight.Length;
            for (int i = 0; i < maxLength; i++)
            {
                if (wordLeft.EndsWith(wordRight.Substring(0, maxLength - i), true, null)) // büyük küçük harf ayrımı yapmasın
                {
                    return maxLength - i;
                }
            }

            return 0;
        }
    }
}
```

Uygulamamızda basit bir extension metod ile string birleştirme işlemi gerçekleştirilmektedir. Buna göre örneğin sonucu aşağıdaki gibi olacaktır.

![artcl_13_1.png](/assets/images/2013/artcl_13_1.png)

Görüldüğü üzere ağırlık derecelerine göre bazı kelimelerin birleşimi daha kısa olurken bazıları ise arka arkaya gelmektedir, nitekim ağırlıkları 0 dır.

Yazmış olduğumuz bu temel fonksiyon iki kelime işin içerisinde olduğunda işe yaramaktadır. Lakin kelime dizisinin tamamının ele alınması biraz daha farklı bir durumdur. Daha fazla kod ve daha karmaşık bir algoritma gerekmektedir. Yaptığım araştırmalar sonucunda aşağıdaki gibi bir kod parçasını toparlamayı başarabildim.

![ko_8.png](/assets/images/2013/ko_8.png)

Sınıf diagramında (Class Diagram) da görüleceği üzere, kelimeler arası ilişkileri tasvir edebilmek adına bir Node listesini modelemeye çalışıyoruz. Bu Node listesi, bir kelime ve buna bağlı ne kadar diğer kelime kombinasyonu varsa, ağırlıkları ile birlikte cover etmeye çalışmaktadır. Diğer yandan Node listesini kullanarak uygun olabilecek bir birleşimi üretme fonksiyonelliğini, Creator isimli tip karşılamaktadır. Kodun biraz karmaşık olduğunu biliyorum. Lakin bu konu ile ilişkili olarak yaptığım araştırmalarda, en basite indirgeyebildiğim sıkıştırma algoritması bu oldu. İnanın bu anlamda ele alınan diğer algoritmalar (NP Algoritmaları özellikle) inanılmaz derecede karışık geliyor bana

![Sealed](/assets/images/2013/smiley-sealed.gif)

Ama yılmak yok! Lafı fazla uzatmadan kodumuzu paylaşalım.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnKisaCumle
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] words = { "Paket", "KETEN", "EngiNar", "arPA" };

            Creator graph = new Creator(new List<string>(words));
            Console.WriteLine(graph.CompressWords());
        }
    }

    // Yardımcı olacak genişletme metodlarımız
    public static class Extensions
    {
        // İki kelimeyi ağırlıkları mertebesinde birleştirir.
        public static string Combine(this string LeftWord, string RightWord)
        {
            int weight = FindWeight(LeftWord, RightWord);
            string result = string.Format("{0}{1}", LeftWord, RightWord.Substring(weight, RightWord.Length - weight));
            return result;
        }

        // İki kelimenin birleşme ağırlık değerini hesaplar
        public static int FindWeight(this string LeftWord, string RightWord)
        {
            int maxLength = LeftWord.Length < RightWord.Length ? LeftWord.Length : RightWord.Length;
            for (int i = 0; i <= maxLength; i++)
            {
                if (LeftWord.EndsWith(RightWord.Substring(0, maxLength - i), true, null)) // büyük küçük harf ayrımı yapmasın
                {
                    return maxLength - i;
                }
            }
            return 0;
        }

        // Belirtilen Sequence koleksionu üzerindeki her bir elemanda Action temsilcisi ile belirtilen fonksiyonelliğin çalıştırılmasını sağlar
        public static void Run<T>(this IEnumerable<T> Sequence, Action<T> Action)
        {
            foreach (var item in Sequence)
                Action(item);
        }

        // Reduce metodu içerisinde devreye girer
        public static IEnumerable<T> With<T>(this IEnumerable<T> Sequence, T Item)
        {
            foreach (var t in Sequence)
                yield return t;
            
            yield return Item;
        }

        // Bir kelime kombinasyonu içerisindeki Node ve Parent' ı arasındaki birleşimi üretir ve yeni bir Node olarak elde etmemizi sağlar
        public static Node<string, int> Combine(this Combination<string, int> Combination)
        {
            return new Node<string, int>(Combination.Parent.Value.Combine(Combination.Node.Value));
        }
    }

    // Bir kelime ve bu kelime ile diğerleri arasındaki ilişkiler ile ağırlıkları tutan temel tipimizdir
    public class Node<T, W> 
    {
        private List<Combination<T, W>> combinations = new List<Combination<T, W>>();
        Func<Node<T, W>, Node<T, W>, W> weightCalculator;

        public Func<Node<T, W>, Node<T, W>, W> WeightCalculator
        {
            get
            {
                if (weightCalculator == null)
                    weightCalculator = (n1, n2) => default(W);
                return weightCalculator;
            }
            set
            {
                weightCalculator = value;
            }
        }
        public List<Combination<T, W>> Combinations
        {
            get
            {
                return combinations;
            }
            set
            {
                combinations = new List<Combination<T, W>>(value);
            }
        }
        public T Value { get; private set; }
        public Node(T value)
        {
            Value = value;
        }

        public void Add(Node<T, W> node)
        {
            var combination = new Combination<T, W>(this, node, WeightCalculator(this, node));
            combinations.Add(combination);
        }
    }

    // Kelime kombinasyonlarını Parent ve Current Node bazında ağırlıkları ile birlikte saklar.
    public class Combination<T, W>
    {
        public Node<T, W> Parent { get; private set; }
        public Node<T, W> Node { get; private set; }
        public W Weight { get; private set; }

        public Combination(Node<T, W> Parent, Node<T, W> Node, W Weight)
        {
            this.Parent = Parent;
            this.Node = Node;
            this.Weight = Weight;
        }
    }

    // Asıl işlemleri üstlenen tipimiz. Kelimelere ait node listesine yeni örnekler eklenmesi, sıkıştırma yapılması, olası kombinasyonların azaltılması gibi fonksiyonellikleri üstlenir.
    public class Creator
    {
        List<Node<string, int>> nodes = new List<Node<string, int>>();
        Func<Node<string, int>, Node<string, int>, int> weightCalculator;

        public Creator()
        {
            weightCalculator = (node, newNode) =>
            {
                return node.Value.FindWeight(newNode.Value);
            };
        }

        public Creator(List<string> Words)
        {
            Words.Run(Add);
        }

        public List<Node<string, int>> Nodes
        {
            get
            {
                return nodes;
            }
            set
            {
                nodes = new List<Node<string, int>>(value);
            }
        }

        public void Add(string Value)
        {
            if (!nodes.Exists(n => n.Value == Value))
            {
                var newNode = new Node<string, int>(Value)
                {
                    WeightCalculator = weightCalculator
                };

                nodes.Run(node =>
                {
                    newNode.Add(node);
                    node.Add(newNode);
                });

                nodes.Add(newNode);
            }
        }

        public Creator With(string value)
        {
            Add(value);
            return this;
        }

        public Creator Reduce()
        {
            if (nodes.Count <= 1)
                return this;

            var combinations = from n in nodes
                        from e in n.Combinations
                        select e;
            var combination = combinations.First(n => n.Weight == combinations.Max(e => e.Weight));

            return new Creator(nodes.Select(n => n.Value)
                                        .Where(str => str != combination.Parent.Value && str != combination.Node.Value)
                                        .With(combination.Combine().Value).ToList());
        }

        public string CompressWords()
        {
            Creator graph = this;
            while (graph.nodes.Count > 1)
            {
                graph = graph.Reduce();
            }

            return graph.nodes[0].Value;
        }
    }
}
```

Örneğimizi çalıştırdığımızda aşağıdaki gibi bir sonuç elde ederiz.

![ko_9.png](/assets/images/2013/ko_9.png)

Görüldüğü üzere kelimeleri istediğimiz şekilde birleştirebilidik.

Uygulama kodunu kavrayabilmek adına Debug ederek adım adım ilerlemenizi öneririm. Lakin içeride makaleyi yazdığım tarih itibariyle halen çözemediğim bazı bug'lar bulunmakta. Söz gelimi kelime dizimize 5nci bir içeriği eklediğimizi düşünelim. "DEMİR". Aşağıdaki sonucu elde ederiz.

![ko_10.png](/assets/images/2013/ko_10.png)

Başarılı bir birleştirme işlemi yapılmış gibi görünüyor değil mi? Aslında biraz daha dikkat edersek, hiç bir kelime ile ortak notkası olmayan DEMİR sözcüğünün en sona veya en başa alınmasının daha doğru olduğunu görebiliriz. Çünkü bu durumda

EngiNarPAketENDEMİR veya DEMİREngiNarPAKetEN

sonuçlarını elde edebiliriz. Yani PA hecesinin teke indirgenmesi söz konusudur. Dolayısıyla kodun gözden geçirilmesi, algoritmanın temizlenerek en uygun sonucun elde edilmesi için gerekli müdahalelerin yapılması gerekmekte. Burada iş biraz da sizlere düşüyor. Her ne kadar mükemmel şekilde çalışan bir algoritma olmasa da, kendi adıma kelime sıkıştırma konusunda epey bir fikir verdiğini, en azından kağıt üzerinde kelimeler arası ilişkilerin bulunması noktasında nasıl hareket edilebileceğini hem kendi adıma hem de değerli okurlarım adına anlamış bulunmaktayım. Tabi önmeli bir eksiğimiz daha var. Sıkıştırılan metni nasıl geri çözümleyeceğimiz. Buna bir çözüm getirebilir misiniz?

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[EnKisaCumle.zip (59,15 kb)](/assets/files/2013/EnKisaCumle.zip)
