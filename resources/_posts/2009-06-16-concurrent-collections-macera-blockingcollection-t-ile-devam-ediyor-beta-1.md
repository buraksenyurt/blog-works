---
layout: post
title: "Concurrent Collections : Macera BlockingCollection<T> ile Devam Ediyor [Beta 1]"
date: 2009-06-16 06:54:00 +0300
categories:
  - parallel-programming
tags:
  - parallel-programming
  - csharp
  - dotnet
  - threading
  - concurrency
  - generics
---
Bir önceki blog yazımda paralel programlama kabiliyetlerinden birisi olan Concurrent Collections (Eş Zamanlı Koleksiyonlar) kavramını incelemeye çalışmıştım. Ne varki kendimi bunlara olan gereklilikler konusunda bir süredir ikna edebilmiş değilim. Dolayısıyla ihtiyaçları ortaya koymak adına basit bir senaryo üzerinden ilerlemeye karar verdim. Aslında eş zamanlı koleksiyonların kullanılması için en büyük gereksinim, bir koleksiyonun elemanları üzerinde aynı anda işlemler yapılmak istenmesi halinde ortaya çıkmaktadır. Konuyu daha net kavrayabilmek adına şöyle bir senaryoyu geliştirmeye karar verdim; Bir metin dosyasında `|` işaretleri ile birbirlerinden ayrılmış text tabanlı verilerin, generic bir List koleksiyonu içerisine alınması ve sonrasında ise bu koleksiyon elemanlarının içeriklerinin değiştirilmesi.

Tabiki burada iki ana iş var. Metin dosyasının ayrıştırılıp (parse) koleksiyon içerisinde toplanması ilk adım olarak düşünülebilir. İkinci adımda ise, bu koleksiyon üzerinde ileri yönlü bir iterasyon ile o anki nesne örneği üzerinde değişiklik yapılmaya çalışılması (örneğin maaş bilgisinin değiştirilmesi) durumu ele alınmalıdır. Ancak burada küçük ama önemli bir maddemiz var; bu iki adımdaki işlemleri paralel olarak gerçekleştirebilmek

Dolayısıyla iki farklı Thread'in birlikte çalışarak söz konusu işlemleri yapması sağlanabilir. Bu fikirden yola çıkarak aşağıdaki bir Console uygulamasını geliştirdim. Projede yer alan ana sınıflar aşağıdaki class diagram çizelgesinde görüldüğü gibidir.

![blg32_1.gif](/assets/images/2009/blg32_1.gif)

Örnekte text tabanlı içeriği tutan Personel.txt dosyasının içeriğini ise aşağıdaki gibi tamamen atmason verilerden oluşturmuş bulunmaktayım.

![blg32_2.gif](/assets/images/2009/blg32_2.gif)

Program kodları ise;

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ConcurrentCollections2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PersonManager manager = new PersonManager();
                manager.StartTest();
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.Message);
            }

            Console.ReadLine();
        }        
    }

    // Metin dosyasındaki bilgilerin nesne karşılıkları için tasarlanmış Person sınıfı
    class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public decimal Salary { get; set; }
    }

    // Test metodunu içeren Test sınıfımız
    class PersonManager
    {
        // Person bilgilerinin tutulacağı generic List koleksiyonu
        List<Person> personList = new List<Person>();

        public void StartTest()
        {            
            // GetPersonList metodu için bir Thread tanımlanır
            Thread trd1 = new Thread(new ThreadStart(GetPersonList));
            // ProcessPersonList metodu için bir Thread tanımlanır
            Thread trd2 = new Thread(new ThreadStart(ProcessPersonList));

            // Thread' ler başlatılır
            trd1.Start();
            trd2.Start();
        }

        // Metin dosyasından okuma işlemini yaparak personList isimli generic List koleksiyonuna Person nesne örneklerinin eklenmesi işlemini üstlenir
        private void GetPersonList()
        {
            // Personel.txt dosyasındaki tüm satırlar string[] dizisine alınır
            string[] persons = File.ReadAllLines(System.Environment.CurrentDirectory + "\\Personel.txt");

            // Her bir satır ele alınır
            foreach (string person in persons)
            {
                // Satır | işaretine göre ayrıştırılır
                string[] values = person.Split('|');

                // Ayrıştırma sonucu elde edilen değerlere göre Person nesne örneği oluşturulur
                Person prs = new Person
                {
                    PersonId = Convert.ToInt32(values[0]),
                    Name = values[1],
                    Title = values[2],
                    Salary = Convert.ToDecimal(values[3])
                };
                // Persone nesne örneği koleksiyona eklenir
                personList.Add(prs);
                // Console penceresinden bilgilendirme yapılır
                Console.WriteLine("{0} listeye eklendi", prs.Name);

                Thread.Sleep(250); // işleyişi kolay takip edebilmek için küçük bir zaman aldatmacası
            }
        }

        // personList isimli generic List koleksiyonundaki her bir Person nesne örneğinin Salary bilgisini değiştirir
        private void ProcessPersonList()
        {
            // Koleksiyondaki her bir Persone nesne örneği ele alınır
            foreach (Person person in personList)
            {
                // O anki Person nesne örneğinin Salary özelliğinin değeri değiştirilir
                person.Salary += 1.18M;

                // Console ekranında bilgilendirme yapılır
                Console.WriteLine("\t {0} için maaş {1} olarak değiştirildi", person.Name, person.Salary);
                Thread.Sleep(250); // işleyişi kolay takip edebilmek için küçük bir zaman aldatmacası
            }
        }
    }
}
```

PersonManager sınıfı içerisinde yer alan StartTest metodu kendi içerisinde iki farklı Thread oluşturmakta ve çalıştırmaktadır. Bu Thread'lerden birisi GetPersonList fonksiyonunu kullanarak koleksiyona veri ekleme işlemini üstlenmektedir. İkinci Thread tarafından çağırılan ProcessPersonList metod ise, maaş bilgilerini düzenlemektedir. Kritik olan nokta her iki Thread'in aynı koleksiyon nesne örneği üzerindeki elemanları kullanmak istemesidir. Programı çalıştırdığımda aşağıdaki sonuç ile karşılaştım;

![blg32_3.gif](/assets/images/2009/blg32_3.gif)

Görüldüğü gibi koleksiyon zaten farklı bir Thread içerisinde ele alındığından, düzenleme işlemi yapılmasına izin verilmemektedir. İşte eş zamanlı koleksiyonları ele almak için geçerli bir neden. Peki ama hangi eş zamanlı koleksiyon

![Undecided](/assets/images/2009/smiley-undecided.gif)

Bu noktada bir önceki blog yazımın sonunda verdiğim sözü hatırlıyorum. BlockingCollection koleksiyonu. Bunun üzerine kodu aşağıdaki şekilde değiştirdim.

```csharp
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollections2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PersonManager manager = new PersonManager();
                manager.StartTestConcurrent();
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.Message);
            }
        }        
    }

    // Metin dosyasındaki bilgilerin nesne karşılıkları için tasarlanmış Person sınıfı
    class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public decimal Salary { get; set; }
    }

    // Test metodunu içeren Test sınıfımız
    class PersonManager
    {
        // Person bilgilerinin tutulacağı generic List koleksiyonu
        // List<Person> personList = new List<Person>();
        BlockingCollection<Person> personList = new BlockingCollection<Person>();

        public void StartTestConcurrent()
        {
            // Task' leri başlatalım
            Task[] tasks ={ Task.Factory.StartNew(() => { GetPersonList(); }),
                              Task.Factory.StartNew(() => { ProcessPersonList(); })
                          };

            // Tüm Task' ler tamamlanıncaya kadar bekle
            Task.WaitAll(tasks);
            
            Console.WriteLine("İşlemler sona erdi. Programdan çıkmak için bir tuşa basın");
            Console.ReadLine();
        }

        // Metin dosyasından okuma işlemini yaparak personList isimli generic List koleksiyonuna Person nesne örneklerinin eklenmesi işlemini üstlenir
        private void GetPersonList()
        {
            // Personel.txt dosyasındaki tüm satırlar string[] dizisine alınır
            string[] persons = File.ReadAllLines(System.Environment.CurrentDirectory + "\\Personel.txt");

            // Her bir satır ele alınır
            foreach (string person in persons)
            {
                // Satır | işaretine göre ayrıştırılır
                string[] values = person.Split('|');

                // Ayrıştırma sonucu elde edilen değerlere göre Person nesne örneği oluşturulur
                Person prs = new Person
                {
                    PersonId = Convert.ToInt32(values[0]),
                    Name = values[1],
                    Title = values[2],
                    Salary = Convert.ToDecimal(values[3])
                };
                // Persone nesne örneği koleksiyona eklenir
                personList.Add(prs);
                // Console penceresinden bilgilendirme yapılır
                Console.WriteLine("{0} listeye eklendi", prs.Name);

                Thread.Sleep(250); // işleyişi kolay takip edebilmek için küçük bir zaman aldatmacası
            }
            // koleksiyona daha fazla eleman eklenmeyeceğini belirt.
            // Bu metodu kullanmadan denediğinizde programın asılı kaldığını ve kapanmadığını göreceksiniz.
            personList.CompleteAdding();
        }

        // personList isimli generic List koleksiyonundaki her bir Person nesne örneğinin Salary bilgisini değiştirir
        private void ProcessPersonList()
        {
            // Koleksiyondaki her bir Persone nesne örneği ele alınır
            foreach (Person person in personList.GetConsumingEnumerable())
            {
                // O anki Person nesne örneğinin Salary özelliğinin değeri değiştirilir
                person.Salary += 1.18M;

                // Console ekranında bilgilendirme yapılır
                Console.WriteLine("\t {0} için maaş {1} olarak değiştirildi", person.Name, person.Salary);
                Thread.Sleep(250); // işleyişi kolay takip edebilmek için küçük bir zaman aldatmacası
            }
        }
    }
}
```

Bu kez BlockingCollection tipinden bir nesne örneğini kullanmaktayız. Bu koleksiyon kendi içerisindeki elemanlar üzerinde eş zamanlı işlemler yapılabilmesine imkan tanımaktadır. Ayrıca istenirse bir boyut verilerek, eş zamanlı çalışma sırasında maksimum eleman ekleme tavanınıda belirtebiliriz. Kodda görüldüğü gibi Task sınıfından yararlanarak kodu tamamen.Net 4.0 havasına büründürmüş bulunuyoruz.

![Laughing](/assets/images/2009/smiley-laughing.gif)

StartTestConcurrent metodu içerisinde dikkat edilmesi gereken noktalardan biriside, Task sınıfının static WaitAll fonksiyonu ile, çalışan tüm Task'lerin tamamlanmasının beklenmesidir.

Ayrıca, GetPersonList metodu içerisinde, text tabanlı dosyadaki tüm elemanların aktarılma işlemi tamamlandıktan sonra CompleteAdding fonksiyonu kullanılarak, artık daha fazla eleman eklenmeyeceği, bu nedenle aynı koleksiyon üzerinde bekleyen başka görevler var ise yollarına devam edebilecekleri belirtilmektedir. Eğer CompleteAdding metodunu kullanmassak, programın kapanmadığı gözlemlenecektir. Uygulamayı çalıştırdığımda aşağıdaki sonuçları aldığımı gördüm;

![blg32_4.gif](/assets/images/2009/blg32_4.gif)

Harika değil mi?

![Laughing](/assets/images/2009/smiley-laughing.gif)

Artık hata mesajı yok. Üstelik koleksiyon üzerinde aynı anda iki farklı gövde işlem yapabilmekte. İstenirse görev sayısı dahada arttırılabilir elbetteki. Örneğin çalışmasına göre bir GetPersonList bir ProcessPersonList metodundan sonuçlar alınması Thread.Sleep sürelerinin aynı olmasından kaynaklanmaktadır. Elbetteki gerçek hayat senaryosunda bu süre aynı olmayacaktır. Bende bu düşünce ile Thread.Sleep metodlarını kaldırdığıma aşağıdaki sonuçları aldım.

![blg32_5.gif](/assets/images/2009/blg32_5.gif)

Dikkat edileceği üzere dosyadan koleksiyona ekleme işlemleri gerçekleşmeden, maaş bilgilerinin düzenlenmesine izin verilmemektedir. Bir başka deyişle koleksiyon içerisinde elemanlar olduğu sürece, ProcessPersonList metodu içerisindeki foreach döngüsü çalışabilmektedir. Aksi durumlarda, koleksiyon üzerindeki iterasyon elemanlar ekleninceye kadar duraksatılmaktadır (Tabi, maaş değişiklikerini yapan foreach döngüsü nerede duracağını nasıl bilecektir sorusunun cevabı = CompleteAdding metodudur). Buda koleksiyona neden BlockingCollection dendiğini açıklamaktadır.

![Wink](/assets/images/2009/smiley-wink.gif)

BlockingCollection tipinin farklı özellikleride bulunmakta. Bunlarıda yeri geldikçe incelemeye gayret edeceğim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ConcurrentCollections2.rar (27,67 kb)](/assets/files/2009/ConcurrentCollections2.rar)