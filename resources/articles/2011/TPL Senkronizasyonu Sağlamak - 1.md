---
title: "TPL Senkronizasyonu Sağlamak - 1"
pubDate: 2011-01-27 13:40:00
categories:
  - Parallel Programming
  - TPL
tags:
  - Parallel Programming
  - TPL
  - C#
  - .NET
  - Task Parallel Library
  - Threading
  - Visual Studio
  - Shared State
---

# TPL Senkronizasyonu Sağlamak - 1
![blg212_Giris](images/blg212_Giris.jpg)

Merhaba Arkadaşlar,

Pek çoğumuzun anahtarlığında sayısız anahtar bulunmaktadır. Özellike gerilim filmlerinde bu anahtarlardan doğru olanı bulmak ve anahtar deliğine sokmak, hep zaman alan başarısız kaçış girişimleri olarak sahnelenir. Genellikle bu başarız girişimlerin sonunda ne olduğu malumdur.

Ancak ister gerilim filmi olsun ister olmasın sonuçta anahtar deliğine herhangibir zamanda takılabilecek sadece tek bir anahtar söz konusudur. Üstelik bu anahtar, aynı yere başka bir anahtarın takılmasına da izin vermez. Aslında izin verip vermemesi, anahtarı tutan veya kullanan kişinin elindedir.

Aslında şu anda varmak istediğim nokta lock kelimesidir. lock, çok kanallı uygulamalarda Thread senkronizasyon işlemlerinde ele alınan tekniklerden yanlızca birisidir. Çok doğal olarak farklı amaçlarla ele alınan tipler de söz konusudur.

Tüm bunlara ek olarak uzun zamandır bilinen Task Parallel Library ve doğal olarak yeni paralel programlama yapısı mevcuttur. Dolayısıyla aynı senkronizasyon vakaları TPL içerisinde de geçerlidir ve bu amaçla eklenmiş yeni özellikler bulunmaktadır. İşte bu yazımız ile birlikte TPL içerisinde senkronizasyon konusunu incelemeye çalışıyor olacağız.

[TPL ve Shared Data Isolation](TPL ve Shared Data Isolation.md) başlıklı yazımızda, n sayıda Task örneğinin ortaklaşa kullandıkları bir veri alanı üzerindeki işlemlerinin, ne gibi sonuçlara yol açabileceğini incelemiş ve bunun önün geçmek için basit bir kaç yolu ele almıştık. Hatırlayacağınız üzere değerlendirdiğimiz senaryoda, Plane tipinden olan nesne örneğine ait Altitude özelliğinin değerinin, Task blokları içerisinde değiştirildiği nokta, sorunun oluşmasına neden olan kritik bölgeydi.

```csharp
for (int i = 0; i < 5; i++) 
{ 
	tasks[i] = new Task(() => 
   { 
		for (int j = 0; j < 1000; j++) 
		{ 
		   f16.Altitude += j - 5; 
		} 
	} 
	); 
	tasks[i].Start(); 
}
```

Aslında ezelden beri Multi-Thread programlamada, Thread’ lerin senkronizasyonu konusu öyle ya da böyle bir şekilde kulağımıza gelmiştir. Çok doğal olarak benzer ihtiyaçlar.Net Framework 4.0 Paralel Programlama alt yapısı içinde geçerlidir. Ancak.Net Framework 4.0 tarafında Lightweight Primitives adında yeni ve daha basit senkronizasyon tipleri de söz konusudur. Bu tipler klasik senkronizasyon tiplerine göre daha kola uygulanabilir. Tabi klasik senkronizasyon Primitive’ lerinin uygulanması her ne kadar zor olsa da, birden fazla Application Domain üzerinde kontrole izin vermektedirler. Oysa ki LightWeight Primitive’ ler sadece tek bir Application Domain için uygulanabilir. Olayı daha fazla karmaşıklaştırmadan önce senkronizasyon ile neyi ifade ettiğimizi ortaya koymamızda yarar vardır.

[![Exclamation](images/Exclamation_thumb_3.gif)](images/Exclamation3.gif) “Herhangibir t anında kritik olan bölgeyde sadece tek bir Task örneğinin işlem yapmasını sağlamak”

Senkronizasyonu sağlamak için.Net Framework üzerinde önceden tanımlı çeşitli Primitive’ lerden yararlanılmaktadır. Aslında özel veri tipleri olarak düşünebileceğimiz Primitive’ ler, kritik bölgelere doğru yapılan Task erişimlerini kontrol altına alan varlıklar olarak düşünülebilirler.

Genel işleyiş şekline bakıldığında, kritik bölgede yer alan veriye erişmek isteyen bir Task örneği, bu isteğini ilk önce Primitive’ e iletmektedir. Eğer söz konusu kritik bölge müsait ise, Task işlemlerini icra etmeye başlayabilir. Ancak müsait değilse, Task, Primitive nesnesinin belirlediği ilkelere göre beklemede kalacaktır. Bu anda, söz konusu bölgede çalışmakta olan Task örneği de, işini bitirdiğinde Primitive’ e bildirimde bulunacaktır. Bu bildirimin ardından Primitive nesne, bekleyen Task örneğinin söz konusu bölgede işlem yapmasına izin verecektir. Tabi işlem yapmaya başlayan Task örneği, söz konusu bölgeyi en azından kilitlediğini, Primitive’ e bildirecektir. Elbette gerçek hayat senaryolarında durum bu anlatım şeklinde olduğu gibi basit değildir. N sayıda Task söz konusu olduğunda birbirlerine göre önceliklerini belirlemek gibi ihtiyaçlarımız da olabilir.

Bu yazımızla birlikte söz konusu senkronizasyon tiplerini basit bir şekilde incelemeye başlıyor olacağız. Haydi başlayalım.

## En Basit Askerimiz: lock

lock anahtar kelimesi aslında System.Threading.Monitor sınıfının daha basit bir şekilde uygulanmasını sağlayan bir Primitive olarak düşünülebilir. Normal şartlar altında kritik bölgeyle olan etkileşimlerde Monitor tipinin Enter, TryEnter, Exit gibi metodlarının kullanılması gerekmektedir. Monitor tipi HeavyWeight Primitive olarak bilinmektedir ve daha alt seviyedeki vakaların karşılanmasında kullanılmaktadır. Aşağıdaki kod parçasında daha önceki yazımızda ele aldığımız sorunlu senaryonun lock ile çözümü gösterilmektedir.

```csharp
using System; 
using System.Threading.Tasks;

namespace SynchronizationPrimitives 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                TestMethod(); 
            }

            Console.WriteLine("İşlemler tamamlandı.\nProgramı kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        }

        private static void TestMethod() 
        { 
            Plane f16 = new Plane(); 
            Task[] tasks = new Task[5];

            object obj = new object();

            for (int i = 0; i < 5; i++) 
            { 
                tasks[i] = new Task(() => 
                { 
                    for (int j = 0; j < 1000; j++) 
                    { 
                        lock (obj) 
                        { 
                            f16.Altitude += j - 5; 
                        }                        
                    } 
                } 
                ); 
                tasks[i].Start(); 
            }

            Task.WaitAll(tasks);

            Console.WriteLine("[ {0} ]", f16.Altitude); 
        } 
    }

    class Plane 
    { 
        public int Altitude { get; set; } 
    } 
}
```

İlk olarak tüm Task örneklerinin, kritik bölgede işlem yapılırken lock bloğuna girdiklerini ifade edebiliriz. Bu lock bloğu içerisindeki çalışma süresi boyunca, yürütücü Task dışında başka bir Task örneğinin Altitude değerini değiştirmesine izin verilmemektedir. lock keyword’ ü kullanılırken object tipinden bir nesneyi parametre olarak alır. Bu nesne de aslında tüm Task örnekleri için ortaktır. Uygulamayı kaç kere çalıştırırsanız çalıştırın beklediğimiz aynı sonuçları elde ettiğimizi görebiliriz. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

[![blg212_Runtime1](images/blg212_Runtime1_thumb.gif)](images/blg212_Runtime1.gif)

Şimdi senaryomuzu biraz daha ilginçleştirelim. Bu sefer iki ayrı Task kümesinin, Altitude özelliği üzerinden farklı hesaplamalar yaptığını varsayacağız. Bu amaçla vaka kodumuzu aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System; 
using System.Threading.Tasks;

namespace SynchronizationPrimitives 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                TestMethod(); 
            }

            Console.WriteLine("İşlemler tamamlandı.\nProgramı kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        }

        private static void TestMethod() 
        { 
            Plane f16 = new Plane(); 
            Task[] taskSet1 = new Task[5]; 
            Task[] taskSet2 = new Task[5];

            for (int i = 0; i < 5; i++) 
            { 
                taskSet1[i] = new Task(() => 
                { 
                    for (int j = 0; j < 1000; j++) 
                    { 
                        f16.Altitude += j - 5; 
                    } 
                } 
                ); 
                taskSet1[i].Start(); 
            }

            for (int i = 0; i < 5; i++) 
            { 
                taskSet2[i] = new Task(() => 
                { 
                    for (int j = 0; j < 1000; j++) 
                    { 
                        f16.Altitude += j+7; 
                    } 
                } 
                ); 
                taskSet2[i].Start(); 
            }

            Task.WaitAll(taskSet1); 
            Task.WaitAll(taskSet2);

            Console.WriteLine("[ {0} ]", f16.Altitude); 
        } 
    }

    class Plane 
    { 
        public int Altitude { get; set; } 
    } 
}
```

Bu sefer taskSet1 ve taskSet2 isimli iki farklı Task dizisinin Altitude özelliği üzerinde gerçekleştirdiği farklı işlemler söz konusudur. Çok doğal olarak ortaklaşa kullanılan değişken söz konusu olduğundan çalışma zamanında aynı sonuçların elde edilmesi nadir bir durumdur. Uygulamanın aşağıdaki örnek çıktısında bu durum açık bir şekilde görülebilir.

[![blg212_Case](images/blg212_Case_thumb.gif)](images/blg212_Case.gif)

Bildiğiniz üzere her denemenin aynı sonucu üretiyor olmasını beklemekteyiz. Peki lock mekanizmasını bu tip senaryoda nasıl kullanabiliriz? Aşağıdaki kod parçasında bu çözümleme işlemi değerlendirilmektedir.

```csharp
using System; 
using System.Threading.Tasks;

namespace SynchronizationPrimitives 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                TestMethod(); 
            }

            Console.WriteLine("İşlemler tamamlandı.\nProgramı kapatmak için bir tuşa basınız"); 
            Console.ReadLine(); 
        }

        private static void TestMethod() 
        { 
            Plane f16 = new Plane(); 
            Task[] taskSet1 = new Task[5]; 
            Task[] taskSet2 = new Task[5];

            object obj = new object();

            for (int i = 0; i < 5; i++) 
            { 
                taskSet1[i] = new Task(() => 
                { 
                    for (int j = 0; j < 1000; j++) 
                    { 
                        lock (obj) 
                        { 
                            f16.Altitude += j - 5;    
                        }                        
                    } 
                } 
                ); 
                taskSet1[i].Start(); 
            }

            for (int i = 0; i < 5; i++) 
            { 
                taskSet2[i] = new Task(() => 
                { 
                    for (int j = 0; j < 1000; j++) 
                    { 
                        lock (obj) 
                        { 
                            f16.Altitude += j + 7; 
                        } 
                    } 
                } 
                ); 
                taskSet2[i].Start(); 
            }

            Task.WaitAll(taskSet1); 
            Task.WaitAll(taskSet2);

            Console.WriteLine("[ {0} ]", f16.Altitude); 
        } 
    }

    class Plane 
    { 
        public int Altitude { get; set; } 
    } 
}
```

Dikkat edileceği üzere ilk örneğimizdeki kodlama stilinden farklı bir uygulanış biçimi yoktur. Dikkat edilmesi gereken nokta ise, her iki lock bloğu içinde aynı object örneğinin kullanılmış olmasıdır.

## lock Aslında Montior Tipini Kullanır

Daha önceden de bahsettiğimiz üzere lock bloğu aslında Monitor tipinin ilgili metodlarını kullanmaktadır. Bu durum kodun arka planda üretilen IL çıktısında rahat bir şekilde görülebilir.

```text
.method public hidebysig instance void  '<TestMethod>b__3'() cil managed 
{ 
  // Code size       85 (0x55) 
  .maxstack  4 
  .locals init ([0] int32 j, 
           [1] bool '<>s__LockTaken1', 
           [2] object CS$2$0000, 
           [3] bool CS$4$0001) 
  IL_0000:  nop 
  IL_0001:  ldc.i4.0 
  IL_0002:  stloc.0 
  IL_0003:  br.s       IL_0048 
  IL_0005:  nop 
  IL_0006:  ldc.i4.0 
  IL_0007:  stloc.1 
  .try 
  { 
    IL_0008:  ldarg.0 
    IL_0009:  ldfld      object SynchronizationPrimitives.Program/'<>c__DisplayClass6'::obj 
    IL_000e:  dup 
    IL_000f:  stloc.2 
    IL_0010:  ldloca.s   '<>s__LockTaken1' 
    IL_0012:  call       void [mscorlib]System.Threading.Monitor::Enter(object, bool&) 
    IL_0017:  nop 
    IL_0018:  nop 
    IL_0019:  ldarg.0 
    IL_001a:  ldfld      class SynchronizationPrimitives.Plane SynchronizationPrimitives.Program/'<>c__DisplayClass6'::f16 
    IL_001f:  dup 
    IL_0020:  callvirt   instance int32 SynchronizationPrimitives.Plane::get_Altitude() 
    IL_0025:  ldloc.0 
    IL_0026:  ldc.i4.7 
    IL_0027:  add 
    IL_0028:  add 
    IL_0029:  callvirt   instance void SynchronizationPrimitives.Plane::set_Altitude(int32) 
    IL_002e:  nop 
    IL_002f:  nop 
    IL_0030:  leave.s    IL_0042 
  }  // end .try 
  finally 
  { 
    IL_0032:  ldloc.1 
    IL_0033:  ldc.i4.0 
    IL_0034:  ceq 
    IL_0036:  stloc.3 
    IL_0037:  ldloc.3 
    IL_0038:  brtrue.s   IL_0041 
    IL_003a:  ldloc.2 
    IL_003b:  call       void [mscorlib]System.Threading.Monitor::Exit(object) 
    IL_0040:  nop 
    IL_0041:  endfinally 
  }  // end handler 
  IL_0042:  nop 
  IL_0043:  nop 
  IL_0044:  ldloc.0 
  IL_0045:  ldc.i4.1 
  IL_0046:  add 
  IL_0047:  stloc.0 
  IL_0048:  ldloc.0 
  IL_0049:  ldc.i4     0x3e8 
  IL_004e:  clt 
  IL_0050:  stloc.3 
  IL_0051:  ldloc.3 
  IL_0052:  brtrue.s   IL_0005 
  IL_0054:  ret 
} // end of method '<>c__DisplayClass6'::'<TestMethod>b__3'
```

IL (Intermediate Language) koduna baktığımızda Monitor.Enter ve Monitor.Exit metod çağrılarının gerçekleştirildiği görülmektedir. Üstelik lock ifadesi içerisine alınan kod kısmı için arka planda bir try…finally bloğu oluşturulmuştur. Finally bloğunda gerçekleştirilen Monitor.Exit çağrısı, tahmin edileceği üzere her ne olursa olsun devreye girecektir. Tabi ki Monitor tipinin bilinçli olarak kullanılması gerektiği durumlarda söz konusu olabilir. Bu ve diğer durumları ilerleyen yazılarımızda ele almaya çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SynchronizationPrimitives.rar (22,26 kb)](assets/SynchronizationPrimitives.rar) [Örnek Visual Studio 2010 Ultimate sürümünde geliştirilmiş ve test edilmiştir]
