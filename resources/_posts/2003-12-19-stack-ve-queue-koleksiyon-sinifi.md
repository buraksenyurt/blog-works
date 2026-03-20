---
layout: post
title: "Stack ve Queue Koleksiyon Sınıfı"
date: 2003-12-19 10:00:00 +0300
categories:
  - csharp
tags:
  - csharp
---
Bugünkü makalemizde Stack ve Queue koleksiyon sınıflarını incelemeye çalışacağız. Bir önceki makalemizde bildiğiniz gibi, HashTable koleksiyon sınıfını incelemeştik. Stack ve Queue koleksiyonlarıda, System.Collections isim alanında yer alan ve ortak koleksiyon özelliklerine sahip sınıflardır. Stack ve Queue koleksiyonları, her koleksiyın sınıfında olduğu gibi, elemanlarını nesne (object) tipinde tutmaktadırlar. Bu koleksiyonların özelliği giren-çıkan eleman prensibleri üzerine çalışmalarıdır. Stack koleksiyon sınıfı, LIFO adı verilen, Last In First Out (Son giren ilk çıkar) prensibine gore çalışırken, Queue koleksiyon sınıfı ise FIFO yani First In First Out (ilk giren ilk çıkar) prensibine gore çalışır.Konuyu daha iyi anlayabilmek için aşağıdaki şekilleri göz önüne alalım.

![mk23_1.gif](/assets/images/2003/mk23_1.gif)

Şekil 1. Stack Koleksiyon Sınıfının Çalışma Yapısı

Görüldüğü gibi, Stack koleksiyonunda yer alan elemanlardan son girene ulaşmak oldukça kolaydır. Oysaki ilk girdiğimiz elemana ulaşmak için, bu elemanın üstünde yer alan diğer tüm elemanları silmemiz gerekmektedir. Queue koleksyion sınıfına gelince;

![mk23_2.gif](/assets/images/2003/mk23_2.gif)

Şekil 2. Queue Koleksiyon Sınıfının Çalışma Yapısı

Görüldüğü gibi Queue koleksiyon sınıfında elemanlar koleksiyona arkadan katılırlar ve ilk giren eleman kuyruktan ilk çıkan eleman olur. Stack ve Queue farklı yapılarda tasarlandıkları için elemanlarına farklı metodlar ile ulaşılmaktadır. Stack koleksiyon sınıfında, en son giren elemanı elde etmek için Pop metodu kullanılır. Koleksiyona bir eleman eklerken Push metodu kullanılır. Elbette eklenen eleman en son elemandır ve Pop metodu çağırıldığında elde edilecek olan ilk eleman halini alır. Ancak Pop metodu son giren elemanı verirken bu elemanı koleksiyondan siler. İşte bunun önüce geçen metod Peek metodudur. Şimdi diyebilirsinizki maden son giren elemanı siliyor Pop metodu o zaman niye kullanıyoruz. Hatırlarsanız, Stack koleksiyonunda, ilk giren elemanı elde etmek için bu elemanın üstünde yer alan tüm elemanları silmemiz gerektiğini söylemiştik. İşte bir döngü yapısında Pop metodu kullanıldığında, ilk giren elemana kadar inebiliriz. Tabi diğer elemanları kaybettikten sonra bunun çok büyük önem taşıyan bir eleman olmasını isteyebiliriz.

Gelelim Queue koleksiyon sınıfının metodlarına. Dequeue metodu ile koleksiyona ilk giren elemanı elde ederiz. Ve bunu yaptığımız anda eleman silinir. Nitekim dequeue metodu pop metodu gibi çalışır. Koleksiyona eleman eklemek için ise, enqueue metodu kullanılır. İlk giren elemanı elde etmek ve silinmemesini sağlamak istiyorsak yine stack koleksiyon sınıfında olduğu gibi, Peek metodunu kullanırız. Bu koleksiyonların en güzel yanlarından birisi size leman sayısını belirtmediğiniz takdirde koleksiyonun boyutunu otomatik olarak kendilerinin ayarlamalarıdır. Stack koleksiyon sınıfı, varsayılan olarak 10 elemanlı bir koleksiyon dizisi oluşturur.(Eğer biz eleman sayısını yapıcı metodumuzda belirtmez isek). Eğer eleman sayısı 10’u geçerse, koleksiyon dizisinin boyutu otomatik olarak iki katına çıkar. Aynı prensib queue koleksiyon sınıfı içinde geçerli olmakla birlikte, queue koleksiyonu için varsayılan dizi boyutu 32 elemanlı bir dizidir. Şimdi dilerseniz, basit bir console uygulaması ile bu konuyu anlamaya çalışalım.

```csharp
using System;
using System.Collections; /* Uygulamalarımızda koleksiyon sınıflarını kullanabilmek için Collections isim uzayını kullanmamız gerekir.*/

namespace StackSample1
{
    class Class1
    {
        static void Main(string[] args)
        {
            Stack stc =new Stack(4); /* 4 elemanlı bir Stack koleksiyonu oluşturduk.*/
            stc.Push("Burak");
            /*Eleman eklemek için Push metodu kullanılıyor.*/
            stc.Push("Selim");
            stc.Push("ŞENYURT");
            stc.Push(27);
            stc.Push(true);
            Console.WriteLine("Çıkan eleman {0}", stc.Pop().ToString());
            /* Pop metodu son giren(kalan) elemanı verirken, aynı zamanda bu elemanı koleksiyon dizisinden siler.*/
            Console.WriteLine("Çıkan eleman {0}", stc.Pop().ToString());
            Console.WriteLine("Çıkan eleman {0}", stc.Pop().ToString());
            Console.WriteLine("------------------");
            IEnumerator dizi = stc.GetEnumerator();
            /* Koleksiyonın elemanlarını IEnumerator arayüzünden bir nesneye aktarıyoruz.*/
            while (dizi.MoveNext()) /* dizi nesnesinde okunacak bir sonraki eleman var olduğu sürece işleyecek bir döngü.*/
            {
                Console.WriteLine("Güncel eleman {0}", dizi.Current.ToString());
                /* Current metodu ile dizi nesnesinde yer alan güncel elemanı elde ediyoruzç. Bu döngüyü çalıştırdığımızda sadece iki elemanın dizide olduğunu görürüz. Pop metodu sağolsun.*/
            }
            Console.WriteLine("------------------");
            Console.WriteLine("En üstteki eleman {0}", stc.Peek());
            /* Peek metodu son giren elemanı veya en üste kalan elemanı verirken bu elemanı koleksiyondan silmez.*/
            dizi = stc.GetEnumerator();
            while (dizi.MoveNext())
            {
                Console.WriteLine("Güncel eleman {0}", dizi.Current.ToString());
                /* Bu durumda yine iki eleman verildiğini Peek metodu ile elde edilen elemanın koleksiyondan silinmediğini görürüz.*/
            }
        }
    }
}
```

![mk23_3.gif](/assets/images/2003/mk23_3.gif)

Şekil 3. Stack ile ilgili programın çalışmasının sonucu.

Queue örneğimiz ise aynı kodlardan oluşuyor sadece metod isimleri farklı.

```csharp
using System;
using System.Collections;

namespace QueueSample1
{
    class Class1
    {
        static void Main(string[] args)
        {
            Queue qu =new Queue(4);
            qu.Enqueue("Burak");
            /*Eleman eklemek için Enqueue metodu kullanılıyor.*/
            qu.Enqueue("Selim");
            qu.Enqueue("ŞENYURT");
            qu.Enqueue(27);
            qu.Enqueue(true);
            Console.WriteLine("Çıkan eleman {0}", qu.Dequeue().ToString());
            /* Dequeue metodu ilk giren(en alttaki) elemanı verirken, aynı zamanda bu elemanı koleksiyon dizisinden siler.*/
            Console.WriteLine("Çıkan eleman {0}", qu.Dequeue().ToString());
            Console.WriteLine("Çıkan eleman {0}", qu.Dequeue().ToString());
            Console.WriteLine("------------------");
            IEnumerator dizi = qu.GetEnumerator();
            /* Koleksiyonın elemanlarını IEnumerator arayüzünden bir nesneye aktarıyoruz.*/
            while (dizi.MoveNext()) /* dizi nesnesinde okunacak bir sonraki eleman var olduğu sürece işleyecek bir döngü.*/
            {
                Console.WriteLine("Güncel eleman {0}", dizi.Current.ToString());
                /* Current metodu ile dizi nesnesinde yer alan güncel elemanı elde ediyoruzç. Bu döngüyü çalıştırdığımızda sadece iki elemanın dizide olduğunu görürüz. Dequeue metodu sağolsun.*/
            }
            Console.WriteLine("------------------");
            Console.WriteLine("En altta kalan eleman {0}", qu.Peek());

            /* Peek metodu son giren elemanı veya en üste kalan elemanı verirken bu elemanı koleksiyondan silmez.*/
            dizi = qu.GetEnumerator();
            while (dizi.MoveNext())
            {
                Console.WriteLine("Güncel eleman {0}", dizi.Current.ToString());
                /* Bu durumda yine iki eleman verildiğini Peek metodu ile elde edilen elemanın koleksiyondan silinmediğini görürüz.*/
            }
        }
    }
}
```

![mk23_4.gif](/assets/images/2003/mk23_4.gif)

Şekil 4. Queue ile ilgili programın çalışmasının sonucu.

Geldik bir makalemizin daha sonuna. Umuyorumki sizlere faydalı olabilecek bilgiler sunabilmişimdir. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.