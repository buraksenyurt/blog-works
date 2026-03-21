---
layout: post
title: "TPL Senkronizasyonu Sağlamak – 2 (Interlocked)"
date: 2011-01-31 13:45:00 +0300
categories:
  - parallel-programming
tags:
  - task-parallel-library
  - synchronization-primitives
  - parallel-programming
  - .net-framework
  - csharp
  - interlocked
---
“Seçimi zaten yaptın. Şimdi onu anlaman gerekli.” Sanırım Matrix filminde Neo ile Oracle’ ın felsefe içeren ve uzun uzun düşünüldüğünde akla son derece anlaşılır gelen bir kaç sohbetinde geçen repliklerden birisi de buydu. Aslında ben bunu kendi hayatımda zaman zaman “Çözümü uyguladın. Şimdi onun her parçasının ne anlama geldiğini öğrenmen gerekli” diye çeviriyorum.

[![blg213_Giris](/assets/images/2011/blg213_Giris_thumb_1.jpg)](/assets/images/2011/blg213_Giris_1.jpg)


Yazımızın giriş kısmını oluşturan bu paragrafın üretiliş amacı ise en sonda geliştireceğimiz örnek olacak aslında. Gerçekten de bazen çeşitli vakaların çözüme ulaşmasında, gerekli teknikleri uyguladıktan sonra onları anlamaya çalışmak daha öğretici olabilmekte. Merak ediyor musunuz?

Hatırlayacağınız üzere [TPL Senkronizasyonu Sağlamak – 1](https://www.buraksenyurt.com/post/TPL-Senkronizasyonu-Saglamak) başlıklı yazımız ile Task Parallel Library (TPL) tarafında senkronizasyon kullanımını incelemeye başlamıştık. Aslında başımıza iş mi açtık bilemiyorum ama sonuç itibariyle kritik bir konu olduğunda sanıyorum ki hepimiz hem fikiriz. Önceki yazımızda değerlendirdiğimiz senaryoda, lock keyword kullanımı ile izole edilmiş bir veri alanının, farklı iş parçaları tarafından nasıl güvenli bir şekilde kullanılabileceğini analiz etmiştik. Üstelik bu keyword’ ün aslında arka planda Monitor tipini kullandığını da Intermediate Language (IL) kodunda görmüştük. Elbette iş parçalarının senkronizasyonu için kullanılabilecek farklı tipler de söz konusudur. Interlocked sınıfı gibi.

Interlocked Kullanımı

System.Threading isim alanı altında yer alan Interlocked tipine ait atomic metodlar işletim sisteminin ve donanımın özelliklerini kullanarak senkronizasyonu daha yüksek performans ile kullanmayı vaat ederler. Aşağıdaki sınıf diagramında Interlocked tipinin üyleri görülmektedir.

[![blg213_Interlocked](/assets/images/2011/blg213_Interlocked_thumb.gif)](/assets/images/2011/blg213_Interlocked.gif)

Interlocked tipi şekilden de görüleceği üzere static bir sınıftır (Dolayısıyla sadece static üyeler içerebilir ve örneklenemez) Temel olarak Add,CompareExchange, Decrement,Exchange,Increment ve Read metodları ile bunların aşırı yüklenmiş (Overloaded) versiyonlarını içermektedir. Metodların genel yapısı incelendiğinde atomic fonksiyonların int, long,double, float, object gibi tipler ile çalıştığı görülmektedir. Dilerseniz metodların işlevsellikleri hakkında kısa bilgiler vererek ilerlemeye çalışalım.

Exchange metodu ile bir değer ataması yapılabilmektedir. Exchange metodu dikkat edileceği üzere double, float, IntPtr, int, long ve object tipleri ile çalışmaktadır. Ayrıca generic bir versiyonu da bulunmaktadır. Bunun dışında 1er arttırma ve azaltma işlemleri için kullanılabilecek basit Increment ve Decrement metodları mevcuttur. Ayrıca Int32 veya Int64 tipinden sayısal değerleri toplayan Add metoduna da sahiptir. Interlocked tipinin kullanışlı olan metodlarından birisi de CompareExchange fonksiyonudur. Bu fonksiyon, parametre olarak gelen sayısal değerlerin eşitliğini kontrol etmekte ve aynı iseler Exchange işlemini gerçekleştirmektedir. Aşağıdaki kod parçasında Interlocked için örnek bir kullanım söz konusudur.

```csharp
using System; 
using System.Threading; 
using System.Threading.Tasks;

namespace TPLSynchronization 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            for (int i = 0; i < 10; i++) 
            { 
                TestMethod(); 
            }   
        }

        static void TestMethod() 
        { 
            Task[] tasks = new Task[5]; 
            Plane f14Tomcat = new Plane();          

            for (int i = 0; i < 5; i++) 
            { 
                tasks[i] = new Task(() => 
                    { 
                        for (int j = 0; j < 12500; j++) 
                        { 
                            Interlocked.Exchange(ref f14Tomcat.Altitude, (j+1)* 10); 
                        }                        
                    } 
                ); 
            }

            foreach (Task task in tasks) 
            { 
                task.Start(); 
            }

            Task.WaitAll(tasks);

            Console.WriteLine("{0}",f14Tomcat.Altitude); 
        }

        class Plane 
        { 
            public int Altitude; 
        } 
    } 
}
```

Plane sınıfı içerisinde Altitude isimli int tipinden bir Alan (Field) yer almaktadır. Bu değer ilgili Task örnekleri tarafından izole edilen yerel değişken olarak ele alınmaktadır. Dikkat edileceği üzere Task örneklerinin oluşturulduğu ifade içerisinde yer alan kod bloğundan Exchange metodu kullanılarak Altitude değerinin (j+1) 10 birim kadar arttırılması sağlanmaktadır. Bu işlem 5 ayrı Task tarafından gerçekleştirilmektedir ve bildiğiniz üzere senkronizasyon için bir tedbir alınmadığı takdirde, her bir deneme de sonuçlar farklı olacaktır. Örnek uygulamanın çalışma zamanı çıktısı ise aşağıdaki gibidir.

[![blg213_Test1](/assets/images/2011/blg213_Test1_thumb.gif)](/assets/images/2011/blg213_Test1.gif)

Görüldüğü üzere TestMethod için yapılan 10 ayrı denemenin sonucuda aynıdır. Bu örnekte Exchange metodu kullanılmıştır. Ancak bazen çok daha basit işlemler söz konusu olabilir. Söz gelimi 1er arttırma veya azaltma gibi. Bu durumda Interlocked tipinin static Increment veya Decrement metodlarını kullanaraktan da gerekli senkronizasyonu sağlayabiliriz. Örneğimizde Altitude değerini 10000 kez 1er birim arttırmak için yapmamız gereken tek şey kodda aşağıdaki değişikliği yapmaktan ibarettir.

[![blg213_Test2](/assets/images/2011/blg213_Test2_thumb.gif)](/assets/images/2011/blg213_Test2.gif)

Bu kodun çalışma zamanı çıktısı ise aşağıdaki gibi olacaktır.

[![blg213_Test2Runtime](/assets/images/2011/blg213_Test2Runtime_thumb.gif)](/assets/images/2011/blg213_Test2Runtime.gif)

Yanlız Interlocked tipinin atomic fonksiyonları göz önüne alındığında üzerinde işlem yapılan asıl değişkenlerin ref tipinden aktarıldığı görülmektedir. Bu durumda Plane tipinin Altitude isimli alanının (Field), özellik (Property) olarak tasarlanması bir sorun teşkil etmektedir.

[![blg213_Error](/assets/images/2011/blg213_Error_thumb.gif)](/assets/images/2011/blg213_Error.gif)

Bu durumda Altitude özelliğini local bir değişken olarak ele almak ve sonrasında Interlocked tipinin ilgili fonksiyonlarında kullanmak gerekmektedir. Bu amaçla kod parçasını aşağıdaki gibi değiştirebiliriz.

```csharp
static void TestMethod() 
{ 
	Task[] tasks = new Task[5]; 
	Plane f14Tomcat = new Plane(); 
	int altitude = f14Tomcat.Altitude;

	for (int i = 0; i < 5; i++) 
	{ 
		tasks[i] = new Task(() => 
			{ 
			   int result = 0; 
				for (int j = 0; j < 12500; j++) 
				{ 
					result=Interlocked.Increment(ref altitude); 
					//Interlocked.Exchange(ref f14Tomcat.Altitude, (j+1)* 10); 
				} 
				f14Tomcat.Altitude = result; 
			} 
		); 
	}

	foreach (Task task in tasks) 
	{ 
		task.Start(); 
	}

	Task.WaitAll(tasks);

	Console.WriteLine("{0}",f14Tomcat.Altitude); 
}
```

Burada dikkat edilmesi gereken noktalardan birisi de, Increment metodunun geriye bir sonuç döndürüyor olmasıdır. Bu aslında yapılan arttırma işleminin bir sonucudur. Dolayısıyla result değerini tekrardan Altitude özelliğine atamak yeterlidir. İşte çalışma zamanı sonuçları.

[![blg213_Test3](/assets/images/2011/blg213_Test3_thumb.gif)](/assets/images/2011/blg213_Test3.gif)

Görüldüğü gibi yine aynı sonuçlar üretilmiştir. Ancak!!!! Dikkatli olmamız da yarar vardır. Son örneğimizdeki yerel değişken taktiği gerçekten doğru mudur? Bu kullanımı Increment yerine Exchange metodu içinde yaptığımızı düşünelim.

```csharp
static void TestMethod() 
{ 
	Task[] tasks = new Task[5]; 
	Plane f14Tomcat = new Plane(); 
	int altitude = f14Tomcat.Altitude;

	for (int i = 0; i < 5; i++) 
	{ 
		tasks[i] = new Task(() => 
			{ 
				int result = 0; 
				for (int j = 0; j < 12500; j++) 
				{ 
					result=Interlocked.Exchange(ref altitude, (j+1)* 10); 
				}                        
				f14Tomcat.Altitude = result; 
			} 
		); 
	}
```

Ve çalışma zamanı sonuçları.

[![blg213_Test4_1](/assets/images/2011/blg213_Test4_1_thumb.gif)](/assets/images/2011/blg213_Test4_1.gif)

Tüm denemelerin aynı sonucu üretmesi gayet güzel. Ancak aynı senaryonun Field içeren kullanımında elde ettiğimiz sonuçlar 125000 dir. Oysaki aynı sonuçları üretmeleri beklenmektedir. Öncelikle aynı sonuçları üretecek kod parçasını geliştirelim. Bu amaçla kodumuzu aşağıdaki gibi güncelleştirmemiz gerekmektedir.

```csharp
static void TestMethod() 
{ 
	Task[] tasks = new Task[5]; 
	Plane f14Tomcat = new Plane(); 
	int altitude = f14Tomcat.Altitude;

	for (int i = 0; i < 5; i++) 
	{ 
		tasks[i] = new Task(() => 
			{ 
				int firstAltitude = f14Tomcat.Altitude; 
				int localAltitude = firstAltitude;

				for (int j = 0; j < 12500; j++) 
				{ 
					localAltitude = (j + 1) * 10;                         
				} 
			   int sharedAltitude=Interlocked.CompareExchange(ref altitude, localAltitude, firstAltitude); 
		  f14Tomcat.Altitude=sharedAltitude;                      
			} 
		); 
	}

	foreach (Task task in tasks) 
	{ 
		task.Start(); 
	}

	Task.WaitAll(tasks);

	Console.WriteLine("{0}",f14Tomcat.Altitude); 
}
```

Kodu bu şekilde çalıştırdığımızda istediğimiz sonuçları elde ettiğimizi görebiliriz.

[![blg213_Test4_2](/assets/images/2011/blg213_Test4_2_thumb.gif)](/assets/images/2011/blg213_Test4_2.gif)

Koda baktığımızda Task örneklenmesi sırasında firstAltitude ve localAltitude isimli iki değişken üretildiği görülmektedir. Bu değişkenlerden localAltitude aslında yerel değişken olarak kullanılmaktadır. Bir başka deyişle sadece tanımlandığı Task örneğine ait bir değişkendir. Ancak bir de 5 Task örneğinin ortaklaşa kullandıkları ve asıl amacımız olan Altitude özelliğinin değeri söz konusudur. Bu nedenle her bir Task’ in paylaşılan değişkenin son değerini alabilmesi içinde sharedAltitude isimli değişken kullanılmaktadır.

Task gövdesi içerisinde yer alan for döngüsü dikkat edileceği üzere yerel değişkeni arttırmaktadır. for döngüsünü takip eden satırda ise CompareExchange metoduna bir çağrıda bulunulduğu görülmektedir. Bu metod, paylaşımlı verinin güncellenmesi amacıyla kullanılmaktadır. Burada paylaşımlı veri ile başlangıçtaki değer arasında bir kıyaslama yapılır. Örneğimize göre eğer localAltitude ve altitude değerleri birbirlerine eşitse, localAltitude’ un anlık güncel değeri altitude içerisinde saklanır. Aksi durumda herhangibir operasyon gerçekleştirilmez.

Burada karşılaştırma ve veri değiş tokuşu fonksiyonellikleri söz konusudur ve her ikisi de aslında tek bir atomic operasyon olarak ele alınmaktadır. CompareExchange metodunun dönüşü aslında metoda gelen ilk parametrenin orjinal değeridir. Aslında Interlocked tipinin bu efsane kullanımını daha net kavramak adına [MSDN](http://msdn.microsoft.com/en-us/library/801kt583.aspx) ‘ de ilgili içeriğe bakmakta yarar vardır. Ben sadece kapıyı gösterebiliyorum. Oradan geçmesi gereken kişi sizsiniz.

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[TPLSynchronization_Interlocked.rar (23,07 kb)](/assets/files/2011/TPLSynchronization_Interlocked.rar) [Örnek Visual Studio 2010 Ultimate Sürümünde geliştirilmiş ve test edilmiştir]