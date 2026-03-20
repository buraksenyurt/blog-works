---
layout: post
title: "Int32 ve Int64 Haricindekiler için Parallel.ForEach"
date: 2010-05-16 21:46:00 +0300
categories:
  - parallel-programming
tags:
  - parallel-programming
  - csharp
  - dotnet
  - threading
  - visual-studio
---
Bir kaç yıl öncesine kadar Bizitek firmasına Junior Developer olarak görev almaktaydım. Bu şirkette çalıştığım süre boyunca pek çok projede görev alma fırsatım oldu. Ancak genellikle şirketin iş akışları üzerine geliştirdiği bir ürünün kurulması ve ihtiyaçlara göre düzenlenmesi ile ilgilenmekteydim. Söz konusu uygulamanın belki de en önemli özelliklerinden birisi, kurulduğu firmanın organizasyon ağacını içermesi ve buna göre akış içi adımların kolayca tesis edilebilmesiydi. İşte zaten bazı sıkıntılar da burada başlıyordu. Nitekim bazı firmaların organizasyonel yapıları düzgün değildi. En sık rastlanan vakalardan birisi, herhangibir çalışanın aslında birden fazla görev üstlenmesi nedeniyle organizasyon ağacında birden fazla yerde var olabilmesiydi.

![blg165_Giris.jpg](/assets/images/2010/blg165_Giris.jpg)

![Undecided](/assets/images/2010/smiley-undecided.gif)

Dolayısıyla bazı durumlarda en tepeden aşağıya doğru inen ve bağlı liste (Linked List) benzeri bir oluşumun sağlanması zorlaşmaktaydı. Her neyse...Eminim bu sorunlar çoktan aşılmıştır. Ancak bir önceki cümlede yer alan bağlı liste tarzı yapıların başında dolaşan bir kara bulut daha mevcuttur. Sorunun kaynağında paralel programlama amacıyla.Net ortamına kazandırılan Parallel.ForEach döngüsü yer almaktadır. Dilerseniz öncelikle sorunu masaya yatıralım. Bu amaçla aşağıdaki kod içeriğine sahip Employee isimli basit bir sınıfımız olduğunu düşünelim.

```csharp
class Employee
{
	public string Profession { get; set; }
	public string Name { get; set; }
	public Employee Parent { get; set; }
	public Employee Child { get; set; }        
}
```

Employee sınıfı ile bir şirketin belirli organizasyonel pozisyonlarını ifade etmek istediğimizi düşünebiliriz. İçeriğinde yer alan Parent ve Child isimli özellikler dikkat edileceği üzere Employee tipindendir. Buna göre bir Employee nesne örneğinin altına ve üstüne başka bir Employee referansının atanması mümkündür. Dolayısıyla bir ağaç yapısının kolayca oluşturulması mümkündür. Elbetteki sembolik olarak. Söz gelimi;

Director
--->Project Manager
------>Technical Project Manager
--------->Senior Developer
------------>Junior Developer

gibi.

Yukarıdaki gibi bir yapıyı oluşturduğumuzda Parent ve Child özellikleri sayesinde organizasyon içerisinde aşağı ve yukarı doğru kolayca hareket edebileceğimizi görebiliriz. Bu durum aşağıdaki Console uygulamasında ele alınmaktadır.

```csharp
using System;

namespace ParallelForNonIntegralRanges
{
    class Program
    {
        static void Main(string[] args)
        {
            Employee root = Fill();
                        
            #region Case 1 - Seri for döngüsü

            for (Employee emp = root; emp !=null ; emp=emp.Child)
            {
                Console.WriteLine("{0} {1}",emp.Profession,emp.Name);
            }

            #endregion
        }

        static Employee Fill()
        {
            Employee d = new Employee { Profession = "Director",Name="Bill" };
            Employee pm = new Employee { Profession = "Project Manager",Name="Steve" };
            Employee tpm = new Employee { Profession = "Technical Project Manager",Name="Joe" };
            Employee sd = new Employee { Profession = "Senior Developer",Name="Nicole" };
            Employee jd = new Employee { Profession = "Junior Developer",Name="Burak" };

            d.Parent = null;
            d.Child = pm;

            pm.Parent = d;
            pm.Child = tpm;

            tpm.Parent = pm;
            tpm.Child = sd;

            sd.Parent = tpm;
            sd.Child = jd;

            jd.Parent = sd;
            jd.Child = null;

            return d;
        }
    }

    class Employee
    {
        public string Profession { get; set; }
        public string Name { get; set; }
        public Employee Parent { get; set; }
        public Employee Child { get; set; }        
    }
}
```

Dikkat edileceği üzere for döngüsünden yararlanılarak Director'den en alt kademede yer alan Junior Developer'a kadar ilerlenilmesi sağlanılmaktadır. İşte bu örnek kod parçasının çalışma zamanı çıktısı;

![blg165_Runtime1New.gif](/assets/images/2010/blg165_Runtime1New.gif)

Peki ya Employee gibi bir tipin çalışma zamanındaki örneği ve içeriğindeki bağlı referanslar arasında Parallel.For döngüsü ile ilerlenilmek istenirse?

![Wink](/assets/images/2010/smiley-wink.gif)

Nitekim elimizin alında binlerce ve hatta daha fazla elemandan oluşan bir ağaç yapısı olabilir ve bu yapı üzerindeki elemanlarda bazı işlemlerin yapılması istenebilir. Bu durumda işlemlerin daha hızlı gerçekleştirilebilmesi için paralel programlama yetenekleri göz önüne alınabilir. Ancak ortada önemli bir sorun vardır. Parallel.For metodunun versiyonlarına bakıldığında int (Int32) ve long (Int64) tipleri ile çalıştığı görülecektir. Bu durumda Employee nesne örnekleri için Parallel.For döngüsünü kullanmamız mümkün değildir. O zaman belki Parallel.ForEach döngüsü tercih edilebilir. Edilebilir mi acaba? Bunu denediğimizde derleme zamanında aşağıdaki sonuçlar ile karşılaşmamız kaçınılmazdır.

![blg165_Excpetion.gif](/assets/images/2010/blg165_Excpetion.gif)

Bu son derece doğaldır. Nitekim Employee tipinin IEnumerable gibi bir arayüz implemantasyonu yapmadığı ortadadır. Kaldı ki foreach döngüleri üzerinde hareket edecekleri tipler için bu arayüz implemantasyonunun yapılmasını beklemektedir. O halde farklı bir yardımcı metoddan yararlanalılabilir. Aşağıdaki gibi;

```csharp
static IEnumerable<Employee> Iterate(Employee root)
{
   for (Employee emp = root; emp != null; emp = emp.Child)   
   {
      yield return emp;   
   }
}
```

Iterate isimli metod yield return kullanarak IEnumerable tipinden bir sonuç kümesi döndürmektedir ve bu aslında Parallel.ForEach döngüsünün tamda istediği referanstır. Dolayısıyla artık aşağıdaki gibi bir kod parçası derlenip çalıştırılabilir.

```csharp
Parallel.ForEach<Employee>(
   Iterate(root),
   emp => Console.WriteLine("{0} {1}", emp.Profession, emp.Name)
);
```

Dikkat edileceği üzere ilk parametre ile ForEach döngüsünün beklediği IEnumerable içeriği verilmektedir. Bu durumda kod sorunsuz bir şekilde çalışacaktır. Tabiki ekrana yazdırılma sırası değişmeyecektir. Nitekim buradaki tip içeriği saniyenin çok küçük bir diliminde tamamlanacağından farklı Thread'lerin işi ele almasına zaman kalmayacaktır. Sakın sırası karışık bir organizasyon ağacı yazdırılmasını beklemeyin. Yaptığımız sadece ve sadece Int32/Int64 dışında kalan ve IEnumerable gibi bir arayüzü implemente etmeyen bir tipe ait çalışma zamanı nesne örneği ve bağlı referanslarının, Parallel.ForEach döngüsü tarafından dolaşılabilmesini sağlamaktır.

Ancak yine de dikkat edilmesi gereken önemli bir durum vardır. IEnumerable thread safe olmadığından döngünün kullandığı veriye olan erişimler sırasında kilitleme tekniklerinden yararlanılması gerekebilir ki bu da aslında performansı olumsuz yönde etkileyebilecek bir durumdur. Burada performansı arttırmaya yönelik olarak belkide Iterate metodunun sonucunun ToArray gibi bir metod yardımıyla diziye çevrilmesi düşünülebilir. Aşağıdaki kod parçasında görüldüğü gibi.

```csharp
Parallel.ForEach<Employee>(
   Iterate(root).ToArray(),
   emp => Console.WriteLine("{0} {1}", emp.Profession, emp.Name)
);
```

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ParallelForNonIntegralRanges_RTM.rar (27,05 kb)](/assets/files/2010/ParallelForNonIntegralRanges_RTM.rar) [Örnek uygulama Visual Studio 2010 Ultimate RTM sürümü üzerinde geliştirilmiş ve test edilmiştir]
