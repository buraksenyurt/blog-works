---
title: "Çerezlik Algoritmalar ve Extension Methodlar"
pubDate: 2012-06-07 23:30:00
categories:
  - C#
  - Data Structures, Algorithms
tags:
  - C#
  - Data Structures, Algorithms
  - LINQ
  - HTTP
  - Generics
---

# Çerezlik Algoritmalar ve Extension Methodlar
![artcl_8_1.jpg](images/artcl_8_1.jpg)

Merhaba Arkadaşlar,

Akademik yıllarımızda çoğumuz karmaşık matematik algoritmaları ile uğraşmak durumunda kalmışızdır (Sınav stresini hatırlamak bile istemiyorum) Özellikle veri yapıları ve algoritmalar (Data Structures and Alogirthms) veya Numeric Analiz gibi derslerde yoğun algoritma tasarımları üzerinde çalışılmaktadır. Doğruyu söylemek gerekirse ülkemizde bu dersleri layıkıyla veren kurum sayısı oldukça azdır. Konular genellikle sırlama algoritmalarının (özellikle Quick Sort'un) ötesine pek geçmemektedir. En fazla yüksek lisans öğreniminde farklı konulara girilmesi söz konusudur.

Pek tabi yazılım dünyası söz konusu olduğunda var olan hemen her algoritmanın karşılığı olan kodlamaların geliştirilmesi de önemli bir mevzudur. Bilimsel uygulamalarda, finansal model çözümlerinde, endusturi alanındaki planlama tekniklerinde vb...Ben bu yazımda sizleri o karmaşık ve anlaşılması zor algoritmalar ile yormayacağım. Bunun yerine eğilenceli sayılabilecek ve özellikle oyun programlamada oyunculara keyifli dakikalar yaşatmanızı sağlayabilecek basit bir kaç algoritma üzerinde durmaya çalışacağım. Söz konusu algoritmaları birer Extension Method olarak geliştireceğiz. Dilerseniz hiç vakit kaybetmeden ilk algoritmamız ile işe başlayalım.

![image.axd](images/image.axd)

Biraz eskilere gidiyor olacağız. Hatta Roma imparatoru Ceaser zamanına

![Smile](images/smiley-smile.gif)

Ceaser, Roma imparatorluğunun şaşalı dönemlerinde generalleri ile haberleşirken basit bir şifreleme metodolojisini kullanmaktaymış. Büyük bir ihtimalle sonradan Ceaser Cheaper olarak adlandırılan bu algoritmanın çalışma şekli aslında son derece basitmiş. Algoritmaya göre bir cümleyi veya metni, alfabe üzerinde belirlenen sayı kadar sağa (ileri) veya sola (geri) doğru ötelenmek suretiyle karşılık gelen harfler ile dizmek söz konusudur. Sonuçta ortaya, okunabilirliği pek olmayan anlamsız bir veri çıkmaktadır ama generaller tarafından bu, merkez sayı noktasına göre tekrardan geriye doğru ötelenerek anlamlı hale getirilebilir. Elbetteki bizim gerçek hayat uygulamalarımızda kullanacağımız bir şifreleme algoritması değildir bu. Ancak basit bir zeka oyununda neden kullanılmasın ki. Eğlenceli olabilir

![Wink](images/smiley-wink.gif)

(Algoritma hakkında detaylı bilgilere [bu adresten](http://en.wikipedia.org/wiki/Caesar_cipher) ulaşabilirsiniz.) Haydi gelin bu algoritma için bir genişletme metodu yazalım.

```csharp
public static class AlgorithmExtensions
{
	#region Ceaser Cheaper ile karıştırma

	public static string CaesarChiper(this string Word, int ShiftNumber)
	{
		char[] chars = Word.ToCharArray();

		for (int i = 0; i < chars.Length; i++)
		{
			char currentLetter = chars[i];
			currentLetter = (char)(currentLetter + ShiftNumber);
			
			if (currentLetter > 'z')
				currentLetter = (char)(currentLetter - 26);
			else if (currentLetter < 'a')
				currentLetter = (char)(currentLetter + 26);
		
			chars[i] = currentLetter;
		}
		return new string(chars);
	}

	#endregion
}
```

Aslında algoritma oldukça basit gördüğünüz gibi. Girilen Shift değerine göre ASCII tablosu üzerinden sağa veya sola doğru hareket ediliyor. z ve a aralığında bir öteleme hareketi yapıldığına dikkat edelim. Öteleme noktalarında elde edilen karakterler ardışıl olarak dizilerekten metnin karıştırılmış hali geriye döndürülüyor. Bir Extension Method olarak yazdığımız için herhangibir String değişken üzerinden uygulanabilir. Peki nasıl kullanacağız?

![Wink](images/smiley-wink.gif)

Gelin aşağıdaki kod parçası ile ilerleyelim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Ceaser Chipper Test

            string word = "sol kanattan saldırın";
            Console.WriteLine("Cümle : {0}\n",word);
            Console.WriteLine("{0}\n",word.CaesarChiper(15));
            Console.WriteLine("{0}\n", word.CaesarChiper(-15));
            Console.WriteLine("{0}\n", word.CaesarChiper(6));
            Console.WriteLine("{0}\n", word.CaesarChiper(10));

            #endregion

        }
    }
}
```

Örnek uygulamamızda girilen cümlenin Shift değerine göre farklı çıktılarının üretildiği görülecektir. İlk seferde 15 karakter sağa gidilerek işe başlanırken ikinci denemede 15 karakter sola gidilmek suretiyle bir üretim söz konusudur. Aşağıdaki ekran çıktısında çalışma zamanındaki durum net bir şekilde görülmektedir.

![artcl_8_2.jpg](images/artcl_8_2.jpg)

Dikkat edileceği üzere eğlenceli görünen karmaşık veri içerikleri üretilmiş durumda. Tabi bunu çözümlemeye çalışmak oyuncunun işi olacak. Oldukça fazla zorlanacağından emin olabilirsiniz.

![Wink](images/smiley-wink.gif)

![image.axd](images/image.axd)

Ceaser'ın hakkına Ceaser'a verip ve Ceaser'a elveda diyerek yolumuza devam edelim. Sırada yer alan algoritmamız ise Fisher-Yates Shuffle olarak literatürde yer almaktadır. Bu algoritma yardımıyla bir sayı veya kelime dizisinin ya da farklı bir veri kümesinin her defasında farklı olacak şekilde karıştırılarak elde edilmesi söz konusudur. Bir başka deyişle farklı permütasyonların hesap edilerek bir karma veri içeriği üretilmesi gibi bir durum mevcuttur. (1938 yılında keşfedilmiş olan bu algoritma hakkında ki detaylı bilgileri yine [wikipedia](http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle) adresi üzerinden elde edebilirsiniz.) Biz hiç vakit kaybetmeden bu algoritma için bir extension metod geliştirerek ilerleyelim.

```csharp
using System;
using System.Collections.Generic;

public static class AlgorithmExtensions
{
    #region Fisher-Yates ile Shuffling

    public static void Shuffle<T>(this List<T> SourceArray) // Fisher-Yates Shuffle Algoritmasına göre karıştırır
    {
        Random randomizer = new Random();

        for (int i = SourceArray.Count; i > 1; i--)
        {
            int j = randomizer.Next(i); // 0 ile i-1 arasında rastgele bir değer üretecektir
            T temp = SourceArray[j];
            SourceArray[j] = SourceArray[i - 1];
            SourceArray[i - 1] = temp;
        }
    }

    #endregion
}
```

Görüldüğü gibi algoritmamız kaynak olan dizi içerisindeki elemanları sondan başa doğru gezmektedir. Bu işlem sırasında o anki iterasyon değerine kadarki aralıkta üretilen bir rastgele sayı ve bunun dizideki karşılığı olan değer geçici bir değişkene atanır. Ardından o anki iterasyonun bir önceki değerine karşılık gelen dizi elemanı rastgele elde edilen değerin işaret ettiği indise taşınır. Son olarak da geçici değişkene alınan eleman o anki iterasyon değerinin bir öncesine karşılık gelen indise yerleştirilir. Genişletme metodunu generic olarak tasarladığımızı ve herhangibir List tipine uygulayabildiğimizi fark etmişsinizdir. Temel olarak hedefimiz sayısal veya metin tabanlı koleksiyon listelerinin karıştırılmış çıktılarını elde etmektir (ki bir Puzzle uygulamasında bu teknik oldukça işe yarayabilir ![Wink](images/smiley-wink.gif)) Metodun kalbinde Random tipi yer almaktadır. Metodumuzu aşağıdaki kod parçasında olduğu gibi test sürüşüne çıkartabiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Fisher - Yates Shuttle Alogirtması

            List<int> numbers = Enumerable.Range(0, 50).ToList();
            List<string> names = new List<string> {
                "bill","steve","daniel","meg","julia"
                ,"sunny","Matheus","chris","lora"
                ,"maggi","jim","steve","Emilie"
                ,"Maria","eva","samantha" 
            };

            numbers.Shuffle();
            WriteToConsole(numbers);

            numbers.Shuffle();
            WriteToConsole(numbers);

            names.Shuffle();
            WriteToConsole(names);

            names.Shuffle();
            WriteToConsole(names);

            #endregion
        }

        private static void WriteToConsole<T>(List<T> SourceArray)
        {
            foreach (var element in SourceArray)
            {
                Console.Write("{0} ", element);
            }
            Console.WriteLine("\n\n");
        }
    }
}
```

Test kodunda string tipte isimlerden oluşan bir List koleksiyonu ve benzer şekilde sayılardan oluşan bir veri içeriği söz konusudur. Uygulamanın çalışma zamanı çıktısı ise aşağıdakine benzer olacaktır.

![artcl_8_3.jpg](images/artcl_8_3.jpg)

Size tavsiyem basit bir fotoğrafı n sayıda kareye böldükten sonra, bu parçaları işaret eden sınıfa ait nesne örneklerinden oluşan bir List koleksiyonunu, Fisher-Yates Shuffle algoritmasını kullanarak, oyuncuyu her seferinde farklı bir karmaşa ile baş başa bırakmayı denemeniz olacaktır

![Smile](images/smiley-smile.gif)

![image.axd](images/image.axd)

Geldik bu yazımızda size aktarmak istediğim son algoritmaya. Bu kez Palindromik veri tespiti yapmak için kullanılan bir algoritma üzerinde duracağız. Palindromic sayılar 181, 191, 55 gibi tersten okunduklarında da aynı sayı değerini veren kavramlar olarak düşünülebilirler. Aslında Palindromic sayılar olarak düşünebileceğimiz bu modeli kelimeler için de ele alabiliriz. ANA, KAÇAK gibi örnekler bu anlamda düşünülebilir. Senaryo olarak baktığımızda ise, söz gelimi metin içerikli bir döküman içerisinde yer alan Palindromic kelimeleri veya cümleleri oyuncuya buldurabilir ve süre bazında başarısını ölçümlemeye çalışabiliriz. (Bu algoritma ile ilişkili detaylı bilgileri yine [Wikipedia](http://tr.wikipedia.org/wiki/Palindrom) üzerinden okuyabilirsiniz.) Ancak öncesinde algoritma için gerekli olan genişletme metodumuzu yazalım.

```csharp
using System;
using System.Collections.Generic;

public static class AlgorithmExtensions
{
    #region Palindromic Kelimeleri/Cümleleri Bulmak

    public static List<string> PalindromicCheck(this List<string> Words)
    {
        List<string> result = new List<string>();

        foreach (string word in Words)
        {
            if (IsPalindromicData(word))
                result.Add(word);
        }

        return result;
    }

    private static bool IsPalindromicData(string SourceValue)
    {
        int minValue = 0;
        int maxValue = SourceValue.Length - 1;
        while (true)
        {
            if (minValue > maxValue)
                return true;
            
            char a = SourceValue[minValue];
            char b = SourceValue[maxValue];
            
            if (char.ToLower(a) != char.ToLower(b))
                return false;
            
            minValue++;
            maxValue--;
        }
    }

    #endregion
}
```

Sonsuz while döngüsü aslında kelimenin başından ve sonundan itibaren orta noktasına gelinceye kadar bir iterasyonu kullanmaktadır. İlk karakter ve son karakterin aynı olup olmadığı noktasında devreye giren algoritma orta noktadaki karaktere kadar devam edecektir. Genişletme metodumuz bu versiyonu ile string tipinden List koleksiyonuna uygulanabilecek şekilde tasarlanmıştır. Koleksiyon içerisindeki veriler, IsPalindromicData metodu yardımıyla kontrol edilmektedir. Metod sonuç olarak Palindromic veri içeriğini barındıran başka bir List koleksiyonunu geriye döndürür. Haydi gelin örneğimizi test edelim. Bu amaçla aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Palindromik kelimelerin tespiti

            List<string> words = new List<string>{
                "ana","baba","amaç","abla","aha","kaçak"
                ,"kayak","kırık","cam","uzak","yakın","meşale"
                ,"neden","sus","süs","'ey edip adanada pide ye'"
            };

            var result = words.PalindromicCheck();
            WriteToConsole(result);

            #endregion
        }

        private static void WriteToConsole<T>(List<T> SourceArray)
        {
            foreach (var element in SourceArray)
            {
                Console.Write("{0} ", element);
            }
            Console.WriteLine("\n\n");
        }
    }
}
```

Örnekte kullanılan koleksiyon içerisinde tertsten okunduklarında da aynı olan bazı kelimeler ve hatta bir cümle mevcuttur. İşte çalışma zamanı sonuçları.

![artcl_8_4.jpg](images/artcl_8_4.jpg)

Bu algoritmayı oyuncudan ziyade oyun motoru kullanıyor olabilir. Ya da siz geniş bir kelime kümesini ekrana basıp bu küme içerisindeki Palindromic kelimeleri tespit etmesi için henüz ilk okul çağında olan bir oyuncuyu tercih edebilir ve süre bazlı bir ortam sağlayarak, onun dikkat, kavrama, fark etme, görsel hafıza gibi yeteneklerini arttırmaya çalışabilirsiniz

![Wink](images/smiley-wink.gif)

Aslında oyun programlama denilince çok basit ve yararlı algoritmalar olduğunu görebiliyoruz. Ben bu yazımızda sadece 3 tanesini sizlere aktarmaya çalıştım. Elbetteki çok daha fazlası var. Araştırmak, denemek, öğrenmek, test etmek ve kullanmak sizin göreviniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Wink](images/smiley-wink.gif)
