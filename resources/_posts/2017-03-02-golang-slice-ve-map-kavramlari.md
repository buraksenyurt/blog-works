---
layout: post
title: "GoLang - Slice ve Map Kavramları"
date: 2017-03-02 21:23:00 +0300
categories:
  - golang
tags:
  - golang
  - slice
  - append
  - capacity
  - copy
  - len
  - map
  - array
---
Gopher olma çalışmalarım iş yoğunluğuna bağlı olarak zaman zaman hızlı ve zaman zaman da yavaş bir şekilde devam ediyor. Açıkçası Gopher karakterini ve internetteki örneklerini çok sevdim. Google oldukça sevimli bir logo yaratmış. Bu nedenle her bölüm mümkün mertebe farklı bir Gopher'ı ekranlarınıza taşımaya çalışıyorum.

![gobanana.gif](/assets/images/2017/gobanana.gif)

Efendim gelelim sadede. Go dilinde kullanılan türlerden olan Array tipini kısaca inceledikten sonra geçtiğimiz günlerde Slice ve Map kavramlarını da incelemeye başladım. Her ikisi de sağladıkları esneklikler açısından dil içerisinde oldukça önemli bir yere sahipler. Bu makalemizde ilgili veri tiplerini basit örnekler üzerinden tanıma gayretinde olacağız.

## Slice

Diziler (Arrays) pek çok programlama dilinde olduğu gibi Go için de vazgeçilmez veri türlerinden birisi. Lakin esnek olmadığı noktalar da vardır. Örneğin diziler sabit uzunluktadır ve boyutları yeni ilaveler ile dinamik olarak genişletilemez. İşin aslı dizi dediğimiz kavram sabit uzunlukta, belirli türde, indisle erişilen ve sıralanmış bir veri yapısını işaret eder. Diğer yandan slice veri türü daha esnek bir kullanım alanı sunar. Hatta dizilerin belli aralıktaki parçalarını birer slice haline getirip kullanabiliriz. Go dilinde ustalaşanlar dizi değişkenleri yerine slice türü ile çalışmayı tercih etmektedirler. Tanımlanan bir slice eleman sayısı ve kapasite ile birlikte oluşturulabilir. append fonksiyonu yardımıyla yeni elemanlar eklenebilir, copy fonksiyonu ile bir slice içeriğinin bir diğerine kopyalanması sağlanabilir. Esas itibariyle bir slice aşağıdaki basit şekil ile ifade edilebilir.

![slice_1.gif](/assets/images/2017/slice_1.gif)

Tanımlanan bir slice değişkeni içerisinde belli türden elemanlar barındıran diziyi işaret eden bir pointer, bu dizinin uzunluğu ve kapasitesi yer alır. Yukarıdaki şekilde 5 adet integer eleman içeren maksimum kapasitesi de 10 olan numbers isimli bir slice tanımının bellekteki farazi gösterimi yer almaktadır.

Bir slice'dan vereceğimiz değer aralığına göre başka bir slice da oluşturabiliriz. Ancak burada dikkat edilmesi gereken önemli bir husus vardır. Oluşan yeni slice alt kümesi olduğu slice içerisindeki dizi elemanlarını işaret eder. Bunun anlamını biliyorsunuz. Yeni oluşan slice elemanlarında yapılacak olan değişiklikler alt kümesi olduğu slice için de geçerli olacaktır. Aşağıdaki şekilde bu durum özetlemektedir.

![slice_2.gif](/assets/images/2017/slice_2.gif)

Şimdi basit bir kod parçası ile slice tipini nasıl kullanabileceğimize bakalım.

```text
package main

import (
	"fmt"
	"math"
	)
	
func main(){
	words:=[]string{"red","plane","colors","car"}
	fmt.Println(words)
	fmt.Println(words[1])
	
	var sumBytes []int // Slice tanimi
	sumBytes=make([]int,4,10) // 4 eleman icerek sekilde slice'in kurgulanmasi
	fmt.Printf("Length=%d,Capacity=%d\n",len(sumBytes),cap(sumBytes))
	fmt.Println(sumBytes)
	for i:=0;i<cap(sumBytes);i++{
		if i>=len(sumBytes){
			sumBytes=append(sumBytes,i*3)
			}else{
				sumBytes[i]=i*2
				}
	}
	fmt.Println(sumBytes)
	
	// slice'dan slice alinmasi halinde ortak degiskenlerin durumu
	subSumBytes:=sumBytes[3:6]
	subSumBytes[0]=-1
	subSumBytes[1]=-1
	subSumBytes[2]=-1
	fmt.Println(sumBytes)
	fmt.Println(subSumBytes)
	
	// diziden slice cikartmalar
	points:=[6]int{3,5,6,-4,-18,20}	 // 6 elemanli array alttaki birkac slice icin kullanilacak
	subPoints1:=points[0:3]
	fmt.Println(subPoints1)	
	subPoints2:=points[3:len(points)]
	fmt.Println(subPoints2)
	subPoints3:=points[:]
	fmt.Println(subPoints3)
	subPoints4:=points[4:]
	fmt.Println(subPoints4)
	
	// append
	newSlice:=make([]float32,5,10) // 5 elemanli ve max 10 kapasiteli slice tanimi
	newSlice[0]=1.32	
	newSlice=append(newSlice,math.Pi,2.277) // Slice'in sonuna eleman ekledi
	fmt.Println(newSlice)
	
	// Copy
	newSlice1:=[]int{3,5,1,7,8} // 5 elemanli ve kapasiteli bir slice
	newSlice2:=make([]int,3) // 3 elemanli ve kapasiteli bir slice
	copy(newSlice2,newSlice1)
	fmt.Println(newSlice1)
	fmt.Println(newSlice2)
}
```

Öncelikle kodun çalışma zamanı çıktısına bir bakalım ve içeriği üzerinden konuşalım.

![slice_3.gif](/assets/images/2017/slice_3.gif)

İlk olarak en basit haliyle words isimli bir slice tanımlayarak başlıyoruz. Eleman sayısını belirtmediğimiz bu slice'ın içereceği string değerleri ilk ifadede atamaktayız. Sonrasında tüm ve sadece 1nci indisteki elamanları ekrana yazdırıyoruz. Bir başka deyişle aynı dizilerde olduğu gibi slice elemanlarına da indis operatörü ile erişebiliyoruz.

sumBytes integer değerler taşıyacak bir slice. İlk satırda eleman sayısını vermeden tanımlıyor sonraki satırda make fonksiyonunu kullanarak oluşturuyoruz. Bunu programın başında tanımlı bir slice değişkeninin ilerleyen kısımlarda oluşturabileceği manasında yorumlayabiliriz. sumBytes 4 eleman içerecek şekilde ve 10 birim kapasiteye sahip olacak şekilde tanımlanmış durumda. Slice kapasitesi ve uzunluğunu bulmak için cap ve len isimli fonksiyonlardan yararlanıyoruz. sumBytes elemanlarını ekrana yazdırdığımızda sadece 4 adet 0dan oluşan bir dizi ile karşılaşmaktayız ki bu son derece doğaldır.

Sonrasında gelen for döngüsü ise dikkate değer. Burada ilk 4 elemana indisin iki katını işaret eden sayılar ekliyoruz. Ancak i elemanı belirtilen uzunluktan fazla ise append fonksiyonuna başvuruyoruz. Neden? sumBytes için belirlediğimiz eleman uzunluğu 4. Kapasite ise 10. Buna göre örneğin 5nci elemana bir değer atamak istersek çalışma zamanında aşağıdaki ekran görüntüsünde olduğu gibi "index out of range" hatasını alırız.

![slice_4.gif](/assets/images/2017/slice_4.gif)

Bu yüzden append fonksiyonundan yararlanmaktayız.

subSumBytes ise sumBytes'ın bir alt kümesi gibi düşünülebilir. Bu da bir slice değişkenidir ve aslında sumBytes'ın 3ncü indisinden itibaren 4ncü ve 5nci dahil olmak üzere 6ncı indisine kadar olan elemanları işaret etmektedir. Dikkat edilmesi gereken nokta subSumBytes elemanlarında yapılan değişikliklerin doğal olarak sumBytes'ı da etkilemiş olmasıdır (-1 atamalarına dikkat edelim)

points bir array olarak tanımlanmıştır. Eleman sayısı belirli ve dizi uzunluğu sabittir. subPoints1,2,3 ve 4 bir diziden nasıl parça alınabileceğinin bir kaç örneğini barındırmaktadır.: işaretinin sol ve sağ tarafındaki girdilere göre dizinin belli bir parçasının veya tamamının ([:] kullanımı) slice olarak elde edilmesi mümkündür.

newSlice değişkeninde append fonksiyonunun kullanımı bir kez daha örneklenmiştir. 5 eleman uzunluğunda bir tanımlama yapıldığından append fonksiyonu 5nci indisteki eleman olarak dizideki yerini alacaktır. Bu nedenle [1.32 0 0 0 0 3.1415927 2.277] şeklinde bir sonuç oluşmaktadır.

Örnek kodun son satırlarında bir slice içeriğinin (newSlice1) bir diğerine (newSlice2) kopyalanması işlemine yer verilmiştir. Burada kapasite ve eleman sayısı 5 olan bir slice'ın, 3 elemen ve kapasiteli bir slice'a kopyalanması söz konusudur. Çok doğal olarak bir trim işlemi gerçekleşir ve newSlice1'in sadece ilk 3 elemanı newSlice2'ye kopyalanır.

## Map

Gelelim bir diğer konumuz olan map veri türüne. Bu veri türünü key-value çiftleri şeklinde elemanlar barındıran bir veri yapısı olarak düşünebiliriz (Aslında Hash veri yapısının Go dilindeki built-in karşılığıdır) map veri yapısı da aynen slice ve array gibi referans türlüdür. Aynen slice kullanımında olduğu gibi make fonksiyonu ile de oluşturulabilir yada tek ifade ile elemanları atanabilir. Bir map kendisinde key-value çiftleri eklendikçe otomatik olarak büyür. map'lere eklenen şekilde bir sırlama da oluşmaz. Yani bir map içeriğini dolaştığımızda elemanlarını eklediğimiz sırada bulamayabiliriz.

key olarak tutulan veri türünün karşılaştırılabilir (comparable) olması gerekir (.net tarafında olsak IComparable gibi arayüzleri implemente eden türleri key olarak kullanabiliriz gibi bir cümleyi sarfedebilirdim lakin Go'da bu durum nasıl ele alınıyor henüz bilmiyorum. İlk okuduğum yazılara göre temel veri türlerinin key olarak kullanılabileceği yönünde) map türü Concurrent kullanımlar için güvenli (safe) değildir bu nedenle okuma ve yazma adımlarının senkronizasyonu için sync.RWMutex gibi fonksiyonlardan yararlanılır (Go dilinde Concurrent programlama konusuna ilerleyen zamanlarda bakmaya çalışacağım) Şimdi basit örnekler ile map türünün kullanımına bir bakalım.

```text
package main

import (
	"fmt"
	)
	
func main(){
	var words map[string]string
	words=make(map[string]string) // map'i kullanabilmek icin make ile olusturmamiz gerekir.Yoksa calisma zamaninda hata aliriz
	
	words["red"]="rot"
	words["black"]="schwarz"
	words["blue"]="blau"
	words["green"]="grun"
	
	fmt.Println(words)
	fmt.Println(words["blue"])
	fmt.Println(words["empty"])
	
	// bir key degerinin map icinde olup olmadigini anlamak icin asagidaki ifadeyi kullanabiliriz
	_, isExist:=words["empty"]
	fmt.Println("words[empty] is exist = ",isExist)
	
	for key,value:=range words{
		fmt.Printf("key:%s\tvalue:%s\n",key,value)
	}
	
	// Bir map'i asagidaki gibi ayni ifadede tanimlayip elemanlarini belirleyebiliriz
	levels:=map[int]string{
		100 : "Beginners",
		200 : "Intermediate",
		300 : "Amateur",
		400 : "Pro",
		500 : "Master",
		0 : "unknown",		
	}
	WriteMapToConsole(levels)
	// map'ten eleman silmek icin delete kullanabiliriz
	fmt.Println("deleting 0")
	delete(levels,0)
	WriteMapToConsole(levels)
}

func WriteMapToConsole(m map[int]string){
	for k,v:=range m{
		fmt.Printf("key:%d -> value:%s\n",k,v)
	}
}
```

![gomaps_1.gif](/assets/images/2017/gomaps_1.gif)

words isimli map değişkeni string tipinden key ve value değerleri taşıyacak şekilde tanımlanıyor. İlk satırda bir tanımlama yapıyoruz ve sonrasında make metodu ile map değişkenini oluşturuyoruz. Bu şekilde yaptığımız bir map tanımından sonra make komutunu kullanmadan eleman eklemeye kalkarsak çalışma zamanı hatası alırız (Panic:)). words isimli map'e bir kaç key:value çifti ekliyor ve ekrana yazdırıyoruz. Olmayan bir eleman talep edildiğinde (words["empty"] gibi) geriye value'nun tipinin varsayılan değeri dönecektir. Dilersek _,isExist satırında olduğu ilgili key değerinin map içerisinde bulunup bulunmadığını kontrol ederek de ilerleyebiliriz. Bu kontrol sırasında gereksiz yere değişken ataması da yapmamış oluruz. Bir map içerisinde dönmek oldukça basittir. for döngüsünde key,value değerlerine birlikte erişme şansına sahibiz. Anahtar kelimemiz tahmin edileceği üzere range. levels isimli map değişkenini tek bir ifade içerisinde hem tanımlıyor hem de elemanlarını atıyoruz. Bir map serisinden eleman silmek için delete fonksiyonundan yararlanabiliriz.

map'ler ile ilgili enteresan konulardan birisi de value içerisinde de map'ler barındırılabileceği. Aşağıda bu durumu anlatan örnek bir kod parçasına yer veriliyor.

```text
package main

import (
	"fmt"
	)
	
func main(){
	css:=map[string]map[string]string{
		"table" : map[string]string{
			"fontColor" : "white",
			"backgrounColod" : "blue",
		},
		"button" : map[string]string{
			"fontName" : "tahoma",
			"fonsSize" : "24",
			"fontColor" : "red",
			"backColor" : "black",
		},
	}
	
	for k,v:=range css{
		fmt.Println(k)
		for sk,sv:=range v{
			fmt.Printf("\t%s:%s\n",sk,sv)
		}
	}
}
```

css isimli map'in value değerleri birer map olarak tanımlanmıştır. Buna göre her bir key'e karşılık string tipinden key:value çiftleri taşıyan map'ler söz konusudur. Tüm map içeriğini içiçe for döngüsü ile dolaşmamız da mümkündür. İçteki for döngüsü dıştaki value tipini ele alacak şekilde ileri yönlü bir iterasyonu başlatabilir. Kodun çalışma zamanı çıktısı ise aşağıdaki gibi olacaktır.

![gomaps_2.gif](/assets/images/2017/gomaps_2.gif)

Gopher olma çalışmalarımızla ilgili olarak bu yazımızda slice ve map kavramlarına değinmeye çalıştık. Bir başka makalemizde görüşünceye dek hepinize mutlu günler dilerim.
