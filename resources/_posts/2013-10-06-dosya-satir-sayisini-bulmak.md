---
layout: post
title: "Dosya Satır Sayısını Bulmak"
date: 2013-10-06 12:02:00 +0300
categories:
  - csharp
tags:
  - csharp
  - linq
  - sql-server
  - windows-forms
  - parallel-programming
  - performance
  - reflection
  - pointers
  - generics
  - visual-studio
---
Eğer sizde zamanında benim gibi bankaların teknoloji departmanlarında çalışmış ve yazılım geliştirmişseniz, eminimki hayatınızın bir döneminde büyük boyutlu Text dosyaları ile çalışmak zorunda kalmışsınızdır.

[![1370555_lots_of_files_2](/assets/images/2013/1370555_lots_of_files_2_thumb.jpg)](/assets/images/2013/1370555_lots_of_files_2.jpg)


Malum Bankaların sistemleri halen daha eski olabildiğinden, bölümler arası veya uygulamalar arası veri aktarmanın en popüler yollarından birisi olarak Text tabanlı dosya formatları göz önüne alınmaktadır. Bazen onlarca megabyte'ı aşan ve milyonlarca satırdan oluşabilen düzenli text dosyaları söz konusu olur ve bunların bir şekilde uygulamaların konuştuğu veritabanı ortamlarına işlenerek, ilişkisel veri bütünlüğü içerisinde yerlerini alması beklenir.

Vaka

Aktarım işlemleri sırasında çoğunlukla SSIS (Sql Server Integration Services) paketlerinden yararlanılmaktadır veya ekstra 3ncü parti araçlar kullanılır. Lakin bazen kendi uygulamalarımız içerisinde bu tip dosyaların kod yardımıyla ayrıştırılması ve işlenmesi de gerekebilir. Böyle bir halde ise son kullanıcının durumdan haberdar edilmesi ve özellikle işlem çok uzun sürecekse bir Progress Bar bileşeni ile (En azından Windows Forms tarafı için) anlık ilerleme durumunun gösterilmesi uygun olabilir. Tabi anlık durumun bir dosya için gösterilmesi söz konusu ise, dosyanın toplam satır sayısının da bilinmesi gerekecektir.

Hımmm…Bir dosyanın toplam satır sayısı nasıl bulunabilir peki? Bunun için aklımıza gelecek ve uygulayabileceğimiz bir çok yol bulunmakta. Ancak hangisinin daha efektif olduğunu bir şekilde tespit etmemiz ve görmemiz önemlidir. Dolayısıyla bu yazımızda, bir text dosyanın satır sayısını bulmak için kullanabileceğimiz metodlardan bazılarını ve bu fonksiyonların toplam çalışma sürelerini hesaplatacağız. Tabi alacağımız sonuçlar sistemden sisteme farklılık gösterebilirler.

Hazırlıklar

İlk olarak operasyonel işlemleri üstlenen tipimizi ve test kodumuzu geliştirip üzerinde kısaca konuşalım. Uygulamamıza ait sınıf diagramı ve kod içeriği aşağıdaki gibidir.

[![diagram](/assets/images/2013/diagram_thumb.png)](/assets/images/2013/diagram.png)

```csharp
using System; 
using System.Diagnostics; 
using System.IO; 
using System.Linq; 
using System.Reflection; 
using System.Text.RegularExpressions;

namespace FindLineCountsApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            // Satır sayısı hesap edilecek olan dosya adresi alınır 
            string file = Path.Combine(Environment.CurrentDirectory, "Content.txt");

            // Hesaplama işlemlerini üstlenen tipimiz 
           FileProcessor prcsr=new FileProcessor(file);

            // Reflection' dan yararlanarak, FileProcessor tipi içerisinde LineTest niteliği ile işaretlenmiş ne kadar metod varsa çağırıyor ve her birinin hesaplama sürelerini buluyoruz. 
            Type t = prcsr.GetType(); 
            foreach (var method in t.GetMethods()) 
            { 
               if (method.GetCustomAttributes(false).Length == 1) 
                { 
                    var lAttr = (method.GetCustomAttributes(false)[0]) as LineTestAttribute; 
                    if (lAttr != null) 
                    { 
                        // Kronometremizi her metod çağrısı içerisinde oluşturuyoruz 
                        Stopwatch watcher = new Stopwatch(); 
                       watcher.Start(); // Kronometre start 
                        var result = method.Invoke(prcsr, BindingFlags.InvokeMethod, null, null, null); 
                            // Metod çağırılıyor 
                        watcher.Stop(); // Kronometre stop 
                        Console.WriteLine("Method {0}\tLineCount {1}\tDuration {2} milisaniye", method.Name,  result.ToString(), watcher.ElapsedMilliseconds.ToString()); 
                    } 
                } 
            } 
        } 
    }

    class FileProcessor 
    { 
        private string _fileName;

        public FileProcessor(string fileName) 
        { 
            _fileName = fileName; 
        }

        // Hesaplama metodlarımızdan birisi File tipinin ReadAllLines metodunu kullanarak dönen dizinin uzunluğuna bakar 
        [LineTest] 
        public int Compute1() 
        { 
            int lineCount = 0;

            lineCount=File.ReadAllLines(_fileName).Length;

            return lineCount; 
        } 
        // Dosya baştan sona okunurken Alt Satıra geçme karakteri araştırılır '\n' bu -1 olmadığı sürece yani dosya sonuna gelinmediği sürece satır sayısı ve pozisyon arttırılarak devam edilir. 
       [LineTest] 
        public int Compute2() 
        { 
            int lineCount = 0; 
            int position = 0; 
            while ((position = File.ReadAllText(_fileName).IndexOf('\n', position)) != -1) 
            { 
                lineCount++; 
                position++; 
            } 
            return lineCount+1; 
        }

        // RegEx ifadesinden yararlanılarak \n' lerin sayısı hesaplanır. 
        [LineTest] 
        public int Compute3() 
        { 
            int lineCount = 0;

            Regex regEx = new Regex("\n", RegexOptions.Multiline); 
            MatchCollection matchCollection = regEx.Matches(File.ReadAllText(_fileName)); 
            lineCount=matchCollection.Count;

            return lineCount+1; 
        }

        // Bu sefer \n karakterlerinin tespiti için Linq sorgusundan yararlanılmaktadır. 
        [LineTest] 
        public int Compute4() 
        { 
            int lineCount = 0;

            lineCount=(from ch in File.ReadAllText(_fileName) 
             where ch == '\n' 
             select ch).Count();

            return lineCount+1; 
        }

        // Bu kez dosya içeriği bir char dizisine atanır ve tüm dizide dönülerek \n' lerin toplam sayısı bulunur 
        [LineTest] 
        public int Compute5() 
        { 
            int lineCount = 0; 
            string text = File.ReadAllText(_fileName); 
            int posMax = text.Length; 
            char[] a = text.ToCharArray();

            for (int position = 0; position < posMax; ) 
                if (a[position++] == '\n') 
                    lineCount++; 
            return lineCount+1; 
        }

        // Bu seferki metodumuzda dosyanın uzunluğundan, dosya içerisinde new line karakterlerinin kaldırılması sonucu oluşan içeriğin uzunluğunu çıkartarak hesaplama yaptırıyoruz 
        [LineTest] 
        public int Compute6() 
        { 
            int lineCount = 0;

            string text = File.ReadAllText(_fileName); 
            lineCount=text.Length - text.Replace("\n", "").Length;

            return lineCount+1; 
        }

        // Count Extension Method' unun kullanılması ve new line' ların sayılarının bulunarak satır sayısının hesap edilmesi işlemini üstlenir 
        [LineTest] 
        public int Compute7() 
        { 
            int lineCount = 0;

            lineCount=File.ReadAllText(_fileName).Count(c => (c == '\n'));

            return lineCount+1; 
        }

        // Bu sefer dosyanın her bir karekteri foreach döngüsü ile dolaşılıyor ve new line karakterlerinin sayısı hesap ediliyor 
        [LineTest] 
        public int Compute8() 
        { 
            int lineCount = 0;

            foreach (char c in File.ReadAllText(_fileName)) 
                if (c == '\n') 
                    lineCount++;

            return lineCount+1; 
        } 
    }

    // Satır sayısını hesaplama ile ilişkili test metodlarımızı belirtmek amacıyla basit bir attribute tanımlıyoruz 
    [AttributeUsage(AttributeTargets.Method)] 
    class LineTestAttribute 
        :Attribute 
    { 
    } 
}
```

Kod Ne Yapıyor?

FileProcessor isimli sınıfımız ComputeX isimli 8 adet metod içermektedir. Her bir metod yorum satırlarında belirtildiği üzere, ilgili dosyanın satır sayısını bulmak için farklı bir yöntem kullanılmaktadır. Constructor (Yapıcı Metod) teste tabi tutulacak olan dosya adını alır.

Dikkat edilmesi gereken noktalardan birisi de, ComputeX metodlarının LineTest isimli bir nitelik (Attribute) ile imzalanmış olmalarıdır. Bu niteliği metodları çalışma zamanında test için çağırırken kullanacağımız Reflection bazlı kodlarda değerlendireceğiz. (Nitekim her test metodunu Main içerisinde arka arkaya yazıp, her biri için süre hesabını o kod satırlarında yazıp, satır sayısını arttırmak istemiyorum. Maintability ve Readability açısından biraz daha yüksek bir kod olsun niyetindeyim)

Main metodu içerisinde dikkat edileceği üzere Reflection’ dan yararlanılarak FileProcessor tipinin üye metodları arasında gezilmekte ve LineTest niteliği uygulanmış olanların çağırılması sağlanmaktadır.

Testler

Teste tabi tutacağımız text dosyası içerisinde Lorem Ipsum metinlerinden bolca yer almaktadır. İlk testler için dosyamızda 10891 satır yer alıyor. Buna göre ilk sonuçlar aşağıdaki gibidir.

[![runtime2](/assets/images/2013/runtime2_thumb.png)](/assets/images/2013/runtime2.png)

Şu hemen dikkatinizi çekmiş olmalıdır. Compute2 metodu inanılmaz derecede yavaştır. En hızlı metod ise Compute1 olmuştur ki içerisinde File tipinin static ReadAllLines metodunu kullandığımızı belirtmek isterim. Bu sonuçlara göre Compute2 metodunu doğrudan elemeliyiz. İkinci testimizde bu metodu göz ardı edeceğiz. Şimdi dosyanın satır sayısını belirgin ölçüde arttırdığımızı düşünelim. Örneğin 152487 satır olsun.

Çok mu sizce? Ben milyonları gördüğüm için bana normal geliyor aslında. Gerçi bu durumu Visual Studio bile yadırgadı. 50 Mb’ a yaklaşan boyutuyla Text dosyasını sadece Notepad++ ile açabildim. Neyse tekrar konumuza dönelim. Amacımız boyutun artması halinde, hesaplama metodlarının aynı performans istikrarını sağlayıp sağlamadığını görebilmek. İşte sonuçlar;

[![runtime3](/assets/images/2013/runtime3_thumb.png)](/assets/images/2013/runtime3.png)

Compute2 metodunu hariç tuttuğumuzda 1nci ve 2nci testlerin sonuçlarını aşağıdaki tablo grafiğinde görüldüğü gibi yorumlayabiliriz.

[![report](/assets/images/2013/report_thumb.png)](/assets/images/2013/report.png)

Sonuçlar

Görüldüğü üzere sonuçlar gayet açık ve net. File.ReadAllLines metodunun eline pek su döken olmadı. Ancak yaklaşabilenler var. Peki bu metod kendi içerisinde nasıl bir çalışma modeline sahipte bu kadar hızlı sonuç döndürülmesine olanak sağlıyor?

```csharp
private static string[] InternalReadAllLines(string path, Encoding encoding)

{ 
    List<string> list = new List<string>(); 
    using (StreamReader reader = new StreamReader(path, encoding)) 
    { 
       string str; 
        while ((str = reader.ReadLine()) != null) 
        { 
            list.Add(str); 
        } 
   } 
    return list.ToArray(); 
}
```

Dikkat edileceği üzere ReadAllLines metodu aslında içeride başka bir metodu tetiklemekte (InternalReadAllLines). Bu metod içerisinde StreamReader'dan yararlanılarak bir okuma işlemi yapılmakta ve satırların generic bir List koleksiyonunda toplanması sağlanmakta. Söz konusu metod, generic list koleksiyonun Array'e indirgenmiş halini döndürmektedir. Sonrasında zaten bu dönüş dizisinin eleman sayısını bulmamız yeterlidir.

Tabi satır sayısını bulmak için alternatif yollarda düşünebilir ve mutlaka vardır. Söz gelimi Pointer kullanımı daha hızlı sonuçlar alabilmemizi sağlayabilir. Ya da paralel programlama desenlerinden yararlanarak özellikle çok büyük boyutlu bir dosyanın parçalara bölünmek suretiyle satır sayısının hesaplanması ve [Reduction](https://www.buraksenyurt.com/post/Parallel-Programming-Reduction.aspx) ile kümülatif toplamın ortaya konulması düşünülebilir. Ancak önerilen yol çok büyük boyutlu dosyalarda File.ReadAllLines metodunun kullanılmasıdır. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[FindLineCountsApp.zip (32,62 kb)](/assets/files/2013/FindLineCountsApp.zip)