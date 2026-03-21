---
layout: post
title: ".Net Framework 4.0 System.IO.File Tarafındaki Yenilikler"
date: 2010-10-25 21:45:00 +0300
categories:
  - dotnet-framework-4-0
  - bcl
tags:
  - base-class-library
  - .net-framework
  - file-io
  - system.io
---
Bu yazımızda ele alacağımız konu ile ilişkili olarak kullanacağım giriş resmi için uzun bir süre araştırma yapmak zorunda kaldım. Sanırım yazıyı yazdığım bu sıcak yaz gününde devrelerim istediğim randımanı vermedi. Ancak en azından yandaki resim, anlatacağım ilk konu ile doğrudan alakalı olarak düşünülebilir. Bu resimde üst üste binmiş onlarca metre yüksekliğe varan dosya dolapları olduğu ifade edilemekte. Bunların hepsinin tek bir dosya içerisinde birleştirildiğini düşünün. Üstelik bu dosya text tabanlı olsun.

![blg199_Giris_.jpg](/assets/images/2010/blg199_Giris_.jpg)

Bu tip bir dosyanın bir bankadan gönderilme olasılığı aslında çok yüksektir. Geçmiş deneyimlerimi düşündüğümde son derece olağan bir durum. Örneğin bundan önce çalıştığım ve outsource olarak görev yaptığım bankada, boyutları 600 mb’ ın üzerinde olan text tabanlı dosyalar sistemler arasında dolaşıp durmaktaydı. Hatta bu dosyalardan bazıları SSIS paketlerine sokularak veritabanı ortamına aktarılmaktaydı. Hatta SSIS uzmanı olmamama rağmen Proje Yöneticisi tarafından bir dönem bana da böyle bir iş verildiğini ifade etmek isterim

![Undecided](/assets/images/2010/smiley-undecided.gif)

Tabi bu tip dosyaların en büyük özelliği de veri taşımak amacıyla satır ve sütun kavramlarını kullanmalarıdır. Ayrıca verinin çok yalın bir formatta taşınması ve her tür platform tarafından kolayca ele alınması, hatta çıplak gözle (Tabi zaman zaman. Nitekim benim üzerinde çalıştığım SSIS paketlerinin sütunlarının sayısı 250’ ye varmaktaydı ![Sealed](/assets/images/2010/smiley-sealed.gif)) rahatlıkla okunabilmesi gibi avantajlar da söz konusudur. Buna göre aşağıdaki gibi bir text içeriği doğru bir veri saklama biçimi olarak düşünülebilir.

![blg199_TextContent_.gif](/assets/images/2010/blg199_TextContent_.gif)

Burada birbirleriyle | işaretleri şeklinde ayrılmış sütunlar söz konusudur. Toplam 4 satırdan oluşan veri içeriğini programatik ortamda da okumak son derece kolaydır. Bunun için, kolaya kaçan geliştiriciler.Net Framework 2.0 ile birlikte File sınıfına eklenmiş static ReadAllLines metodunu kullanır. Aşağıdaki kod parçasında görüldüğü gibi.

![blg199_ReadAllLines_.gif](/assets/images/2010/blg199_ReadAllLines_.gif)

ReadAllLines metodu parametre olarak dosya adresini almakta ve içeriğinde tüm satırları string tipinden bir diziye aktarmaktadır. Debug çıktısında, string dizisine aktarılan içerik net bir şekilde görülebilir. Tabi bu adımdan sonra elde edilen string[] dizisi üzerinde dolaşılıp | işaretlerine göre ayrıştırma yapılarak sütunlara da kolayca erişilebilir. Ancak önemli bir sorun da vardır?

Boyutu çok yüksek olan bir dosyanın ReadAllLines metodu yardımıyla okunmasının sakıncıları var mıdır?

Aslında en büyük sakınca ReadAllLines metodu, dosyanın tüm satırlarını string[] diziye aktarana kadar, içinde çalıştığı Thread’ i duraksatmaktadır. Bir başka deyişle ReadAllLines metodu ile okunan satırların tamamı string dizisine aktarılmadan kod bir sonraki satıra inmeyecektir. İşte Base Class Library->IO bünyesinde getirilen yeni static metodlardan birisi bu vakayı çözmek amacıyla getirilmiştir. File.ReadLines metodu.

ReadLines static metodunun aşırı yüklenmiş (Overload) iki versiyonu vardır. Her ikiside satırları okunacak dosyayı parametre olarak almaktadır. Metodlar geriye IEnumerable Arayüzü (Interface) tarafından taşınabilecek bir referans döndürmektedir. ReadLines metodu aslında dosyanın satırları arasında gezinmeyi sağlayacak bir numaralandırıcıyı hazırlamaktadır. Bu sebepten kod satırı anında alt satıra inebilir. Üstelik tüm satırların yüklenmesi de söz konusu değildir. Örneğin çok büyük bir text dosyasının sadece ilk 3 satırını almak istediğimizde aşağıdaki gibi bir kod parçası işe yarayacaktır.

```csharp
using System; 
using System.IO; 
using System.Collections.Generic;

namespace NewIOFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string filePath = Path.Combine(Environment.CurrentDirectory, "Bilgiler.txt"); 
            //string[] lines=File.ReadAllLines(filePath); 
            IEnumerable<string> lines=File.ReadLines(filePath);            
            int i = 0; 
            foreach (string line in lines) 
            { 
                if (i < 3) 
                { 
                    Console.WriteLine(line); 
                    i++; 
                } 
                else 
                    break; 
            } 
        } 
    } 
}
```

![blg199_ReadLinesRuntime_.gif](/assets/images/2010/blg199_ReadLinesRuntime_.gif)

Bu kod parçasında görüldüğü üzere foreach döngüsü ile Bilgiler.txt dosyasının satırları üzerinde hareket edilmeye başlanmaktadır. Integer tipinden olan i değişkeninden yararlanılarak bir sayaç oluşturulmuş ve ilk 3 satır okunduktan sonra break keyword’ ü yardımıyla döngüden çıkılması sağlanmıştır. Tabi şunu hatırlatmamızda yarar vardır. foreach döngüsü yardımıyla dosya içerisindeki tüm satırların dolaşılması ReadAllLines metodunun yaptığı işin aynısıdır. Dolayısıyla duruma göre uygun olan metodu kullanmakta yarar vardır. System.IO tarafında BCL takımı tarafından getirilen yeniliklerden birisini bu şekilde özetlemiş olduk. Gelelim diğer bir yeniliğe.

File tipinin dosya okuma ve yazma işlemlerinde sağladığı kolaylık bilinmektedir. Ancak yine de bazı iyileştirmeler gerekmiştir. Söz gelimi WriteAllLines metodunun.Net 4.0 öncesindeki versiyonunu ele alalım.

![blg199_WriteAllLinesOld_.gif](/assets/images/2010/blg199_WriteAllLinesOld_.gif)

WriteAllLines metodu bir önceki sürümde iki aşırı yüklenmiş versiyona sahiptir. Bu metod belirtilen dosyaya, satır bazlı string bir içeriğin eklenmesi amacıyla kullanılmaktadır. Çok güzel. Ama eksik. Nitekim versiyonda dikkati çeken nokta, sadece string tipinden bir dizinin kullanılabiliyor olmasıdır. Peki bunun ne gibi bir sakıncası olabilir?

string[] tipinden olmayan, örneğin List gibi bir koleksiyon içeriğinin WriteAllLines metodu yardımıyla dosyaya yazılabilmesi için, önce string[] dizisine çevrilmesi gerekmektedir.

Visual Studio 2008 üzerinde yazılmış örnek bir kod parçası ile bu durumu görelim.

```csharp
using System; 
using System.Collections.Generic; 
using System.IO;

namespace IO 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string filePath=Path.Combine(Environment.CurrentDirectory,"Bilgiler.txt"); 
            List<string> content=new List<string>{ 
                "10001|Burak Selim Şenyurt|1000|A" 
                ,"10002|Bili Geyt|2000|B" 
                ,"10003|Maykıl Cordın|3400|B" 
                ,"10004|Megın Fox|2200|C" 
            };

            File.WriteAllLines(filePath, content.ToArray()); 
        } 
    } 
}
```

Burada List tipinden olan koleksiyonun içeriğinin Bilgiler.txt dosyasına yazıldığını görmektesiniz. Ancak WriteAllLines metodunun içeriğine dikkat edilecek olursa, content isimli değişkenin ToArray metodu yardımıyla string[] dizisine dönüştürüldüğü görülmektedir. İşte BCL takımı.Net Framework 4.0’ da WriteAllLines metodunun yeni bir aşırı yüklenmiş (Overload) versiyonunu yazarak bu durumu düzeltmiştir.

![blg199_WriteAllLinesNew_.gif](/assets/images/2010/blg199_WriteAllLinesNew_.gif)

Buna göre daha önceki versiyonda yapmak zorunda kaldığımız ToArray dönüştürmesine gerek yoktur. Son olarak yazdığımız kodun Visual Studio 2010 üzerinde geliştirilen yeni hali aşağıdaki gibidir.

```csharp
string filePath = Path.Combine(Environment.CurrentDirectory, "Bilgiler.txt"); 
            List<string> content = new List<string>{ 
                "10001|Burak Selim Şenyurt|1000|A" 
                ,"10002|Bili Geyt|2000|B" 
                ,"10003|Maykıl Cordın|3400|B" 
                ,"10004|Megın Fox|2200|C" 
            };

            File.WriteAllLines(filePath, content);
```

Bu yenilik sayesinde WriteAllLines metodunun string Array dışındaki IEnumerable arayüzünü uygulayan tipler ile de çalışabilmesi sağlanmıştır. Gelelim bu yazımızda değineceğimiz son yeniliğe.

WriteAllLines metodu content değişkeni ile aldığı içeriği ilgili dosyaya yazarken dosyanın sürekli yeniden oluşturulmasına neden olmaktadır. Oysaki dosya sonuna ilave yapmamız da gerekebilir..Net Framework 4.0 öncesindeki sürümde AppendAllLines isimli bir metod bulunmamaktadır. Ancak.Net Framework 4.0 ile birlikte bu metod File tipine eklenmiştir ve aşağıdaki örnek kod parçasında olduğu gibi kullanılabilir.

```csharp
using System; 
using System.IO; 
using System.Collections.Generic;

namespace NewIOFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string filePath = Path.Combine(Environment.CurrentDirectory, "Bilgiler.txt"); 
            List<string> content = new List<string>{ 
                "10001|Burak Selim Şenyurt|1000|A" 
                ,"10002|Bili Geyt|2000|B" 
                ,"10003|Maykıl Cordın|3400|B" 
                ,"10004|Megın Fox|2200|C" 
            };

            File.WriteAllLines(filePath, content);

            List<string> newLines = new List<string> 
            { 
                "10006|Komiser Kolomb|2345|Z" 
            }; 
            File.AppendAllLines(filePath, newLines); 
        } 
    } 
}
```

AppendAllLines metodu IEnumerable tipi ile çalışmakta ve bu tipin çalışma zamanındaki içeriğini parametre olarak verilen dosyanın sonuna eklemektedir.

Tabi BCL takımı tarafından getirilen daha pek çok yenilik daha söz konusudur. Örneğin bunlardan bazılarını hemen araştırmaya başlayabilirsiniz.

- Directory.EnumerateFiles
- Directory.EnumerateDirectories
- Directory.EnumerateFileSystemEntries

vb...

Yeri geldikçe bunları toplu olarak ele almaya çalışıyor olacağız. Bu yazımızda File tipi için öne çıkan bazı yenilikleri gördük. Buna göre;

- ReadLines metodu yardımıyla satır bazlı bir text içeriğinin bir seferde değil ama bir iterasyon yardımıyla okunabileceğini gördük.
- WriteAllLines metodunun sadece string Array tipi ile çalışan versiyonu yerine IEnumerable ile çalışan bir versiyonunun geliştirildiğini gördük. Bu sayede bir önceki sürümde söz konusu olan ToArray metodu ile diziye dönüştürme zorunluluğunun ortadan kalktığına şahit olduk.
- Son olarak dosya sonuna IEnumerable ile işaret edilen bir içeriğin eklenebilmesini sağlayan AppendAllLines metoduna değindik.

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[NewIOFeatures.rar (21,53 kb)](/assets/files/2010/NewIOFeatures.rar) [Örnek Visual Studio 2010 Ultimate sürümünde geliştirilmiş ve test edilmiştir]
