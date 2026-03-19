---
title: "Json, Protobuf ve MessagePack Serileştirme Performansları"
pubDate: 2019-04-01 06:00:00
categories:
  - .Net Core
tags:
  - .Net Core
  - JSON
  - XML
  - Bash
  - C#
  - .NET
  - ASP.NET
  - ASP.NET Core
  - Concurrency
  - Performance
  - Serialization
  - Generics
  - GitHub
---

# Json, Protobuf ve MessagePack Serileştirme Performansları
![mpack_0.jpg](images/mpack_0.jpg)

Merhaba Arkadaşlar,

Süper salyangoz Turbo'nun hikayesini bilir misiniz? Hani şu sırtında roket takılı olan Turbo'nun. Peki ya oyununu oynadınız mı? Ben epey süre önce S (h) arp ile oynamış ve oldukça eğlenmiştim. Tabii cevaplar kişiden kişiye değişir lakin ondan esinlenilen bir logo'nun günümüz.Net uygulamalarında performans ölçümü için kullanılan meşhur [BenchmarkDotNet](https://benchmarkdotnet.org/index.html)'e ait olduğu kesin diyebilirim. Aslında ciddi anlamda düşünürsek yazdığımız uygulamaların bütün olarak veya parça halinde çalışma zamanı performanslarını metrik olarak ölçümlemek pek de kolay olmayan konulardan birisidir.

Bazen fonksiyonelliklerin hızı öne çıkarken bazen de alansal büyüklükler değer kazanabilir. Ama başka kriterler de vardır. Eş zamanlı yüklenmelerin artması sonrası oluşan hatalı sonuçlanma değerleri, standart sapmalar bunlara örnek olarak verilebilir. Tabii ölçümlemeyi yaparken kullanılan teknikler de önemlidir. Çıktıların yorumlanması titizlikle yapılmalıdır. Çünkü hatalı yorumlamalar tercihleri olumsuz yönde etkileyebilir.

Peki ne oldu da bu konuya geldim dersiniz?

Geçtiğimiz günlerde West-World'ün başında otururken hız ve alansal büyüklük açısından ölçümlemem gereken bir konuyla karşılaştım. MessagePack isimli bir ikili (binary) serileştirme formatı. İlk kez karşılaştığım bir konuydu (cahilliğin bu kadarı) Bildiğimiz JSON içeriklerine uygulanabilen bir teknik olarak karşımıza çıkıyor aslında ama daha hızlı ve daha az yer kapladığı ifade ediliyor. Bir ikili serileşme söz konusu ve örneğin

```json
{"fullName":"burak selim şenyurt","city":"istanbul","salar":1000}
```

şeklindeki bir JSON içeriği, MessagePack formatında

```text
83 a8 66 75 6c 6c 4e 61 6d 65 b4 62 75 72 61 6b 20 73 65 6c 69 
6d 20 c5 9f 65 6e 79 75 72 74 a4 63 69 74 79 a8 69 73 74 
61 6e 62 75 6c a5 73 61 6c 61 72 cd 03 e8
```

gibi üretiliyor ve [resmi siteye](https://msgpack.org/index.html) göre %18 kadar yer kazanımı sağlıyor (Bu örnek için tabii)

![mpack_5.gif](images/mpack_5.gif)

Bu arada MessagePack serileştirmesi binary formatta olmak zorunda da değil. İnsan gözüyle okunabilir bir formatta da dönüşebiliyor ve bu haliyle de daha az yer tuttuğu daha hızlı serileştiği belirtiliyor.

```xml
[["Burak Selim Şenyurt,"istanbul",1000]]
```

Performans açısından söylenenlerin doğruluğundan pek şüphem yok ama yine de ölçümlemek lazım. Malum JSON dışında [Protobuf](https://github.com/protocolbuffers/protobuf) (Google'ın veri değiş tokuş formatı) isimli başka bir serileştirme formatı daha var ortada. Peki bu üç farklı serileştirme formatını performans açısından kıyaslamak için ne yapabiliriz? Benim ilk aklıma gelen senaryo generic bir Entity listesinin farklı boyutlardaki örneklerini bu üç formata göre serileştirmek oldu. Bu deneyi.Net Core tarafında uygulayabilir ve ölçümleme için meşhur BenchmarkDotnet kütüphanesinden yararlanabiliriz. Gelin birlikte basit bir örnek geliştirelim ve hem yeni serileştirme tekniklerini nasıl kullanacağımızı hem de performanslarını nasıl ölçümleyeceğimizi öğrenelim.

İlk olarak bir Console projesi oluşturarak işe başlayabiliriz. Sonrasında bize yardımcı olacak paketleri projeye eklemekte yarar var.

```bash
dotnet add package protobuf-net
dotnet add package MessagePack
dotnet add package Newtonsoft.Json
dotnet add package BenchmarkDotnet
```

Profobuf, MessagePack ve Json serileştirme işlerinde kullanacağımız paketlere ek olarak ölçümleme için BenchmarkDotNet paketini de yüklememiz gerekiyor. Deneysel amaçlı kullanacağımız sınıf aşağıdaki gibi tasarlanabilir.

```csharp
using MessagePack;
using ProtoBuf;

[MessagePackObject, ProtoContract]
public class Book{
    [Key(0),ProtoMember(1)]
    public int Id { get; set; }
    [Key(1),ProtoMember(2)]
    public string Title { get; set; }
    [Key(2),ProtoMember(3)]
    public double Price { get; set; }
}
```

MessagePack ve Protobuf serileştirmeleri için bir kaç nitelik (attribute) ile zengineştirilmiş bir sınıf olduğunu görebilirsiniz. Şimdi Benchmark işlerini üstlenecek Serializers isimli bir başka sınıfı yazalım.

```csharp
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BenchmarkDotNet.Attributes;
using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;

[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter, CoreJob, MaxWarmupCount(8),MinIterationCount(3), MaxIterationCount(5)] // DryCoreJob
public class Serializers
{
    [Params(1,10,1000,10000,100000)]
    public int BookCount { get; set; }
    public List<Book> books = new List<Book>();
    private string rootPath="c:\\projects\\data\\";

    [GlobalSetup]
    public void LoadDataset()
    {
        for (int i = 1; i < BookCount; i++)
        {
            books.Add(new Book
            {
                Id = i,
                Title = $"Book_{i}",
                Price = 10
            });
        }
    }

    [Benchmark]
    public void ToJson()
    {
        var result = JsonConvert.SerializeObject(books);
        WriteToFile($"json_sample_{BookCount}.json",result);
    }
    [Benchmark]
    public void ToMessagePack()
    {
        var result = MessagePackSerializer.Serialize(books);
        WriteToFile($"mPack_sample_{BookCount}.bin",result);
    }

    [Benchmark]
    public void ToMessagePackJson()
    {
        var content = MessagePackSerializer.Serialize(books);
        var result = MessagePackSerializer.ToJson(content);
        WriteToFile($"mPack_Json_sample_{BookCount}.bin",result);
    }

    [Benchmark]
    public void ToProtobuf()
    {
        using (FileStream fs = new FileStream($"{rootPath}protobuf_sample_{BookCount}.bin", FileMode.Create))
        {
            Serializer.Serialize(fs, books);
        }
    }

    public void WriteToFile(string fileName,string content)
    {
        using (FileStream fs = new FileStream(Path.Combine(rootPath,fileName), FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(content);
            }
        }
    }

    public void WriteToFile(string fileName,byte[] content)
    {
        using (FileStream fs = new FileStream(Path.Combine(rootPath,fileName), FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(content);
            }
        }
    }
}
```

Teorimize göre 1,10,1000,10000 ve 100000 adetlik Book nesne koleksiyonları ile çalışılacak. Benchmark ortamına bu parametreleri ele alacağını söylemek için BookCount özelliğinin başına konan Params niteliğinden yararlanılıyor. Veri kümesini bu parametrelere göre her ölçüm için oluşturacak metod ise LoadDataset ki onun bu görevi üstlenmesi için GlobalSetup niteliği ile işaretlenmesi gerekiyor. Sınıf içerisinde ölçümlenmesini istediğimiz metodların her biri Benchmark niteliğine sahip olmalı. Ölçümlemek istediğimiz dört farklı senaryo var. Kitap listesini JSON, Protobuf, MessagePack ve insan gözüyle okunabilir MessagePack formatında serileştirip fiziki bir dosyaya çıkmak (Aslında serileştirme ve serileşen içeriği dosyaya yazma ölçümlenmesi gereken iki farklı operasyon gibi lakin burada tek bir atomikmiş gibi düşünebiliriz) Tabii bu dört senaryo her bir parametre için oluşan veri setlerinde deneyimlenecek.

Sınıfın başında belirtilen başka nitelikler de var. Standart ölçümlemede kullanılan aşırı yüklenme ve tekrar etme değerleri yüksek olduğu için MaxWarmupCount, MinIterationCount ve MaxIterationCount değerleri ile oynayabiliriz ki ben öyle yaptım. Tabii burada daha fazla ince ayar yapmak mümkün. Bunun için ürünü biraz daha tanımak ve pratiklerin neler olduğunu öğrenmek gerekiyor. CoreJob niteliği,.Net Core çalışma zamanına yönelik bir ölçümleme yapmak istediğimizi ifade ediyor. Önden gelen diğer nitelikler ise ölçüm sonuçlarının hangi formatlarda çıktı olarak sunulacağını ifade etmekte. Buna göre CSV, HTML, MD (markdown language. Alın ürününüz için direkt github'a performans raporu olarak koyun mesela), ASCII gibi formatlarda rapor üretilecek.

Serileştirme işlemlerinde ilgili tiplerin basit fonksiyonelliklerinden yararlanıyoruz. MessagePack serileştirmesi için MessagePackSerializer sınıfının statik Serialize metodu kullanılıyor. Buradan çıkan byte içeriği okunabilir formata çevirmek içinse ToJson fonksiyonundan yararlanılmakta. Protobuf serileştirme doğrudan bir Stream üzerine yapılabiliyor. Serializer sınıfının Serialize metodu bu işi üstlenmekte. Artık aşina olduğumuz JSON serileştirmesi içinse JsonConvert'ün static SerializeObject'i kullanılmakta. Dosya oluşturma ve yazma işlemlerinde ise genellikle çıktının türüne göre (byte array veya string olabilir) hareket etmekteyiz.

Pek tabii Benchmark koşucusunun devreye girmesi için BenchmarkRunner sınıfının statik Run metodunu Main metodunda ele almamız gerekiyor.

```csharp
using System;
using BenchmarkDotNet.Running;

namespace SerializationPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Serializers>();
        }
    }
}
```

Ölçümlemeleri hesaplatmak için uygulamayı release modda çalıştırmamız lazım.

```bash
dotnet run -c release
```

Bir süre bekledikten sonra terminalde aşağıdakine benzer sonuçlarla karşılaşmalısınız. Ben yaklaşık olarak dört dakika kadar bekledim.

![mpack_1.gif](images/mpack_1.gif)

Burada üç ölçüm kriteri görülüyor. Herbirisi mikrosaniye cinsinden hesaplanmış durumda. Çalışma süresini Mean sütununda görebiliriz. Hata üretme değerleri Error kısmında yer alırken standart sapma verileri de StdDev sütununda bulunmakta. Kullandığımız sistemin donanımı da etkili tabii ama aslında kitap sayısı açısından bakarsak oransal anlamda tüm platformlarda benzer sonuçlar çıkacak. Elde edilen verilere göre serileşecek veri kümesinin küçük boyutlu olması halinde tüm ölçüm değerlerinin birbirlerine yakın çıktığını söyleyebiliriz. Ancak 1000, 10000 ve 100000 için farklılıklar daha da belirginleşmeye başlıyor. Newtonsoft aracılığıyla yapılan JSON serileştirme süreleri çok uzun. Hata payı ve standart sapma değerleri de oldukça yüksek. Binary MessagePack en iyi sonuçları üretmiş görünüyor. Hatta protobuf serileştirme süresinden de iyi bir performans sergilemiş diyebiliriz.

Oluşan dosya boyutlarına baktığımızda da MessagePack serileştirmesinin (binay olan) diğerlerine göre en az yer kaplayan içeriği oluşturduğunu söyleyebiliriz (Protobuf serileştirme sonucu ortaya çıkan boyutta fena sayılmaz aslında) JSON çıktısı ise neredeyse iki katı. Tabii çok küçük veri kümelerinde boyutsal farklılıklar çok fark etmiyor.

![mpack_3.gif](images/mpack_3.gif)

BenchmarkDotnet ayrıca rapor çıktılarını da bir klasör altında topluyor (Proje klasöründeki BenchmarkDotNet.Artifacts altında) Aşağıdaki görsellerde örneğimize istinaden üretilen içeriklerden bir kaçını görebilirsiniz.

![mpack_2.gif](images/mpack_2.gif)

Raporun web tabanlı örnek görünümü;

![mpack_4.gif](images/mpack_4.gif)

Hepsi bu kadar:) MessagePack bu ölçümleme değerleri göz önüne alındığında özellikle gerçek zamanlı iletişim yapılan uygulamalarda değer kazanıyor. Gerçek zamanlı uygulamalar denince akla ilk gelen sanıyorum ki SignalR ve WebSockets. Örneğin Asp.Net Core tarafında geliştirilen bir chat uygulamasında mesajlaşma kısmı için MessagePack serileştirmeden yararlanılabilir. Ya da bir merkezden gelecek stok verisini, broadcast yayınla sunucuya bağlı olan tüm istemcilerin grafiklerinde güncelleyecek bir sistemde kullanılabilir. Nitekim kullanıcı sayısının ve gerçek zamanlı veri değiş tokuşunun arttığı senaryolarda, hat üzerinde yol alacak paket içeriklerinin boyutsal olarak minimize edilmesi her zaman için hız avantajı sağlayacaktır. Bu tip bir örneği sonraki makalelerimizde ele almaya çalışacağım. Ne de olsa MessagePack kullanmanın avantaj sağlayacağını görmüş olduk. Burada size düşen bir görev de var. Örneğimizde sadece serileştirme senaryoları ölçümlendi. Peki ya ters serileştirmeden (deserialization) ne haber? Ellerinizden öper:) Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
