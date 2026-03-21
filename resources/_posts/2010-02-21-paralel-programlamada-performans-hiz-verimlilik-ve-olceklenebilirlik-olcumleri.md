---
layout: post
title: "Paralel Programlamada Performans, Hız, Verimlilik ve Ölçeklenebilirlik Ölçümleri"
date: 2010-02-21 23:05:00 +0300
categories:
  - parallel-programming
tags:
  - parallel-programming
  - task-parallel-library
  - .net-framework
  - csharp
---
Ben Matematik Mühendisliği eğitimi almış bir bireyim. Öğrenim hayatım boyunca en çok yaptığım işlerden birisi, matematiksel teoremlerin bilimsel ispatlarını gerçekleştirmek olmuştur. Hemen hemen mühandisliğin her alanındaki farklı problemlerin modellenmesi ve ispatlarının yapılarak en uygun yol olduklarının gösterilmesi adına pek çok kağıt karalamış ve tüketmişimdir.

![blg135_Giris.jpg](/assets/images/2010/blg135_Giris.jpg)

Zaman zaman neden yaptığımızı anlamadığım ispatlardan tutunda, lastiğine konumuş olan sineğin bisikletin ileriye yönlü ama düz bir rotada olmayan hareketi boyunca çizdiği sarmalımsının denklemini çıkartmaya kadar matematiğin bir o kadar garip ama gizemli evreninde dolaşıp durduğumu hatırlıyorum. Hatta bir gün Matematik Analiz dersinin finalinde karşılaştığım bir soruda ne hikmetse 1=2 sonucuna ulaşmışımdır. Halbuki 1=1' e ulaşmış olmam gerekirdi.

![Sealed](/assets/images/2010/smiley-sealed.gif)

Tabi zaman ilerleyip iş para kazanmaya gelince kimsenin teorem ispatları ile uğraşmadığnı acı olarak farketmiştim. İlgi duyduğum yazılım sektörüne gireli uzun yıllar olduğu için matematiksel teorem ispatlarında tam anlamıyla pas tutmuş durumdayım. Yine de zaman zaman yazılım içerisinde matematiği basit haliyle bile görebilmek, en azından bazı konuların ispatında kullanabiliyor olmak sevindirici.

Gelelim bu yazımızın konusuna. Bildiğiniz üzere bir süredir paralel programlama ile ilişkili konuları incelemeye çalışıyor ve öğrenebildiklerimi sizlerle paylaşıyorum. Yine bu vesile ile geçtiğimiz haftalar içerisinde internette dolaşırken gözüme ilişen kısa ve özlü bir yazı ile karşılaştım. [Microsoft Paralel programlama takımı tarafından yayınlanan FAQ girdisinde](http://blogs.msdn.com/pfxteam/archive/2010/01/19/9950541.aspx), çeşitli kriterlere göre hangi kodun daha iyi olduğunun ölçümlenebilmesi için hangi kriterlere bakılabileceği özetlenmektedir. Kısaca elimizde aşağıdaki tabloda yer alan kriterler mevcut. Bu kriterlere göre seri ve paralel olarak yazılmış kod algoritmalarının kıyaslanması mümkün. Özellikle yazdığımız paralel program kodlarının normal versiyonlarına göre daha iyi olup olmadıklarının tespit edilmesi noktasında son derece mühim kriterler olduklarını düşünmekteyim.

Kriter

Açıklama

Performance (Performans)

Genel olarak seri ve paralel yazılmış kodların performans ölçümlerinde algoritmanın toplam icra süreleri hesaba katılmaktadır. Çoğunlukla ve pek tabii olarak bir algoritmanın yürütülme süresinin diğerine göre daha düşük olması tercih edilir ki bu iyi bir perfomans anlamına gelmektedir.

SpeedUp (Hızlanma)

Hızlanma değerini hesap etmek için şu formül kullanılır;
SpeedUp = Seri Çalışma Süresi / Paralel Çalışma Süresi
Bu formülün sonucuna göre bir algoritmanın diğerinden kaç kat hızlı olduğu belirlenebilir.

Efficiency (Verimlilik/Etkinlik)

Tabiki yazılan algoritmanın çalıştırıldığı seri veya paralel işleyişin hangisinin tercih edileceği kararını vermede rol oynan kriterlerden birisi de hangisinin daha verimli olduğudur. Verimliliği veya etkinliği hesap etmek içinse aşağıdaki formülden yararlanıldığı görülmektedir.
Efficiency = Hızlanma / İşlemci Çekirdek Sayısı

Scalability (Ölçeklenebilirlik)

Ölçeklenme bilimsel alanda son derece yaygın kullanılan etkili bir terimdir. Şu anki konumuza baktığımızda ise yazılan algoritmanın seri ve paralel denemelerine ait SpeedUp değerlerinin farklı sayıda çekirdek/işlemci için nasıl sonuçlar verdiği ile alakalıdır. İşlemci veya çekirdek sayısının artması ile SpeedUp değerlerinin düşmesi bir başka deyişle daha hızlı sonuçlar elde edilmesi, pozitif ölçeklenmenin ispatı olarak düşünülebilir. Yani işlemci/çekirdek sayısının artması dolayısıyla ortamın büyümesi karşılığında hızlanmanında artması beklenir.
Kişisel Not: Her ne kadar söz konusu yazıda sadece çekirdek sayıları hesaba katılsa da bana göre ram ve işlemci tiplerinin de söz konusu algoritmanın seri veya paralel çalışması durumlarındaki ölçeklenmeyi etkileyeceği ve hesaba katılması gerektiği düşüncesindeyim.

Haydi gelin buradaki ölçüm değerlerini örnek bir kod üzerinden incelemeye çalışalım. Bu amaçla Visual Studio 2010 Ultimate RC ve.Net Framework 4.0 RC üzerinde basit bir Console uygulaması geliştiriyor olacağız. Örneğe ait senaryomuz ise şu şekilde olacaktır.

Bir klasör içerisinde yer alan jpg uzantılı resim dosyalarının boyutlarının arttırılmasını ele alan bir kod parçası geliştireceğiz. Resimlerin boyut arttırım işleminin zaman alan ve yorucu bir işlem olduğu düşünüldüğünde, seri ve paralel kodların ürettiği sonuçları yukarıdaki kriterler eşliğinde değerlendirmeye çalışacağız. Ne yazık ki elimde sadece çift çekirdekli iki makine olduğundan Scalability kriterini bu örnekte sizlere gösteremiyorum

![Undecided](/assets/images/2010/smiley-undecided.gif)

Ancak sizler uygun test ortamlarına sahipseniz, ölçeklenebilirlik kriterini değerlendirebilirsiniz ki değerlendirmenizi tavsiye ederim. İşte Console uygulaması kodlarımız...

```csharp
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ProofOfConcept
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles("..\\..\\Images\\", "*.jpg");

            // Execution süreleri tüm kriterlerde önemlidir. Bu hesaplama için Diagnostics isim alanında yer alan Stopwatch tipinden yararlanılır
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            SerialExecution(files);
            watcher.Stop();
            // Seri çalışmanın toplam işlem süresi milisaniye cinsinden elde edilir
            float serialElapsed=Convert.ToSingle(watcher.ElapsedMilliseconds);
            
            watcher.Restart();
            ParallelExecution(files);
            watcher.Stop();
            // Paralel çalışmanın toplam işlem süresi milisaniye cinsinden elde edilir
            float parallelElapsed = Convert.ToSingle(watcher.ElapsedMilliseconds);

            // Kriterler hesaplanır
            Console.WriteLine("Serial Performance {0} mili saniye \t Parallel Perfomance {1} mili saniye",serialElapsed,parallelElapsed);
            float SpeedUp = serialElapsed / parallelElapsed;
            Console.WriteLine("SpeedUp {0}",SpeedUp);
            double Efficiency = SpeedUp / Environment.ProcessorCount;
            Console.WriteLine("Efficiency {0} (% {1})", Efficiency,Efficiency*100);
            Console.WriteLine("Scalability bu örneğimizde ne yazıkki test edilemedi");
        }

        // Seri çalıştırma metodumuz
        static void SerialExecution(string[] fileList)
        {
            foreach (string file in fileList)
            {
                Resize(file);
            }
        }

        // Paralel çalıştırma metodumuz
        static void ParallelExecution(string[] fileList)
        {
            // Dosyaları Paralel ForEach döngüsüne göre ele almakta.
            Parallel.ForEach<string>(fileList, s => Resize(s));
        }

        // Resim dosyasını yeniden boyutlandırmak için kullandığımız metod
        static void Resize(string fileName)
        {
            // Önce Image tipi elde edilir
            Image img = Image.FromFile(fileName);
            //Yeni genişlik ve yükseklik değerleri belirlenir. Örnek olarak % 40 artım yapılmıştır
            int newWidth = Convert.ToInt32(img.Width * 1.40);
            int newHeight = Convert.ToInt32(img.Height * 1.40);
            // Yeni boyutlarına göre bir Bitmap nesnesi örneklenir
            Bitmap btmp = new Bitmap(img, new Size(newWidth, newHeight));
        }        
    }
}
```

Uygulamayı kendi makinemde (Intel Core2Duo, 2.5 Gb Ram), 45 resim (44.8 Mb) üzerinde test ettiğimde aşağıdaki sonuçları elde ettim. Burada seri ve paralel yürütmeler arasındaki farklılıklar kriterler bazında açık bir şekilde ortaya çıkmakta.

![blg135_Runtime.gif](/assets/images/2010/blg135_Runtime.gif)

Buna göre paralel çalıştırmanın performansı seri olana göre daha yüksektir. Ayrıca paralel çalıştırma, seri olana göre 1,55 kata kadar daha hızlıdır. İlaveten paralel olan çalıştırmanın seri olana göre %77 daha verimli/etkili olduğu sonucuna ulaşılabilir. Tabi bu kodu farklı sayıdaki işlemci veya çekirdek sayısına sahip sistemlerde defalarca test edip bir istatistik çıkartmak ve bunun sonuçlarına bakmak daha doğru olacaktır. İsterseniz bu kod parçasında farklı bir deneyimi tecrübe edebilirsiniz. Örneğin SerialExecution ve ParallelExecution metodlarını arka arkaya 10 kez çalıştırıp sonuçları Excel tablosunda istatistikleştirip gerekli analizleri yapabilir ve hangisini tercih edebileceğinize daha kolay karar verebilirsiniz. En azından ispatı daha güçlü kılarsınız.

![Wink](/assets/images/2010/smiley-wink.gif)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ProofOfConcept_RC.rar (4,33 mb)](/assets/files/2010/ProofOfConcept_RC.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir]
