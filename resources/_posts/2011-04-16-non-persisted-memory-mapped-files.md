---
layout: post
title: "Non-Persisted Memory Mapped Files"
date: 2011-04-16 15:00:00 +0300
categories:
  - dotnet-framework-4-0
  - csharp-4-0
tags:
  - dotnet-framework-4-0
  - csharp-4-0
  - csharp
  - dotnet
  - visual-studio
---
Akşam saatleriydi. Sıcak ama güneşin fazla görünmediği bir yaz gününün sonları yaklaşmaktaydı. Gün boyu güneşi İstanbul’ dan saklayan bulutlar yavaş yavaş parçalanmaya başlamıştı. Aralardan güneşin ışınları hafif kızılımsı bir renk ile göğü süslemekteydi. Ancak yine de sert esen rüzgar ve hızla hareket eden bulutlar adeta fırtına öncesi sessizliğin bir habercisiydi. Aile efradı tatildeydi ve çalışma odamda tek başıma dışarıya doğru bakerken geceyi nasıl devam ettirmem gerektiğini planlıyordum.

[![blg202_Giris](/assets/images/2011/blg202_Giris_thumb.jpg)](/assets/images/2011/blg202_Giris.jpg)


Zaman ilerledi ve gecenin zifiri karanlığında güçlü bir gök gürültüsü duyuldu. Belli ki fırtınalı, zor bir gece olacaktı. Kimsenin açık denizde veya havada olmak istemeyeceği bir gece.

Kısa bir süre sonra yağmaya başlayan yağmur ve cama vuran damlaların sesi eşliğinde kendimi bilgisayarımın başında buldum. Odayı kaplayan loş lamba ışığı konstanrasyonu en üst seviye çıkartırken, bilgisayardan gelen hafif müzik nameleri, bloğumun girişi için gerekli cümleleri hazırlamama yardımcı oluyordu. Rota çizilmişti. Geceyi güzel bir blog yazısı ile tamamlamak.

Hatırlayacağınız üzere bir önceki yazımızda.Net Framework 4.0 sürümüne dahil edilen [Memory-Mapped Files](/2010/12/17/persisted-memory-mapped-files/) kavramını incelemeye başlamıştık. İncelememizde ele aldığımız örnekte ise Persisted modeli göz önüne almıştık. Bu modelde bellek üzerine açılan içerikler, fiziki disk üzerinde yer alan dosyalar ile doğrudan ilişkilidir. Yani sanal belleğe açılan görünümler (Views), fiziki disk üzerindeki dosyanın belirli bir bölümü veya tamamıdır.

Bir de Non-Persisted modeli söz konusudur. Bu modelde Sanal Bellek (Virtual Memory) üzerinde oluşturulan bir dosya içeriği söz konusudur. Non-Persisted modelinde de, aynen Persisted modelinde olduğu gibi, bellek içeriklerinin ve buna bağlı olarak oluşturulan görünümlerinin birden fazla Process tarafından ele alınması mümkündür. İşte bu yazımızda söz konusu modeli ele alan basit bir örnek geliştirmeye çalışıyor olacağız. MemoryMappedFile tipinin static CreateNew veya CreateOrOpen metodları, Non-Persisted modeline göre dosya oluşturulmasını sağlamaktadırlar. Elbette birden fazla Process’ in bellek bölgesi üzerindeki aynı içeriğin tamamını veya belirli parçalarını kullanabilmeleri için MemoryMappedViewStream nesne örneklerinden yararlanmaları gerekmektedir. Bir başka deyişle içeriğe ait görünümler (View) ele alınmalıdır.

Örnek çözümümüzde, Visual Studio 2010 Ultimate ortamında geliştirilmiş üç Console uygulaması bulunmaktadır. Bu uygulamalardan ilki bellek üzerinde bir Mapped-File oluşturmakta ve içeriğine örnek veri yazmaktadır. İkinci uygulama da benzer şekilde, aynı bellek bölgesindeki dosyaya farklı bir içerik yazdırmaktadır. Üçüncü uygulama ise Mapped-File içeriğini okumak üzere tasarlanmıştır. Çözümün amacı Virtual Memory üzerine açılmış olan bir Mapped-File ile çalışan üç farklı Process’ in okuma ve yazma işlemlerini icra etmesidir. İlk olarak Application1 isimli örnek uygulama kodlarına bakalım.

```csharp
using System; 
using System.IO.MemoryMappedFiles; 
using CommonLib;

namespace Application1 
{ 
    class Program1 
    { 
        static void Main(string[] args) 
        { 
            Console.Title = "Program 1"; 
            string content = String.Empty; 
            Console.WriteLine("Günün mesajını giriniz"); 
            content = Console.ReadLine();

            using (MemoryMappedFile mappedFile = MemoryMappedFile.CreateNew("mappedImage", 1024)) 
            { 
                byte[] array = null; 
                using (MemoryMappedViewStream view = mappedFile.CreateViewStream()) 
                { 
                    array = Helper.CreateByteArray(content); 
                    view.Write(array, 0, array.Length); 
                } 
                Console.WriteLine("Program 1 {0} byte uzunluğunda içeriği Non Persisted Memory-Mapped dosyaya yazdı.\nProgram 2' yi çalıştırın", array.Length); 
                Console.ReadLine(); 
            } 
        } 
    } 
}
```

Örnek, kullanıcıdan günün anlam ve önemine dair bir mesaj istemektedir. Sonrasında 1024 byte uzunluğunda bir Memory-Mapped File oluşturulmaktadır. Bu oluşturulma işleminin CreateNew metodu ile yapıldığına dikkat edelim. Virutal Memory üzerinde bu Process için açılan bölgeye veri yazmak içinse MemoryMappedViewStream tipine ait bir nesne örneğinden yararlanılmaktadır. Söz konusu nesnenin Write metodu kullanılarak array isimli değişkenin byte içeriği, söz konusu alana yazılmaktadır. Burada CommonLib isimli Class Library içerisinde yer alan Helper static sınıfına ait ve aşağıdaki kod içeriğine sahip CreateByteArray metodundan yararlandığımızı da belirtelim.

```csharp
namespace CommonLib 
{ 
    public static class Helper 
    { 
        public static byte[] CreateByteArray(string content) 
        { 
            char[] charArray=content.ToCharArray(); 
            byte[] byteArray=new byte[charArray.Length];

            for (int i = 0; i < charArray.Length; i++) 
            { 
                byteArray[i] = (byte)charArray[i]; 
            } 
            return byteArray; 
        } 
    } 
}
```

Bu metod string olarak alınan içeriğin her bir karakterinin byte karşılığını topladığı bir diziyi geriye döndürmektedir. Tekrar çözümümüze dönelim. İkinci uygulamanın kod içeriği birincisine oldukça benzerdir.

```csharp
using System; 
using System.IO.MemoryMappedFiles; 
using CommonLib;

namespace Application2 
{ 
    class Program2 
    { 
        static void Main(string[] args) 
        { 
            Console.Title = "Program 2"; 
            string content = String.Empty; 
            Console.WriteLine("Doğum yerinizi giriniz"); 
            content = Console.ReadLine();

            using (MemoryMappedFile mappedFile = MemoryMappedFile.OpenExisting("mappedImage")) 
            { 
                byte[] array=null; 
                using (MemoryMappedViewStream view = mappedFile.CreateViewStream(50,100)) 
                { 
                    array = Helper.CreateByteArray(content); 
                    view.Write(array, 0, array.Length); 
                } 
                Console.WriteLine("Program 2 {0} byte uzunluğunda içeriği Non Persisted Memory-Mapped dosyaya yazdı.\nProgram 3' ü çalıştırın", array.Length); 
                Console.ReadLine(); 
            } 
        } 
    } 
}
```

Bu sefer kullanıcıdan doğduğu yer bilgisi istenmektedir. Ancak bir önceki uygulamadan farklı olarak bu kez, MemoryMappedFile tipinin static OpenExisting metodu kullanılmaktadır. Eğer mappedImage ismiyle işaret edilebilen bir Mapped-File söz konusu ise (ki işaret edilemediği durumda bir Exception söz konusu olacaktır) yine bir MemoryMappedViewStream nesnesi oluşturulmakta ve söz konusu bellek bölgesinin 50nci byte’ ından itibaren 100 byte’ lık olan bölgeye veri yazabilmek için bir kanal oluşturulmaktadır. Yardımcı CreateByteArray metodu ile buraya doğum yeri bilgisi yazdırılmaktadır.

Gelelim 3ncü programın kod içeriğine.

```csharp
using System; 
using System.IO.MemoryMappedFiles;

namespace Application3 
{ 
    class Program3 
    { 
        static void Main(string[] args) 
        { 
            Console.Title = "Program 3"; 
            Console.WriteLine("Program 1 ve 2' yi bekleyiniz"); 
            Console.ReadLine();

            using (MemoryMappedFile mappedFile = MemoryMappedFile.OpenExisting("mappedImage")) 
            { 
                using (MemoryMappedViewStream view = mappedFile.CreateViewStream()) 
                { 
                    byte[] array=new byte[100]; 
                    view.Read(array, 0, 100); 
                    for (int i = 0; i < 100; i++) 
                    { 
                        Console.Write((char)array[i]); 
                    } 
                } 
                Console.WriteLine("Kapatmak için bir tuşa basınız"); 
                Console.ReadLine(); 
            } 
        } 
    } 
}
```

Üçüncü uygulamanın tek bir amacı vardır. O da mappedImage ismi ile eşleşen Mapped-File içeriğinin ilk 100 byte’ lık kısmını okumak. Bu kod içeriklerini geliştirdikten sonra sırasıyla 1nci, 2nci ve 3ncü uygulamaları çalıştırdığımızda, aşağıdaki ekran görüntüsündekine benzer sonuçları elde ettiğimizi görebiliriz.

[![blg202_Runtime](/assets/images/2011/blg202_Runtime_thumb.gif)](/assets/images/2011/blg202_Runtime.gif)

Görüldüğü üzere ilk iki uygulama söz konusu Memory-Mapped File üzerine bir takım byte içeriklerini yazmaktadır. 3ncü program ise ilk iki programın yazdığı verileri başarılı bir şekilde okumaktadır. Çözümümüzde yer alan uygulamaların ne yaptığı aşağıdaki grafik ile temsil edilmeye çalışılmaktadır.

[![blg202_What](/assets/images/2011/blg202_What_thumb.gif)](/assets/images/2011/blg202_What.gif)

Non-Persisted dosyalar fiziki disk üzerinde bir dosyayı işaret etmediklerinden, örnekte üretilen Memory Mapped-File içerikleri, programların kapanması sonrasında doğal olarak yok olacaktır. Bu yok olma işleminde Garbage Collector devreye girmekte ve gerekli çok toplaması işlemlerini üstlenmektedir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[NonPersistedMemoryMappedFiles.rar (80,65 kb)](/assets/files/2011/NonPersistedMemoryMappedFiles.rar) [Örnek Visual Studio 2010 Ultimate sürümünde geliştirilmiş ve test edilmiştir]