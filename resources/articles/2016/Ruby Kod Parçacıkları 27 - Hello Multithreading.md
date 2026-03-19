---
title: "Ruby Kod Parçacıkları 27 - Hello Multithreading"
pubDate: 2016-12-01 21:15:00
categories:
  - Ruby
tags:
  - Ruby
  - Threading
  - Concurrency
  - Performance
  - Shared State
  - Mutex
---

# Ruby Kod Parçacıkları 27 - Hello Multithreading
Merhaba Arkadaşlar,

Multithreading, programlamanın zor konularından birisidir. Aslında amaç işlemciye aynı andan birden fazla iş yaptırabilmek ya da bir işi parçalara bölerek eş zamanlı olarak yürütebilmektir. İşlemcinin bu farklı iş parçacıkları (Thread) arasında kısa sürelerde geçişler yapması sonucu istenen sonuçlara daha çabuk ulaşılması sağlanır. Bu, performans gerektiren bazı vakalarda oldukça önemlidir. Büyük veri kümelerinde arama yapan algoritmalar, yüksek boyutlu video görüntülerinin render eden programla, çeşitli matematik problemleri, oyun programlama ve daha pek çok alanda çoklu iş parçacığı tekniklerine başvurulur.

![rubythread_7.gif](images/rubythread_7.gif)

Elbette çoklu iş parçacıkları ile uğraşırken dikkat edilmesi gereken hususlar da vardır. Ortak veri kümeleri üzerinde farklı iş parçacıklarının çalışması sırasında veri bütünlüğünün bozulması, iş parçacıklarının kilitlenmesi (deadlock), kötü tasarım sonucu işlemlerin çabuk bitmesi gerekirken daha uzun sürmesi, CPU peek-time değerlerinin tavan yapması ve soğutma yükü sonucu elektrik maliyetlerinin artması (eğer sunucularda yaptırdığımız çok parçacıklı işlemler söz konusu ise bu ciddi bir sorun olabilir), iş parçacıkları içerisinden ele alınmamış hatalar fırlaması sonucu yorumlayıcının kesimlesi ve benzeri sıkıntılar oluşabilir. Dolayısıyla iş parçacıklarını kodlarken dikkatli olunması gerekir. Lakin bir yerden de işe başlamamız gerekiyor. Öyleyse gelin Ruby dilinde Multithreading programlamaya bir merhaba diyelim.

## Thread Yaşam Döngüsü

Pek çok programlama dilinde olduğu gibi Ruby'de de bir ana iş parçacığı vardır (Main Thread) Bu iş parçacığı ile birlikte çalışacak farklı iş parçaları oluşturmak için Thread sınıfının new metodundan yararlanılır. Bu metod bir kod bloğu alır ve ilgili blok içerisindeki işlerin ana iş parçacığı haricinde farklı bir iş parçacığı olarak çalıştırılmasını sağlar. Aşağıdaki kod parçasında temel anlamda Thread yapısının yaşam döngüsü incelenmeye çalışılmıştır.

```text
puts "Main Thread #{Thread.current}\t#{Time.now}"

t1=Thread.new{
	puts "t1 #{Thread.current}\t#{Time.now}"
	sleep 2
	puts "t1 #{Thread.current}\t#{Time.now}"
}

t2=Thread.new do
	puts "t2 #{Thread.current}\t#{Time.now}"
	sleep 4
	puts "t1 #{Thread.current}\t#{Time.now}"
end

t1.join
t2.join
puts "Main thread #{Thread.current}\t#{Time.now}"
```

![rubythread_1.gif](images/rubythread_1.gif)

Uygulamada dikkat edilmesi gereken önemli noktalar var. Ana iş parçacığı (Main Thread) dışında iki iş parçacığı daha çalışmaktadır. Bunlardan t1 içerisinde sembolik olarak 2 saniyelik gecikme uygulanmıştır. t2 için bu süre 4 saniyedir. Kod akışı hemen Thread.new satırları sonrasında devam edeceğinden join metodunun kullanılmaması halinde uygulama anında sonlanır. Bu nedenle ana iş parçacığına diğerlerini beklemesi söylenmektedir. O an çalışmakta olan Thread'in Ruby tarafından üretilen benzersiz nesne numarasına erişmek için current isimli sınıf metodundan yararlanılmıştır. Normalde bu sınıf metodu, oluşturulan Thread nesne örneğinin kendisini döndürmektedir. Dolayısıyla ekrana basılması halinde nesne numarası elde edilir.

Dikkat edileceği üzere kodun ilk ve son satırlarında elde edilen nesne örnekleri Main Thread'e aittir ve aynıdır. t1 ve t2 içerisinden ulaşılan Thread örnekleri ise tamamen farklıdır. new metodundan sonraki kod bloğunu süslü parantezlerle verebileceğimiz gibi do end ifadesi ile de gönderebiliriz (new dışında start ve fork metoduları ile de thread başlatılabilir)

## Değişken Kullanımı

Thread'ler de değişken kullanımları da oldukça kolaydır. Bazı hallerde farklı iş parçacıklarının aynı değişken değerleri ile çalışması gerekebilir. Ruby dilinde bunu sağlamak çokta zor değildir. Aşağıdaki basit kod parçasını göz önüne alalım.

```text
total=0
threads=[]

5.times{
	threads<<Thread.new{
		sleep(rand(0.1)/10.0)		
		total+=rand(10)
		Thread.current["forThis"]=total
	}
}
threads.each{|t|
		t.join
		puts t["forThis"]
}
puts "Total = #{total}"
```

![rubythread_2.gif](images/rubythread_2.gif)

Örnek kod parçasında 5 farklı iş parçacığı oluşturulmaktadır. Oluşturulan iş parçacıkları << operatörü ile threads isimli diziye eklenmektedir. Her bir Thread bloğu içerisinde rastgele bir süre üretilmekte ve Thread'in o süre boyunca bekledikten sonra işine devam etmesi sağlanmaktadır. Amaç, uzun süren bir işi canlandırmaktır. Nitekim asıl önemli olan nokta blok içerisinde [] operatörleri ile ulaşılan forThis anahtarıdır (Aslında bir hash veri yapısına ulaştığımızı düşünebiliriz) Thread.current üzerinden yapılan bu işlem ile sadece o Thread nesnesi için geçerli olan bir anahtar tanımı ve değer ataması söz konusu olur (forThis anahtarı ThreadLocal olarak adlandırılan değere ulaşılmasını sağlamaktadır) Yani forThis her Thread nesnesinin kendisine özel değer taşır.

Örnek kod parçasında her bir iş parçacığı total değişkeninin rastgele arttırılmış bir değerini barındırır. Bu değer threadlocal değeri olarak atanır. Tabi iş parçacıklarının bu şekilde başlatılmasını takiben ana iş parçacığının da ilgili işlemlerin bitmesini beklemesi gerekir. Bunun için dikkat edileceği üzere threads dizisinde each bloğu kullanılmış ve t nesne örnekleri üzerinden join metoduna çağrılar gerçekleştirilmiştir.

t["forThis"] kullanımı ile o anki iş parçacığının sahip olduğu total değeri ekrana basılır. Tahmin edileceği üzere son satırda ekrana basılan toplam değeri, iş parçacıkları içerisinde en son tamamlanan iş parçacığında atanan değer olacaktır. Burada önemli olan bir diğer nokta da, tüm iş parçacıklarının total isimli değişkene erişebilmeleridir. Nitekim bu değişken Thread blokları dışında tanımlıdır.

## Öncelikler (Priority)

Bir başka kod parçası ile yolumuza devam edelim. Bu kez iş parçacıklarının önceliklerini anlamaya çalışacağız. Normalde Main Thread varsayılan olarak 0 priority değeri ile başlatılır. İstersek diğer iş parçacıklarının önceliklerini değiştirebiliriz. Yüksek önceliğe sahip iş parçacıkları diğer düşük öncelikli iş parçacıklarına nazaran daha çok çalıştırılırlar. Bu durumu aşağıdaki kod parçası ile daha iyi anlayabiliriz.

```text
for tryCount in 1..5
	total1=total2=0
	thread1=Thread.new{
		while true
			total1+=1
		end
	}
	thread1.priority=5
	thread2=Thread.new{
		while true
			total2+=1
		end
	}
	thread2.priority=-5
	sleep 1
	puts "Total1=#{total1}\nTotal2=#{total2}"
	puts "#{format("%.5f", (total1.to_f/total2.to_f))}\n\n"
end
```

Örnekte iki Thread nesne örneği oluşturulmuştur. Thread bloklarında total1 ve total2 isimli değişkenler 1er 1er arttırılmaktadır. Kritik olan nokta priority değerleridir. İlk thread için 5 ikinci thread içinse -5 değeri verilmiştir. Buna göre ilk thread işlemci için daha önceliklidir. Yani işlemci, thread1 içerisindeki bloğu thread2'ye göre daha sık çalıştıracaktır. Sonuç olarak hesaplanan toplam değerler arasında katsayı farkı oluşmuştur (Bu bir ispattır aslında) Aşağıdaki ekran görüntüsü kodu yazdığım makinenin ürettiği sonuçlardır. Dikkat edileceği üzere thread1 tarafından üretilen toplam değerleri thread2 tarafından üretilenlere göre belirgin olarak daha farklıdır.

![rubythread_3.gif](images/rubythread_3.gif)

Öncelikleri yakınlaştırdıkça sonuçların değiştiği görülür. Örneğin ilk thread için önceliği 1 ikinci thread içinse -1 şeklinde belirlersek sayılar birbirlerine daha yakın çıkacaktır. Kendi sistemimde elde ettiğim sonuçlar aşağıdaki gibidir (intel core i7-4600U 2.10 Ghz)

![rubythread_4.gif](images/rubythread_4.gif)

## Exception Yönetimi

Multithreading ile ilgili bir diğer kritik konuda bloklar içerisinde oluşacak hatalarda sistemin nasıl davranış sergilediğidir. Yani bir iş parçacığı içerisinden ortama fırlayacak bir hata söz konusu olursa ne olur? Normal şartlarda bir iş parçacığında istisna oluşursa sadece bu iş parçacığı sonlanır ve kalanlar yaşamaya devam eder. Öncelikle bu vakayı ele alalım. İşte kod parçamız.

```text
threads=[]
threads<<Thread.new{
	puts "thread 1 is running"
	sleep 10
	puts "end of thread 1"
}
threads<<Thread.new{
	puts "thread 2 is running"
	sleep 3
	puts "end of thread 2"
	}
threads<<Thread.new{
	puts "thread 3 is running"
	sleep 1
	raise ArgumentError,"Some argument error"
	puts "end of thread 3"
}
puts "This is main thread"
threads.each{|t|t.join}
puts "end of main thread"
```

Kodu yorumlamadan önce neler yaptığımıza bir bakalım. Üç iş parçacığımız var. İş parçacıkları içerisinde sleep metodu ile belirli süreler boyunca duraksama yapıyoruz. Bu şekilde uzun süren işleri temsil ettiğimizi düşünebiliriz. thread1 ve thread2 sorunsuz çalışacak iş parçacıkları. Ne var ki 3ncü thread daha ilk saniyede ArgumentError fırlatıyor (Oyunbozan Thread) Tüm iş parçacıklarını join ile ana iş parçacığımıza eklemeyi de ihmal etmiyoruz. İşler çalışma zamanında ilginçleşiyor. İşte ekran görüntümüz.

![rubythread_5.gif](images/rubythread_5.gif)

Dikkat edileceği üzere thread 1 ve thread 2 başarılı bir şekilde işlemlerini tamamlamıştır. Bu iş parçacıkları 1nci saniyede ortama ArgumentError fırlatan thread 3'ten daha uzun sürmelerine rağmen işlemlerini tamamlamışlardır. Lakin ilgili hata mesajı Main Thread üzerinde ele alınmadığından onun işleyişi tüm diğer iş parçacıkları tamamlandıktan sonra kesilmiştir.

Tabii bazı durumlarda bir iş parçacığı içerisinde oluşacak hatanın diğer bağımsız çalışan iş parçacıklarının işleyişini de kesmesi istenebilir. Bu durumda abort_on_exception değerine true atanması yeterlidir. Yukarıdaki kod parçasını aşağıdaki gibi düzenleyerek devam edelim.

```text
threads=[]
threads<<Thread.new{
	puts "thread 1 is running"
	sleep 10
	puts "end of thread 1"
}
threads<<Thread.new{
	puts "thread 2 is running"
	sleep 3
	puts "end of thread 2"
	}
threads<<Thread.new{
	puts "thread 3 is running"
	sleep 1
	raise ArgumentError,"Some argument error"
	puts "end of thread 3"
}
threads.last.abort_on_exception=true
puts "This is main thread"
threads.each{|t|t.join}
puts "end of main thread"
```

Önce çalışma zamanı çıktısına bir bakalım.

![rubythread_6.gif](images/rubythread_6.gif)

Önceki örnek ile arada önemli bir fark var. Daha ilk saniyede fire veren iş parçacığımız diğerlerinin de işlerini kesmesine ve ana iş parçacığına düşülmesine sebep olmuştur. Bu durum abort_on_exception niteliğine true değeri verilmesi nedeniyle gerçekleşmiştir. Pek tabii ana iş parçacığında ilgili hata ele alınmadığından o da son satırını işletemeden sonlanmıştır (Garibim Main Thread bir türlü son satırı yazdıramadı)

Yazımızı sonlandırmadan önce Thread sınıfına ait pek çok fonksiyonlik söz konusu olduğunu da ifade etmeliyim. [Detaylı bilgi için şu adrese bakabilirsiniz](https://ruby-doc.org/core-2.2.0/Thread.html) (Hatta bakın. Nitekim Thread'lerin durumlarını/state nasıl öğrenebileceğimizi, istediğimizde onları nasıl durdurabileceğimizi öğreten bilgiler yer alıyor) Görüldüğü üzere Ruby dilinde çoklu iş parçacıkları oluşturmak ve onları yönetmek oldukça basittir. Elbette çok daha ileri seviye konular da var. Örneğin Mutex ve DeadLock gibi vakaları yeni yeni öğreniyorum. Öğrendikçe sizlere paylaşmaya devam edeceğim. Şimdilik benden bu kadar. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
