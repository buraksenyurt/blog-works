---
layout: post
title: "Persisted Memory-Mapped Files"
date: 2010-12-17 10:30:00 +0300
categories:
  - dotnet-framework-4-0
  - csharp-4-0
tags:
  - dotnet-framework-4-0
  - csharp-4-0
  - csharp
  - linq
  - caching
  - generics
  - visual-studio
---
Sene 1997. Üniversite 3ncü sınıf öğrencisiyim. Eskiden lisanslı bir basketbolcu olan uzun boylu arkadaşım Serkan ve Babası ile birlikte Tepebaşındaki bilgisayar fuarındayız. Serkan, kendisine bir bilgisayar almak istiyor. Tabi Baba’ sının gelmesinin nedeni işlemci mimarilerini, ram teknolojilerini çok iyi bilmesi değil. Tamamen duygusal:$ Ben de Serkan arkadaşıma teknik olarak destek verip önerilerde bulunuyorum. Derken 3müz arasında şöyle bir konuşma geçiyor;

[![blg201_Giris](/assets/images/2010/blg201_Giris_thumb.jpg)](/assets/images/2010/blg201_Giris.jpg)


Burak: Serkan bak Pentium 200 MMX var. Creative CD sürücü. Üstelik uzaktan kumandalı. Diamond Stealth ekran kartı 2 mb. İyidir. Seagate Hard Disk. Süper.

Serkan: Abi kaç ram var bunda?

Burak: Abi iki model var. Biri 32 Mb diğeri 64 mb. Ben derim ki 64 Mb olanı al.

Serkan: Baba bu 64 Mb olanı alalım.

Baba: Kaç dolar fark var arada?

Serkan: X dolay fark var Baba.

Baba: 32 Mb olanı alalım.

Serkan: Ama baba, 64 Mb olan daha iyi. Uygulamalar için daha çok yer tahsis ediyor. Daha rahat çalışacak uygulamalar. Aynı anda daha çok uygulama çalışacak.

Baba: Sen önce 32 Mb olanı doldur, sonrasına bakarız:S

Bu minik hikayeden sonra şu an geldiğimiz noktaya baktığımızda Gb’ larca Ram’ den bahsediyoruz. Hatta işlemcilerin birincil ve ikincil ön bellek kapasitelerinin de oldukça yükseldiğini görüyoruz. Tabi bu bana göre yazılımların istedikleri donanımsal ihtiyaçların bir sonucu. Gelelim bu günkü konumuza. Bu günkü konumuzda aslında sanal bellek (Virtual Memory) ile alakalı ve özellikle çok büyük boyutlu dosyalar ile çalışan uygulamalar söz konusu olduğunda bir o kadar da önemli bir mevzu.

.Net Framework 4.0 ile birlikte gelen yeniliklerden birisi de Memory-Mapped File kullanımı. Herşeyden önce Memory-Mapped File kavramının ne anlama geldiğini irdeleyerek işe başlayalım.

Memory-Mapped dosyalar adından da anlaşılacağı üzere bellek üzerine açılmış içerikler olarak düşünülebilirler. Aslında fiziki bir dosyanın tüm içeriğinin Virutal Memory üzerinde oluşturulması ve uygulamanın mantıksal adres alanı içerisinde yer alması söz konusudur. İçeriğin yüklendiği bellek alanı, farklı uygulama Process’ leri tarafından da ele alınabilir. Buna göre farklı Process’ ler bellek üzerine açılmış dosya içeriklerinde okuma ve yazma gibi işlemleri yapabilirler..Net Framework 4.0, Memory-Mapped dosyaların Managed Code tarafından da ele alınabilmesini sağlamaktadır. Nitekim bu versiyona kadar unmanaged code yardımıyla ele alabildiğimiz bir kavramdır. Dolayısıyla C++ ve Win32 API üzerinde değerlendirdiğimiz düşük seviyeli bir konudur.

Memory-Mapped dosyalar iki şekilde değerlendirilmektedir. Persisted ve Non-Persisted olarak. Persisted modelinde bellek üzerinde açılan dosya ile fiziki dosya arasında bir ilişki bulunmaktadır. Bu modele göre bellek üzerindeki içerikte değişiklik yapan son Process işini tamamladığında, yapılanların fiziki dosyaya yansıtılması söz konusudur. MSDN kaynaklarına göre bu model, çok büyük boyutlu dosyaların farklı Process’ ler tarafında ele alındığı durumlarda tercih edilmektedir.

İkinci modele göre bellek üzerine açılan içerik ile fiziki kaynak arasında herhangibir bağ yoktur. Bir başka deyişle fiziki disk üzerindeki bir dosya işaret edilmemektedir. Dolayısıyla bellek üzerinde Process’ ler tarafından yapılan değişiklikler fiziki bir kaynağa yansıtılmamaktadır. Daha çok IPC (Inter Process Communication) tipindeki iletişimlerim söz konusu olduğu durumlarda tercih edilen bir modeldir.

Aslında Memory-Mapped File kavramını aşağıdaki şekil ile biraz daha anlaşılır hale getirebiliriz.
[![blg201_Schema](/assets/images/2010/blg201_Schema_thumb.gif)](/assets/images/2010/blg201_Schema.gif)

Bu şekilde, Memory-Mapped File içerisindeki farklı blokların, farklı Process’ ler tarafından nasıl ele alınabildiği temsil edilmektedir. Buna göre örneğin Block 2, Process 1 ve 2 içerisindeki farklı View’ lar ile ifade edilebilmektedir. Aslında Memory-Mapped dosyaları ile çalışabilmek için mutlaka View oluşturmak gerekmektedir. View nesneleri, ilgili bellek alanında açılan dosya içeriğinin tamamını işaret edebileceği gibi bir kısmını da içerebilir. Bir dosya bloğunun farklı View nesneleri de oluşturulabilmektedir (Multiple Views). Zaten dosya boyutunun, uygulamanının mantıksal bellek alanının (Logical Memory Space) dışına taştığı durumlarda Multi-View nesnelerinin oluşturulması şarttır. Özellikle Gb boyutuna varan dosyalar ile çalışan uygulamalar düşünüldüğünde Multi-View kullanımı kaçınılmazdır.

View nesnelerinin de iki çeşidi bulunmaktadır. Stream Access View ve Random Access View. Eğer dosya içeriğine sıralı olarak erişilecekse (Sequential Access) Stream Access View nesneleri kullanılır. Non-Persisted ve IPC kullanımı söz konusu olduğu durumlarda önerilen metoddur. Persisted modelin kullanıldığı durumlarda ise Random Access View nesneleri tercih edilmelidir. Bu teorik bilgilerden sonra dilerseniz basit bir örnek ile konuyu anlamaya çalışalım.

İlk olarak aynı Solution içerisinde iki farklı Console uygulaması geliştiriyor olacağız. Bu Console uygulamalarından bir tanesi sistemde yer alan bir MP3 dosyasını Memory-Mapped File olarak oluşturacak ve içinden belirli bir aralığı View olarak üretecektir. Diğer uygulamada, aynı dosyanın belleğe açılmış alanına erişecek ve içerisinde farklı bir bloğu View olarak kullanacaktır. İlk olarak ProcessB olarak adlandırdığımız uygulama kodlarını ele alalım.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.IO.MemoryMappedFiles; 
using System.IO;

namespace ProcessB 
{ 
    class ProgramB 
    { 
        static void Main(string[] args) 
        { 
            // Persisted Memory-Mapped File Kullanımı

            Console.Title = "Process B"; 
            string filePath = @"D:\MusicFile.mp3";

            using (MemoryMappedFile mappedFile = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, "Mapped1")) 
            { 
                using (MemoryMappedViewAccessor view = mappedFile.CreateViewAccessor(30000, 50000)) 
                { 
                    for (int i = 0; i < 20; i++) 
                    { 
                        Console.Write("{0} ",view.ReadByte(i)); 
                    } 
                } 
                Console.WriteLine("Kapatmak için bir tuşa basınız"); 
                Console.ReadLine(); 
            } 
        } 
    } 
}
```

Kod parçasında ilk olarak MemoryMappedFile tipinden bir örnek üretildiği görülmektedir. Bu üretim işlemi için CreateFromFile metodundan yararlanılmaktadır. Bu metod Persisted modele göre bir Memory-Mapped File oluşturmaktadır. Bu sebepten dolayı fiziki disk üzerinde var olan bir dosya adresini parametre olarak almaktadır.

Metodun ilk parametresi fiziki dosya adresidir. İkinci parametre ile dosyanın açılacağı ifade edilmektedir. Son parametre ile de Memory-Mapped File için bir isim verilmektedir. Bu isim önemlidir. Nitekim diğer bir uygulama tarafından ilgili bellek adresindeki alana erişilmek istendiğinde kullanılacaktır.

Program kodunun ilerleyen kısmında MemoryMappedFile nesne örneğinin CreateViewAccessor metodundan yararlanılarak bir MemoryMappedViewAccessor oluşturulmaktadır. Bir başka deyişle bir View üretildiğini söyleyebiliriz. Bu üretim işlemi sırasında verilen değerler ise bellek üzerinde okunmak istenen bloğun başlangıcı ile okunacak uzunluğu ifade etmektedir. Bir başka deyişle kaçıncı byte’ tan itibaren ne kadar uzunlukta okuma yapılacağı belirtilir. for döngüsü içerisinde sembolik olarak View şeklinde ele alınan bloğun ilk 20 byte’ ı ekran yazdırılmaktadır. Çok doğal olarak burada veri üzerinde değişiklikler de yapılması söz konusu olabilir. Gelelim diğer Console uygulamasının kodlarına.

```csharp
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.IO.MemoryMappedFiles;

namespace ProcessA 
{ 
    class ProgramA 
    { 
        static void Main(string[] args) 
        { 
            Console.Title = "Process A"; 
            Console.WriteLine("Diğer Process ilgili dosyayı açana kadar bekleyelim"); 
            Console.ReadLine();

            using (MemoryMappedFile mappedFile = MemoryMappedFile.OpenExisting("Mapped1")) 
            { 
                using (MemoryMappedViewAccessor view2 = mappedFile.CreateViewAccessor(70000, 120000)) 
                { 
                    for (int i = 0; i < 10; i++) 
                    { 
                        Console.Write("{0} ", view2.ReadByte(i)); 
                    } 
                } 
                Console.WriteLine("Kapatmak için bir tuşa basınız"); 
                Console.ReadLine(); 
            } 
        } 
    } 
}
```

Bir önceki koda benzer olaraktan yine bir MemoryMappedFile ve View üretimi söz konusudur. Ancak MemoryMappedFile üretimi için OpenExisting metodundan yararlanılmaktadır. Bu metoda verilen parametre dikkat edileceği üzere bir önceki uygulamanın oluşturduğu Memory-Mapped File için verilen takma isimdir. Dolayısıyla program kodunun bu noktasında eğer bellek üzerinde Mapped1 isimli bir Memory-Mapped File varsa ilgili nesne örneklenebilmektedir.

Nesne örneklemesini takiben yine bir MemoryMappedViewAccessor oluşturulmaktadır. Bu kez farklı bir blok ele alınmakta ve yine içeriğinin ilk 10 byte’ lık bölümü ekrana yazdırılmaktadır. Önce ProgramB ardından ProgramA çalıştırılacak şekilde Solution ayarlanırsa aşağıdaki çalışma zamanı görüntüsü elde edilecektir

[![blg201_Runtime](/assets/images/2010/blg201_Runtime_thumb.gif)](/assets/images/2010/blg201_Runtime.gif)

Bu örnekte iki farklı Process’ in aynı Memory-Mapped File üzerinde farklı bloklarını işaret eden View’ lar oluşturarak çalıştığı gösterilmektedir. Ancak tabiki dikkat edilmesi gereken bazı durumlar söz konusudur. Örneğin Memory-Mapped File içeriğini ilk üreten Process diğeri çalışmadan önce kapatılır ve diğer Process Mapped1 isimli bellek alanına erişmeye çalışırsa, aşağıdaki istisna mesajı ile karşılaşılır.

[![blg201_Exception](/assets/images/2010/blg201_Exception_thumb.gif)](/assets/images/2010/blg201_Exception.gif)

Bu son derece doğaldır. Nitekim ProcessB başlatılmamış, using blokları dışına çıkılmış veya uygulama sonlandırılmıştır. Ancak tüm bunlar gerçekleşirken ProcessA henüz çalıştırılmamış olabilir.

Geliştirdiğimiz örnekte bellek üzerine açtığımız dosya içeriğinde bir yazma işlemi gerçekleştirilmemiştir. Ancak CreateFromFile metodunun kullanılması halinde Persisted model söz konusudur. Dolayısıyla yapılan değişiklikler son olarak fiziki dosyaya da aktarılabilir.

Biraz dinlenmeye ne dersiniz. Takip eden yazımızda Non-Persisted modele göre Memory-Mapped File kavramını ele almaya çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[MemoryMappedFileKavrami.rar (40,21 kb)](/assets/files/2010/MemoryMappedFileKavrami.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde geliştirilmiş ve test edilmiştir]